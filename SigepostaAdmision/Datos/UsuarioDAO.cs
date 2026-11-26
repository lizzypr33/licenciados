using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// DAO (Data Access Object) de la tabla Usuario: es la ÚNICA clase que
    /// escribe SQL para usuarios. Las demás capas trabajan con objetos Usuario.
    ///
    /// Seguridad: todas las consultas son PARAMETRIZADAS (@NombreUsuario, etc.).
    /// Nunca se concatenan datos escritos por el usuario dentro del SQL, lo que
    /// evita la INYECCIÓN SQL (por ejemplo, escribir  ' OR 1=1 --  como usuario).
    ///
    /// Se usa "using" en conexión, comando y lector para liberar los recursos
    /// (y devolver la conexión al pool) aunque ocurra una excepción.
    /// </summary>
    public class UsuarioDAO
    {
        /// <summary>
        /// Busca un usuario por su nombre y devuelve sus credenciales (hash y sal)
        /// por parámetros de salida. Devuelve null si el usuario no existe o está
        /// inactivo (en ese caso hash y salt también quedan en null).
        /// </summary>
        public Usuario ObtenerCredenciales(string nombreUsuario, out string hash, out string salt)
        {
            hash = null;
            salt = null;

            const string sql =
                "SELECT IdUsuario, NombreUsuario, ClaveHash, Salt, Rol, IdPosta, Estado " +
                "FROM dbo.Usuario WHERE NombreUsuario = @NombreUsuario";

            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@NombreUsuario", SqlDbType.VarChar, 30).Value = nombreUsuario;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!dr.Read())
                    {
                        return null;    // No existe
                    }

                    bool activo = dr.GetBoolean(dr.GetOrdinal("Estado"));
                    if (!activo)
                    {
                        return null;    // Existe pero está inactivo
                    }

                    // Si el rol guardado no es válido, se trata como sin acceso.
                    RolUsuario rol;
                    if (!Enum.TryParse(dr.GetString(dr.GetOrdinal("Rol")), false, out rol))
                    {
                        return null;
                    }

                    int ordPosta = dr.GetOrdinal("IdPosta");
                    int? idPosta = dr.IsDBNull(ordPosta) ? (int?)null : dr.GetInt32(ordPosta);

                    hash = dr.GetString(dr.GetOrdinal("ClaveHash"));
                    salt = dr.GetString(dr.GetOrdinal("Salt"));

                    return new Usuario(
                        dr.GetInt32(dr.GetOrdinal("IdUsuario")),
                        dr.GetString(dr.GetOrdinal("NombreUsuario")),
                        rol,
                        idPosta,
                        activo);
                }
            }
        }

        /// <summary>Devuelve la cantidad total de usuarios registrados.</summary>
        public int ContarUsuarios()
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Usuario";

            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Inserta un nuevo usuario con su hash y sal ya calculados (nunca la
        /// contraseña en texto plano). Devuelve el IdUsuario generado.
        /// </summary>
        public int Crear(Usuario u, string hash, string salt)
        {
            if (u == null) throw new ArgumentNullException("u");
            if (string.IsNullOrEmpty(hash)) throw new ArgumentException("El hash es obligatorio.", "hash");
            if (string.IsNullOrEmpty(salt)) throw new ArgumentException("La sal es obligatoria.", "salt");

            const string sql =
                "INSERT INTO dbo.Usuario (NombreUsuario, ClaveHash, Salt, Rol, IdPosta, Estado) " +
                "VALUES (@NombreUsuario, @ClaveHash, @Salt, @Rol, @IdPosta, @Estado); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using (SqlConnection cn = Conexion.Crear())
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@NombreUsuario", SqlDbType.VarChar, 30).Value = u.NombreUsuario;
                    cmd.Parameters.Add("@ClaveHash", SqlDbType.VarChar, 64).Value = hash;
                    cmd.Parameters.Add("@Salt", SqlDbType.VarChar, 32).Value = salt;
                    cmd.Parameters.Add("@Rol", SqlDbType.VarChar, 20).Value = u.Rol.ToString();
                    cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value =
                        u.IdPosta.HasValue ? (object)u.IdPosta.Value : DBNull.Value;
                    cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = u.Estado;

                    cn.Open();
                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    u.IdUsuario = id;
                    return id;
                }
            }
            catch (SqlException ex) when (UtilSql.EsClaveDuplicada(ex))
            {
                // NombreUsuario es UNIQUE: otro administrador creó el mismo nombre a la vez.
                throw new RegistroDuplicadoException("Ya existe un usuario con ese nombre.", "NombreUsuario", ex);
            }
        }

        // ===================== Módulo Usuarios =====================

        private const string Administrador = "Administrador";

        // SELECT común: usuario + nombre de su posta (LEFT JOIN: el Administrador puede no tener).
        private const string SelectResumen =
            "SELECT u.IdUsuario, u.NombreUsuario, u.Rol, u.IdPosta, p.Nombre AS Posta, u.Estado " +
            "FROM dbo.Usuario u LEFT JOIN dbo.Posta p ON p.IdPosta = u.IdPosta ";

        /// <summary>Todos los usuarios (sin hash ni sal), ordenados por nombre.</summary>
        public List<UsuarioResumen> Listar()
        {
            return ConsultarResumen(SelectResumen + "ORDER BY u.NombreUsuario", null);
        }

        /// <summary>Un usuario por su Id (sin hash ni sal), o null si no existe.</summary>
        public UsuarioResumen ObtenerPorId(int idUsuario)
        {
            List<UsuarioResumen> lista = ConsultarResumen(SelectResumen + "WHERE u.IdUsuario = @IdUsuario",
                cmd => cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario);
            return lista.Count > 0 ? lista[0] : null;
        }

        /// <summary>¿Ya existe un usuario con ese nombre? (activo o inactivo)</summary>
        public bool ExisteNombre(string nombre)
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Usuario WHERE NombreUsuario = @NombreUsuario";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@NombreUsuario", SqlDbType.VarChar, 30).Value = nombre;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        /// <summary>Cantidad de usuarios con rol Administrador y estado activo.</summary>
        public int ContarAdministradoresActivos()
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Usuario WHERE Rol = @Administrador AND Estado = 1";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Administrador", SqlDbType.VarChar, 20).Value = Administrador;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Cambia el rol y la posta. Si el usuario es un Administrador activo y deja de
        /// serlo, el propio UPDATE exige que quede al menos OTRO Administrador activo
        /// (así dos administradores actuando a la vez no pueden dejar el sistema sin
        /// ninguno). Devuelve true si se actualizó.
        /// </summary>
        public bool ActualizarRolYPosta(int idUsuario, RolUsuario rol, int? idPosta)
        {
            const string sql =
                "UPDATE dbo.Usuario SET Rol = @Rol, IdPosta = @IdPosta " +
                "WHERE IdUsuario = @IdUsuario " +
                "  AND (@Rol = @Administrador OR Rol <> @Administrador OR Estado = 0 " +
                "       OR (SELECT COUNT(*) FROM dbo.Usuario WHERE Rol = @Administrador AND Estado = 1) > 1)";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Rol", SqlDbType.VarChar, 20).Value = rol.ToString();
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value = idPosta.HasValue ? (object)idPosta.Value : DBNull.Value;
                cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                cmd.Parameters.Add("@Administrador", SqlDbType.VarChar, 20).Value = Administrador;
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        /// <summary>
        /// Activa o desactiva (los usuarios nunca se borran). Al DESACTIVAR, el UPDATE
        /// exige que no sea el último Administrador activo. Devuelve true si se actualizó.
        /// </summary>
        public bool CambiarEstado(int idUsuario, bool activo)
        {
            const string sql =
                "UPDATE dbo.Usuario SET Estado = @Estado " +
                "WHERE IdUsuario = @IdUsuario " +
                "  AND (@Estado = 1 OR Rol <> @Administrador " +
                "       OR (SELECT COUNT(*) FROM dbo.Usuario WHERE Rol = @Administrador AND Estado = 1) > 1)";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = activo;
                cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                cmd.Parameters.Add("@Administrador", SqlDbType.VarChar, 20).Value = Administrador;
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        /// <summary>Guarda un nuevo hash y sal (nunca la contraseña). Devuelve true si se actualizó.</summary>
        public bool ActualizarClave(int idUsuario, string hash, string salt)
        {
            if (string.IsNullOrEmpty(hash)) throw new ArgumentException("El hash es obligatorio.", "hash");
            if (string.IsNullOrEmpty(salt)) throw new ArgumentException("La sal es obligatoria.", "salt");

            const string sql = "UPDATE dbo.Usuario SET ClaveHash = @ClaveHash, Salt = @Salt WHERE IdUsuario = @IdUsuario";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ClaveHash", SqlDbType.VarChar, 64).Value = hash;
                cmd.Parameters.Add("@Salt", SqlDbType.VarChar, 32).Value = salt;
                cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        private static List<UsuarioResumen> ConsultarResumen(string sql, Action<SqlCommand> parametros)
        {
            List<UsuarioResumen> lista = new List<UsuarioResumen>();
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
                    int ordPosta = dr.GetOrdinal("IdPosta");
                    int ordNombrePosta = dr.GetOrdinal("Posta");
                    while (dr.Read())
                    {
                        RolUsuario rol;
                        Enum.TryParse(dr.GetString(dr.GetOrdinal("Rol")), false, out rol);
                        lista.Add(new UsuarioResumen
                        {
                            IdUsuario = dr.GetInt32(dr.GetOrdinal("IdUsuario")),
                            NombreUsuario = dr.GetString(dr.GetOrdinal("NombreUsuario")),
                            Rol = rol,
                            IdPosta = dr.IsDBNull(ordPosta) ? (int?)null : dr.GetInt32(ordPosta),
                            Posta = dr.IsDBNull(ordNombrePosta) ? string.Empty : dr.GetString(ordNombrePosta),
                            Estado = dr.GetBoolean(dr.GetOrdinal("Estado"))
                        });
                    }
                }
            }
            return lista;
        }
    }
}
