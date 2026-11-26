using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// DAO de la tabla Paciente: la única clase que escribe SQL para pacientes.
    /// Devuelve y recibe entidades Paciente, nunca DataTables ni SQL suelto.
    ///
    /// Seguridad: todas las consultas son PARAMETRIZADAS. Incluso la búsqueda
    /// con LIKE recibe el patrón como parámetro (@Patron), nunca concatenado.
    /// Se usa "using" en conexión, comando, transacción y lector para liberar
    /// recursos aunque ocurra una excepción.
    ///
    /// Las consultas hacen LEFT JOIN con HistoriaClinica para traer también el
    /// número de historia del paciente (queda vacío si aún no tiene).
    ///
    /// Esta clase NO valida reglas de negocio (eso es de PacienteNegocio); solo
    /// traduce entre objetos y filas de la base de datos.
    ///
    /// DNI (Unidad 4): la columna Dni en texto plano YA NO se usa. Al guardar se
    /// escriben DniCifrado (AES-256, para poder mostrarlo) y DniHash (HMAC-SHA256,
    /// para buscar por DNI exacto y detectar duplicados). Al leer se descifra
    /// DniCifrado y se asigna a Paciente.Dni. La búsqueda por DNI parcial ya no es
    /// posible (ni deseable): el hash solo permite coincidencia exacta.
    /// </summary>
    public class PacienteDAO
    {
        // SELECT común a todas las lecturas. {0} se reemplaza por "TOP (n)" o por "".
        private const string SelectBase =
            "SELECT {0} p.IdPaciente, p.DniCifrado, p.ApellidoPaterno, p.ApellidoMaterno, p.Nombres, " +
            "p.FechaNacimiento, p.Sexo, p.Direccion, p.Telefono, h.NumeroHistoria " +
            "FROM dbo.Paciente p LEFT JOIN dbo.HistoriaClinica h ON h.IdPaciente = p.IdPaciente ";

        private static readonly string Top = "TOP (" + UtilSql.MaximoResultados + ")";

        /// <summary>
        /// Inserta SOLO el paciente (sin historia) y devuelve el IdPaciente generado.
        /// El registro normal usa InsertarConHistoria; este método se conserva
        /// para casos especiales (por ejemplo, migrar datos antiguos).
        /// </summary>
        public int Insertar(Paciente p)
        {
            if (p == null) throw new ArgumentNullException("p");

            try
            {
                using (SqlConnection cn = Conexion.Crear())
                {
                    cn.Open();
                    return InsertarPaciente(cn, null, p);
                }
            }
            catch (SqlException ex) when (UtilSql.EsClaveDuplicada(ex))
            {
                throw DuplicadoDni(p, ex);
            }
        }

        /// <summary>
        /// Registra al paciente Y le abre su historia clínica en UNA SOLA TRANSACCIÓN.
        ///
        /// ¿Por qué una transacción? Porque son tres pasos que deben ocurrir
        /// "todos o ninguno" (atomicidad):
        ///   a) INSERT del paciente (se obtiene su IdPaciente con SCOPE_IDENTITY()),
        ///   b) siguiente número de la secuencia dbo.SeqNumeroHistoria → "HC-000007",
        ///   c) INSERT de la historia clínica con ese número y estado 'Activa'.
        /// Sin transacción, si fallara el paso (c) quedaría un paciente SIN historia,
        /// lo que rompe la regla "todo paciente tiene su historia". Con la
        /// transacción, ante cualquier error se hace Rollback y no queda ni el
        /// paciente ni la historia; solo con Commit se guardan ambos a la vez.
        ///
        /// Nota: los valores de una SEQUENCE no se devuelven al hacer Rollback,
        /// por eso pueden quedar "saltos" en la numeración (HC-000007, HC-000009).
        /// Es normal y aceptado: lo importante es que el número sea ÚNICO.
        /// </summary>
        public int InsertarConHistoria(Paciente p, out string numeroHistoria)
        {
            if (p == null) throw new ArgumentNullException("p");

            try
            {
                using (SqlConnection cn = Conexion.Crear())
                {
                    cn.Open();
                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            int idPaciente = InsertarPaciente(cn, tx, p);                     // (a)
                            string numero = HistoriaDAO.InsertarHistoria(cn, tx, idPaciente); // (b) y (c)

                            tx.Commit();    // Recién aquí se guardan los dos registros

                            p.NumeroHistoriaClinica = numero;
                            numeroHistoria = numero;
                            return idPaciente;
                        }
                        catch
                        {
                            UtilSql.DeshacerSeguro(tx);   // No queda ni el paciente ni la historia
                            p.IdPaciente = 0;
                            throw;                        // Se relanza la excepción original
                        }
                    }
                }
            }
            catch (SqlException ex) when (UtilSql.EsClaveDuplicada(ex))
            {
                // El mensaje de SQL Server nombra la tabla donde se repitió la clave.
                if (ex.Message.IndexOf("HistoriaClinica", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new RegistroDuplicadoException(
                        "No se pudo asignar un número de historia único. Intente nuevamente.",
                        "NumeroHistoria", ex);
                }
                throw DuplicadoDni(p, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un paciente (identificado por IdPaciente).
        /// El DNI no cambia (Negocio envía el mismo DNI ya guardado), pero se vuelve
        /// a escribir cifrado (con un IV nuevo) junto con su huella: así un paciente
        /// antiguo que no tuviera DniCifrado/DniHash queda completo al editarlo.
        /// Devuelve true si se actualizó una fila.
        /// </summary>
        public bool Actualizar(Paciente p)
        {
            if (p == null) throw new ArgumentNullException("p");

            const string sql =
                "UPDATE dbo.Paciente SET " +
                "DniCifrado = @DniCifrado, DniHash = @DniHash, " +
                "ApellidoPaterno = @ApellidoPaterno, ApellidoMaterno = @ApellidoMaterno, " +
                "Nombres = @Nombres, FechaNacimiento = @FechaNacimiento, Sexo = @Sexo, " +
                "Direccion = @Direccion, Telefono = @Telefono " +
                "WHERE IdPaciente = @IdPaciente";

            try
            {
                using (SqlConnection cn = Conexion.Crear())
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = p.IdPaciente;
                    AgregarParametrosDni(cmd, p.Dni);
                    AgregarParametrosEditables(cmd, p);
                    cn.Open();
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
            catch (SqlException ex) when (UtilSql.EsClaveDuplicada(ex))
            {
                throw new RegistroDuplicadoException("Los datos del paciente duplican un registro existente.", ex);
            }
        }

        /// <summary>
        /// Devuelve el paciente con ese DNI, o null si no existe. Se busca por la
        /// huella HMAC (DniHash): el DNI cifrado no se puede comparar directamente.
        /// </summary>
        public Paciente ObtenerPorDni(string dni)
        {
            string sql = string.Format(SelectBase, "") + "WHERE p.DniHash = @DniHash";
            List<Paciente> lista = Consultar(sql, cmd => UtilSql.AgregarFiltroDniHash(cmd, dni));
            return lista.Count > 0 ? lista[0] : null;
        }

        /// <summary>Devuelve el paciente con ese Id, o null si no existe.</summary>
        public Paciente ObtenerPorId(int id)
        {
            string sql = string.Format(SelectBase, "") + "WHERE p.IdPaciente = @IdPaciente";
            List<Paciente> lista = Consultar(sql, cmd =>
                cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = id);
            return lista.Count > 0 ? lista[0] : null;
        }

        /// <summary>
        /// Búsqueda:
        ///   - Si el texto son exactamente 8 dígitos → por DNI exacto (DniHash).
        ///   - Si no → coincidencia parcial en apellidos o nombres (LIKE), incluido
        ///     el nombre completo "Paterno Materno Nombres".
        /// Ya no se busca por DNI parcial: el DNI está cifrado y su huella solo
        /// permite coincidencia exacta. Máximo 100 resultados, por apellidos.
        /// </summary>
        public List<Paciente> Buscar(string texto)
        {
            string valor = (texto ?? string.Empty).Trim();
            string orden = "ORDER BY p.ApellidoPaterno, p.ApellidoMaterno, p.Nombres";

            if (UtilSql.EsDniCompleto(valor))
            {
                string sqlDni = string.Format(SelectBase, Top) + "WHERE p.DniHash = @DniHash " + orden;
                return Consultar(sqlDni, cmd => UtilSql.AgregarFiltroDniHash(cmd, valor));
            }

            string sql = string.Format(SelectBase, Top) +
                "WHERE p.ApellidoPaterno LIKE @Patron " +
                "   OR p.ApellidoMaterno LIKE @Patron " +
                "   OR p.Nombres LIKE @Patron " +
                "   OR (p.ApellidoPaterno + ' ' + p.ApellidoMaterno + ' ' + p.Nombres) LIKE @Patron " +
                orden;

            return Consultar(sql, cmd =>
                cmd.Parameters.Add("@Patron", SqlDbType.VarChar, 200).Value = UtilSql.PatronContiene(valor));
        }

        /// <summary>
        /// Pacientes DISTINTOS que tienen al menos una cita no cancelada en
        /// consultorios de la posta indicada (JOIN Cita → Consultorio). Se usa
        /// DISTINCT porque un paciente con varias citas aparecería varias veces.
        /// </summary>
        public List<Paciente> ListarPorPosta(int idPosta)
        {
            return ListarConCitas(idPosta);
        }

        /// <summary>
        /// Igual que ListarPorPosta, pero de TODAS las postas (para el
        /// Administrador, que no tiene una posta asignada).
        /// </summary>
        public List<Paciente> ListarConCitasTodasLasPostas()
        {
            return ListarConCitas(null);
        }

        // idPosta null = sin filtro de posta (sigue siendo una consulta parametrizada).
        private static List<Paciente> ListarConCitas(int? idPosta)
        {
            string sql = string.Format(SelectBase, "DISTINCT") +
                "INNER JOIN dbo.Cita c ON c.IdPaciente = p.IdPaciente " +
                "INNER JOIN dbo.Consultorio co ON co.IdConsultorio = c.IdConsultorio " +
                "WHERE (@IdPosta IS NULL OR co.IdPosta = @IdPosta) AND c.Estado <> @Cancelada " +
                "ORDER BY p.ApellidoPaterno, p.ApellidoMaterno, p.Nombres";

            return Consultar(sql, cmd =>
            {
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value =
                    idPosta.HasValue ? (object)idPosta.Value : DBNull.Value;
                cmd.Parameters.Add("@Cancelada", SqlDbType.VarChar, 10).Value = EstadoCita.Cancelada.ToString();
            });
        }

        /// <summary>Devuelve los 100 pacientes registrados más recientemente.</summary>
        public List<Paciente> Listar()
        {
            string sql = string.Format(SelectBase, Top) + "ORDER BY p.FechaRegistro DESC, p.IdPaciente DESC";
            return Consultar(sql, null);
        }

        /// <summary>Indica si ya existe un paciente con ese DNI (comparando la huella DniHash).</summary>
        public bool ExisteDni(string dni)
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Paciente WHERE DniHash = @DniHash";

            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                UtilSql.AgregarFiltroDniHash(cmd, dni);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // ================= Métodos de apoyo =================

        /// <summary>
        /// Ejecuta el INSERT del paciente con la conexión (y transacción, si hay)
        /// recibidas, y devuelve el IdPaciente generado con SCOPE_IDENTITY().
        /// </summary>
        private static int InsertarPaciente(SqlConnection cn, SqlTransaction tx, Paciente p)
        {
            // La columna Dni (texto plano) no se escribe: solo DniCifrado y DniHash.
            const string sql =
                "INSERT INTO dbo.Paciente (DniCifrado, DniHash, ApellidoPaterno, ApellidoMaterno, Nombres, " +
                "FechaNacimiento, Sexo, Direccion, Telefono) " +
                "VALUES (@DniCifrado, @DniHash, @ApellidoPaterno, @ApellidoMaterno, @Nombres, " +
                "@FechaNacimiento, @Sexo, @Direccion, @Telefono); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
            {
                AgregarParametrosDni(cmd, p.Dni);
                AgregarParametrosEditables(cmd, p);

                int id = Convert.ToInt32(cmd.ExecuteScalar());
                p.IdPaciente = id;
                return id;
            }
        }

        private static RegistroDuplicadoException DuplicadoDni(Paciente p, SqlException ex)
        {
            return new RegistroDuplicadoException(
                "Ya existe un paciente registrado con el DNI " + p.Dni + ".", "Dni", ex);
        }

        /// <summary>
        /// @DniCifrado = AES-256 del DNI (IV + datos) y @DniHash = HMAC-SHA256 del DNI.
        /// </summary>
        private static void AgregarParametrosDni(SqlCommand cmd, string dni)
        {
            cmd.Parameters.Add("@DniCifrado", SqlDbType.VarBinary, 64).Value = Seguridad.CifradoAes.Cifrar(dni);
            cmd.Parameters.Add("@DniHash", SqlDbType.Char, 64).Value = Seguridad.HashDni.Calcular(dni);
        }

        // Parámetros comunes a INSERT y UPDATE (todo menos el DNI y el IdPaciente).
        private static void AgregarParametrosEditables(SqlCommand cmd, Paciente p)
        {
            cmd.Parameters.Add("@ApellidoPaterno", SqlDbType.VarChar, 40).Value = p.ApellidoPaterno;
            cmd.Parameters.Add("@ApellidoMaterno", SqlDbType.VarChar, 40).Value = p.ApellidoMaterno;
            cmd.Parameters.Add("@Nombres", SqlDbType.VarChar, 60).Value = p.Nombres;
            cmd.Parameters.Add("@FechaNacimiento", SqlDbType.Date).Value = p.FechaNacimiento.Date;
            cmd.Parameters.Add("@Sexo", SqlDbType.Char, 1).Value = p.Sexo.ToString();
            cmd.Parameters.Add("@Direccion", SqlDbType.VarChar, 120).Value = UtilSql.TextoONulo(p.Direccion);
            cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 15).Value = UtilSql.TextoONulo(p.Telefono);
        }

        // Ejecuta un SELECT y convierte cada fila en un Paciente.
        // "parametros" es un delegado que agrega los @parámetros al comando.
        private static List<Paciente> Consultar(string sql, Action<SqlCommand> parametros)
        {
            List<Paciente> lista = new List<Paciente>();

            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                if (parametros != null)
                {
                    parametros(cmd);
                }
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(Mapear(dr));
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Convierte la fila actual del lector en un objeto Paciente. Es "internal"
        /// para que HistoriaDAO lo reutilice (sus consultas traen las mismas columnas).
        /// </summary>
        internal static Paciente Mapear(SqlDataReader dr)
        {
            int ordDireccion = dr.GetOrdinal("Direccion");
            int ordTelefono = dr.GetOrdinal("Telefono");
            int ordHistoria = dr.GetOrdinal("NumeroHistoria");
            int idPaciente = dr.GetInt32(dr.GetOrdinal("IdPaciente"));

            // El DNI se descifra aquí y solo vive en memoria (propiedad Paciente.Dni).
            string dni = UtilSql.LeerDni(dr, "DniCifrado");
            if (dni.Length == 0)
            {
                throw new InvalidOperationException(
                    "El paciente " + idPaciente + " no tiene DNI cifrado. Ejecute la migración de seguridad.");
            }

            Paciente p = new Paciente(
                idPaciente,
                dni,
                dr.GetString(dr.GetOrdinal("ApellidoPaterno")),
                dr.GetString(dr.GetOrdinal("ApellidoMaterno")),
                dr.GetString(dr.GetOrdinal("Nombres")),
                dr.GetDateTime(dr.GetOrdinal("FechaNacimiento")),
                dr.GetString(dr.GetOrdinal("Sexo"))[0],
                dr.IsDBNull(ordDireccion) ? string.Empty : dr.GetString(ordDireccion),
                dr.IsDBNull(ordTelefono) ? string.Empty : dr.GetString(ordTelefono));

            p.NumeroHistoriaClinica = dr.IsDBNull(ordHistoria) ? string.Empty : dr.GetString(ordHistoria);
            return p;
        }
    }
}
