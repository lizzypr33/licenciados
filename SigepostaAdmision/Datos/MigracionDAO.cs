using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// SQL de la migración de seguridad (Unidad 4). Es el ÚNICO lugar del sistema
    /// que lee o escribe la antigua columna Paciente.Dni (texto plano), y solo para
    /// trasladar su contenido a DniCifrado / DniHash y luego vaciarla.
    /// </summary>
    public class MigracionDAO
    {
        /// <summary>¿Existe todavía la columna Dni en la tabla Paciente? (consulta a sys.columns)</summary>
        public bool ExisteColumnaDni()
        {
            const string sql =
                "SELECT COUNT(*) FROM sys.columns " +
                "WHERE object_id = OBJECT_ID(N'dbo.Paciente') AND name = N'Dni'";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// Migra, en UNA transacción, los pacientes con Dni en texto y sin DniCifrado:
        /// calcula DniCifrado (AES) y DniHash (HMAC), los guarda y pone Dni = NULL.
        /// Además borra el Dni en texto de los que ya tenían DniCifrado (para no
        /// dejar ningún DNI legible). Devuelve cuántos pacientes migró.
        ///
        /// ¿Por qué una transacción? Si fallara a mitad (por ejemplo, un DNI inválido
        /// o una caída de conexión), quedarían unos pacientes migrados y otros no.
        /// Con Rollback, la tabla vuelve a su estado original y se puede reintentar.
        ///
        /// Es IDEMPOTENTE: la segunda vez no hay filas con "Dni no nulo y DniCifrado
        /// nulo", así que no hace nada y devuelve 0.
        ///
        /// IMPORTANTE: solo llamar si ExisteColumnaDni() es true (el SQL menciona la
        /// columna Dni y fallaría si ya fue eliminada).
        /// </summary>
        public int MigrarDniPacientes()
        {
            using (SqlConnection cn = Conexion.Crear())
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) Pendientes (se bloquean hasta el Commit para que nadie los cambie a la vez).
                        List<KeyValuePair<int, string>> pendientes = new List<KeyValuePair<int, string>>();
                        const string sqlLeer =
                            "SELECT IdPaciente, Dni FROM dbo.Paciente WITH (UPDLOCK, HOLDLOCK) " +
                            "WHERE Dni IS NOT NULL AND DniCifrado IS NULL";
                        using (SqlCommand cmd = new SqlCommand(sqlLeer, cn, tx))
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                pendientes.Add(new KeyValuePair<int, string>(dr.GetInt32(0), dr.GetString(1).Trim()));
                            }
                        }

                        // 2) Cifrar, calcular la huella y vaciar la columna Dni de cada uno.
                        const string sqlActualizar =
                            "UPDATE dbo.Paciente SET DniCifrado = @DniCifrado, DniHash = @DniHash, Dni = NULL " +
                            "WHERE IdPaciente = @IdPaciente";
                        foreach (KeyValuePair<int, string> paciente in pendientes)
                        {
                            if (!UtilSql.EsDniCompleto(paciente.Value))
                            {
                                // Un DNI inválido detiene TODO (Rollback): mejor corregir el dato que cifrar basura.
                                throw new Entidades.ReglaNegocioException(
                                    "El paciente " + paciente.Key + " tiene un DNI inválido en la columna Dni; " +
                                    "corríjalo antes de migrar.");
                            }

                            using (SqlCommand cmd = new SqlCommand(sqlActualizar, cn, tx))
                            {
                                cmd.Parameters.Add("@DniCifrado", SqlDbType.VarBinary, 64).Value = CifradoAes.Cifrar(paciente.Value);
                                cmd.Parameters.Add("@DniHash", SqlDbType.Char, 64).Value = HashDni.Calcular(paciente.Value);
                                cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = paciente.Key;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3) Ningún DNI debe quedar en texto plano, aunque ya estuviera cifrado.
                        const string sqlLimpiar =
                            "UPDATE dbo.Paciente SET Dni = NULL WHERE Dni IS NOT NULL AND DniCifrado IS NOT NULL";
                        using (SqlCommand cmd = new SqlCommand(sqlLimpiar, cn, tx))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return pendientes.Count;
                    }
                    catch
                    {
                        UtilSql.DeshacerSeguro(tx);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve un paciente cualquiera ya cifrado (DniCifrado y DniHash), o null
        /// si no hay ninguno. Sirve para comprobar que las claves de este equipo
        /// corresponden a los datos de la base.
        /// </summary>
        public Tuple<byte[], string> ObtenerMuestraCifrada()
        {
            const string sql =
                "SELECT TOP (1) DniCifrado, DniHash FROM dbo.Paciente " +
                "WHERE DniCifrado IS NOT NULL AND DniHash IS NOT NULL ORDER BY IdPaciente";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read() ? Tuple.Create((byte[])dr.GetValue(0), dr.GetString(1)) : null;
                }
            }
        }
    }
}
