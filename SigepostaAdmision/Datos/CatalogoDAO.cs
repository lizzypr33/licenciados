using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// Consultas de catálogos (servicios, consultorios y personal) para llenar
    /// listas desplegables. Solo lectura, parametrizadas y con "using".
    ///
    /// Filtro por posta: si idPosta es null (por ejemplo, el Administrador sin
    /// posta asignada) NO se filtra y se devuelven los de todas las postas.
    /// Se logra con "(@IdPosta IS NULL OR c.IdPosta = @IdPosta)", que sigue
    /// siendo una consulta parametrizada.
    /// </summary>
    public class CatalogoDAO
    {
        /// <summary>Servicios que tienen al menos un consultorio en la posta.</summary>
        public List<ServicioItem> ServiciosConConsultorio(int? idPosta)
        {
            const string sql =
                "SELECT DISTINCT s.IdServicio, s.Nombre " +
                "FROM dbo.Servicio s INNER JOIN dbo.Consultorio c ON c.IdServicio = s.IdServicio " +
                "WHERE (@IdPosta IS NULL OR c.IdPosta = @IdPosta) " +
                "ORDER BY s.Nombre";

            List<ServicioItem> lista = new List<ServicioItem>();
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value = Nulable(idPosta);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ServicioItem
                        {
                            IdServicio = dr.GetInt32(0),
                            Nombre = dr.GetString(1)
                        });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Consultorios de la posta y del servicio indicados.
        /// idPosta null = todas las postas; idServicio null = todos los servicios.
        /// </summary>
        public List<ConsultorioItem> ConsultoriosPorPostaYServicio(int? idPosta, int? idServicio)
        {
            const string sql =
                "SELECT c.IdConsultorio, c.Nombre, c.IdPosta, c.IdServicio, s.Nombre AS Servicio " +
                "FROM dbo.Consultorio c INNER JOIN dbo.Servicio s ON s.IdServicio = c.IdServicio " +
                "WHERE (@IdPosta IS NULL OR c.IdPosta = @IdPosta) " +
                "  AND (@IdServicio IS NULL OR c.IdServicio = @IdServicio) " +
                "ORDER BY c.Nombre";

            return ConsultarConsultorios(sql, cmd =>
            {
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value = Nulable(idPosta);
                cmd.Parameters.Add("@IdServicio", SqlDbType.Int).Value = Nulable(idServicio);
            });
        }

        /// <summary>Un consultorio por su Id, o null si no existe.</summary>
        public ConsultorioItem ObtenerConsultorio(int idConsultorio)
        {
            const string sql =
                "SELECT c.IdConsultorio, c.Nombre, c.IdPosta, c.IdServicio, s.Nombre AS Servicio " +
                "FROM dbo.Consultorio c INNER JOIN dbo.Servicio s ON s.IdServicio = c.IdServicio " +
                "WHERE c.IdConsultorio = @IdConsultorio";

            List<ConsultorioItem> lista = ConsultarConsultorios(sql, cmd =>
                cmd.Parameters.Add("@IdConsultorio", SqlDbType.Int).Value = idConsultorio);
            return lista.Count > 0 ? lista[0] : null;
        }

        /// <summary>Personal de salud de la posta (null = de todas las postas).</summary>
        public List<ProfesionalItem> PersonalPorPosta(int? idPosta)
        {
            const string sql =
                "SELECT IdPersonal, Nombres, Apellidos, Tipo, Especialidad, IdPosta " +
                "FROM dbo.PersonalSalud " +
                "WHERE (@IdPosta IS NULL OR IdPosta = @IdPosta) " +
                "ORDER BY Apellidos, Nombres";

            return ConsultarPersonal(sql, cmd =>
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value = Nulable(idPosta));
        }

        /// <summary>Un miembro del personal por su Id, o null si no existe.</summary>
        public ProfesionalItem ObtenerPersonal(int idPersonal)
        {
            const string sql =
                "SELECT IdPersonal, Nombres, Apellidos, Tipo, Especialidad, IdPosta " +
                "FROM dbo.PersonalSalud WHERE IdPersonal = @IdPersonal";

            List<ProfesionalItem> lista = ConsultarPersonal(sql, cmd =>
                cmd.Parameters.Add("@IdPersonal", SqlDbType.Int).Value = idPersonal);
            return lista.Count > 0 ? lista[0] : null;
        }

        /// <summary>Todas las postas (IdPosta, Nombre), ordenadas por nombre.</summary>
        public List<PostaItem> Postas()
        {
            const string sql = "SELECT IdPosta, Nombre FROM dbo.Posta ORDER BY Nombre";
            List<PostaItem> lista = new List<PostaItem>();
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new PostaItem { IdPosta = dr.GetInt32(0), Nombre = dr.GetString(1) });
                    }
                }
            }
            return lista;
        }

        // ===================== Apoyo =====================

        private static object Nulable(int? valor)
        {
            return valor.HasValue ? (object)valor.Value : DBNull.Value;
        }

        private static List<ConsultorioItem> ConsultarConsultorios(string sql, Action<SqlCommand> parametros)
        {
            List<ConsultorioItem> lista = new List<ConsultorioItem>();
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                parametros(cmd);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ConsultorioItem
                        {
                            IdConsultorio = dr.GetInt32(dr.GetOrdinal("IdConsultorio")),
                            Nombre = dr.GetString(dr.GetOrdinal("Nombre")),
                            IdPosta = dr.GetInt32(dr.GetOrdinal("IdPosta")),
                            IdServicio = dr.GetInt32(dr.GetOrdinal("IdServicio")),
                            Servicio = dr.GetString(dr.GetOrdinal("Servicio"))
                        });
                    }
                }
            }
            return lista;
        }

        private static List<ProfesionalItem> ConsultarPersonal(string sql, Action<SqlCommand> parametros)
        {
            List<ProfesionalItem> lista = new List<ProfesionalItem>();
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                parametros(cmd);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    int ordEspecialidad = dr.GetOrdinal("Especialidad");
                    while (dr.Read())
                    {
                        lista.Add(new ProfesionalItem
                        {
                            IdPersonal = dr.GetInt32(dr.GetOrdinal("IdPersonal")),
                            Nombres = dr.GetString(dr.GetOrdinal("Nombres")),
                            Apellidos = dr.GetString(dr.GetOrdinal("Apellidos")),
                            Tipo = dr.GetString(dr.GetOrdinal("Tipo")),
                            Especialidad = dr.IsDBNull(ordEspecialidad) ? string.Empty : dr.GetString(ordEspecialidad),
                            IdPosta = dr.GetInt32(dr.GetOrdinal("IdPosta"))
                        });
                    }
                }
            }
            return lista;
        }
    }
}
