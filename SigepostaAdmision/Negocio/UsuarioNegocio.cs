using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Reglas del módulo Usuarios y del cambio de contraseña propio.
    ///
    /// Reglas:
    ///   1. La gestión de usuarios es SOLO del Administrador (se verifica aquí, en
    ///      Negocio, aunque la pantalla ya esté restringida por Permisos).
    ///   2. Nombre de usuario: 4 a 30 caracteres (letras, números, punto y guion
    ///      bajo) y único.
    ///   3. El rol Admision exige una posta; el Administrador no lleva posta.
    ///   4. Los usuarios NUNCA se eliminan (los referencian citas, atenciones y la
    ///      auditoría): solo se desactivan.
    ///   5. Nadie puede desactivarse ni quitarse el rol de Administrador a sí mismo,
    ///      y siempre debe quedar al menos un Administrador activo.
    ///   6. Contraseña: mínimo 8 caracteres, al menos una letra y un número, distinta
    ///      del nombre de usuario y de las claves iniciales conocidas.
    ///   7. Las contraseñas temporales se generan con un generador CRIPTOGRÁFICO y se
    ///      muestran una sola vez; en la base solo se guarda su hash con sal.
    ///   8. Todo cambio queda en la auditoría, SIN contraseñas ni hashes.
    /// </summary>
    public class UsuarioNegocio
    {
        public const int MinimoClave = 8;
        public const int LargoClaveTemporal = 10;

        // Alfabeto de las contraseñas temporales SIN caracteres que se confunden al
        // leerlos o dictarlos: 0/O/o, 1/l/I.
        private const string Alfabeto = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";

        // Claves iniciales del sistema (están en el código fuente): no se permiten.
        private static readonly string[] ClavesProhibidas = { "Admin123*", "Admision123*" };

        private static readonly Regex FormatoNombre = new Regex("^[A-Za-z0-9._]{4,30}$");

        private readonly UsuarioDAO dao = new UsuarioDAO();
        private readonly CatalogoDAO catalogoDao = new CatalogoDAO();

        // ===================== Apoyo para la pantalla =====================

        public static bool EsAdministrador()
        {
            return Sesion.HaySesion && Sesion.UsuarioActual.Rol == RolUsuario.Administrador;
        }

        /// <summary>Texto del botón según el estado del usuario (el formulario no decide).</summary>
        public static string TextoBotonEstado(bool activo)
        {
            return activo ? "Desactivar" : "Activar";
        }

        /// <summary>¿Este rol necesita una posta? (solo Admision)</summary>
        public static bool RequierePosta(RolUsuario rol)
        {
            return rol == RolUsuario.Admision;
        }

        /// <summary>Reglas de la contraseña, en texto, para mostrarlas en pantalla.</summary>
        public static string TextoReglasClave
        {
            get
            {
                return "La contraseña debe tener al menos " + MinimoClave + " caracteres, con al menos una letra " +
                       "y un número, y no puede ser igual a su nombre de usuario ni a una clave inicial del sistema.";
            }
        }

        /// <summary>Postas para el ComboBox (desde el catálogo).</summary>
        public List<PostaItem> Postas()
        {
            return catalogoDao.Postas();
        }

        // ===================== Reglas de contraseña =====================

        /// <summary>Regla 6. Devuelve todos los errores juntos (sin lanzar excepciones).</summary>
        public ResultadoOperacion ValidarClave(string clave, string nombreUsuario)
        {
            ResultadoOperacion r = new ResultadoOperacion();
            string c = clave ?? string.Empty;

            if (c.Length < MinimoClave)
            {
                r.AgregarError("La contraseña debe tener al menos " + MinimoClave + " caracteres.");
            }
            if (!c.Any(char.IsLetter) || !c.Any(char.IsDigit))
            {
                r.AgregarError("La contraseña debe tener al menos una letra y un número.");
            }
            if (!string.IsNullOrEmpty(nombreUsuario) && string.Equals(c, nombreUsuario.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                r.AgregarError("La contraseña no puede ser igual al nombre de usuario.");
            }
            if (ClavesProhibidas.Any(p => string.Equals(c, p, StringComparison.Ordinal)))
            {
                r.AgregarError("La contraseña no puede ser una de las claves iniciales del sistema.");
            }
            return r;
        }

        // ===================== Gestión (solo Administrador) =====================

        /// <summary>Lista de usuarios (en ResultadoUsuario.Usuarios).</summary>
        public ResultadoUsuario Listar()
        {
            return Ejecutar("listar", () => new ResultadoUsuario { Usuarios = dao.Listar() });
        }

        /// <summary>
        /// Crea un usuario activo con una contraseña TEMPORAL aleatoria, que se
        /// devuelve UNA sola vez en ResultadoUsuario.ClaveTemporal.
        /// </summary>
        public ResultadoUsuario Crear(string nombre, RolUsuario rol, int? idPosta)
        {
            return Ejecutar("crear", () =>
            {
                string nombreLimpio = (nombre ?? string.Empty).Trim();
                ResultadoUsuario r = new ResultadoUsuario();

                // Regla 2: formato del nombre.
                if (!FormatoNombre.IsMatch(nombreLimpio))
                {
                    r.AgregarError("El nombre de usuario debe tener de 4 a 30 caracteres: letras, números, punto o guion bajo.");
                }

                // Regla 3: rol y posta.
                int? posta;
                string errorPosta = ValidarRolYPosta(rol, idPosta, out posta);
                if (errorPosta != null)
                {
                    r.AgregarError(errorPosta);
                }

                if (!r.Exito)
                {
                    return r;
                }

                // Regla 2: único (también lo garantiza la restricción UNIQUE de la tabla).
                if (dao.ExisteNombre(nombreLimpio))
                {
                    throw new ReglaNegocioException("Ya existe un usuario con el nombre \"" + nombreLimpio + "\".");
                }

                // Regla 7: contraseña temporal → solo su hash con sal se guarda.
                string temporal = GenerarClaveTemporal(nombreLimpio);
                string salt = Hash.GenerarSalt();
                Usuario u = new Usuario(0, nombreLimpio, rol, posta, true);
                r.IdGenerado = dao.Crear(u, Hash.CalcularHash(temporal, salt), salt);
                r.ClaveTemporal = temporal;

                new AuditoriaNegocio().Registrar("CREAR_USUARIO", "Usuario", r.IdGenerado, "Rol " + rol);
                return r;
            });
        }

        /// <summary>Cambia el rol y la posta de un usuario (reglas 3 y 5).</summary>
        public ResultadoUsuario ActualizarRolYPosta(int idUsuario, RolUsuario rol, int? idPosta)
        {
            return Ejecutar("editar", () =>
            {
                UsuarioResumen actual = ObtenerExistente(idUsuario);

                int? posta;
                string errorPosta = ValidarRolYPosta(rol, idPosta, out posta);
                if (errorPosta != null)
                {
                    throw new ReglaNegocioException(errorPosta);
                }

                // Regla 5: no quitarse a sí mismo el rol, ni dejar sin Administrador activo.
                bool pierdeAdministrador = actual.Rol == RolUsuario.Administrador && rol != RolUsuario.Administrador;
                if (pierdeAdministrador)
                {
                    if (EsUsuarioDeLaSesion(idUsuario))
                    {
                        throw new ReglaNegocioException("No puede quitarse a sí mismo el rol de Administrador.");
                    }
                    if (actual.Estado && dao.ContarAdministradoresActivos() <= 1)
                    {
                        throw new ReglaNegocioException("No se puede quitar el rol al último Administrador activo.");
                    }
                }

                if (!dao.ActualizarRolYPosta(idUsuario, rol, posta))
                {
                    // El UPDATE también protege al último Administrador (por si otro puesto cambió algo).
                    throw new ReglaNegocioException("No se pudo actualizar: debe quedar al menos un Administrador activo.");
                }

                new AuditoriaNegocio().Registrar("EDITAR_USUARIO", "Usuario", idUsuario,
                    "Rol " + rol + (posta.HasValue ? ", posta " + posta.Value : ", sin posta"));
                return new ResultadoUsuario { IdGenerado = idUsuario };
            });
        }

        /// <summary>Desactiva un usuario (no puede iniciar sesión). Nunca se elimina (regla 4).</summary>
        public ResultadoUsuario Desactivar(int idUsuario)
        {
            return Ejecutar("desactivar", () =>
            {
                UsuarioResumen actual = ObtenerExistente(idUsuario);
                if (!actual.Estado)
                {
                    throw new ReglaNegocioException("El usuario ya está inactivo.");
                }
                if (EsUsuarioDeLaSesion(idUsuario))
                {
                    throw new ReglaNegocioException("No puede desactivar su propio usuario.");
                }
                if (actual.Rol == RolUsuario.Administrador && dao.ContarAdministradoresActivos() <= 1)
                {
                    throw new ReglaNegocioException("No se puede desactivar al último Administrador activo.");
                }

                if (!dao.CambiarEstado(idUsuario, false))
                {
                    throw new ReglaNegocioException("No se pudo desactivar: debe quedar al menos un Administrador activo.");
                }

                new AuditoriaNegocio().Registrar("DESACTIVAR_USUARIO", "Usuario", idUsuario, "Usuario desactivado");
                return new ResultadoUsuario { IdGenerado = idUsuario };
            });
        }

        /// <summary>Vuelve a activar un usuario.</summary>
        public ResultadoUsuario Activar(int idUsuario)
        {
            return Ejecutar("activar", () =>
            {
                UsuarioResumen actual = ObtenerExistente(idUsuario);
                if (actual.Estado)
                {
                    throw new ReglaNegocioException("El usuario ya está activo.");
                }
                if (!dao.CambiarEstado(idUsuario, true))
                {
                    throw new ReglaNegocioException("No se encontró el usuario.");
                }

                new AuditoriaNegocio().Registrar("ACTIVAR_USUARIO", "Usuario", idUsuario, "Usuario activado");
                return new ResultadoUsuario { IdGenerado = idUsuario };
            });
        }

        /// <summary>
        /// Activa o desactiva según el estado ACTUAL guardado (así el formulario no
        /// decide cuál de las dos operaciones corresponde).
        /// </summary>
        public ResultadoUsuario AlternarEstado(int idUsuario)
        {
            return Ejecutar("cambiar estado", () =>
                ObtenerExistente(idUsuario).Estado ? Desactivar(idUsuario) : Activar(idUsuario));
        }

        /// <summary>Genera y guarda una contraseña temporal nueva; la devuelve UNA sola vez.</summary>
        public ResultadoUsuario RestablecerClave(int idUsuario)
        {
            return Ejecutar("restablecer clave", () =>
            {
                UsuarioResumen actual = ObtenerExistente(idUsuario);

                string temporal = GenerarClaveTemporal(actual.NombreUsuario);
                string salt = Hash.GenerarSalt();
                if (!dao.ActualizarClave(idUsuario, Hash.CalcularHash(temporal, salt), salt))
                {
                    throw new ReglaNegocioException("No se encontró el usuario.");
                }

                new AuditoriaNegocio().Registrar("RESTABLECER_CLAVE", "Usuario", idUsuario, "Contraseña temporal generada");
                return new ResultadoUsuario { IdGenerado = idUsuario, ClaveTemporal = temporal };
            });
        }

        // ===================== Cambio de la contraseña propia (cualquier rol) =====================

        /// <summary>
        /// El usuario de la sesión cambia su propia contraseña: verifica la actual con
        /// Hash.Verificar, aplica las reglas, exige que la nueva sea distinta y la guarda.
        /// </summary>
        public ResultadoOperacion CambiarMiClave(string claveActual, string claveNueva, string confirmacion)
        {
            if (!Sesion.HaySesion)
            {
                return ResultadoOperacion.Error("No hay una sesión iniciada.");
            }
            Usuario u = Sesion.UsuarioActual;

            // Primero las reglas que no necesitan la base (todas juntas).
            ResultadoOperacion r = new ResultadoOperacion();
            if (string.IsNullOrEmpty(claveActual))
            {
                r.AgregarError("Escriba su contraseña actual.");
            }
            if (!string.Equals(claveNueva, confirmacion, StringComparison.Ordinal))
            {
                r.AgregarError("La nueva contraseña y su confirmación no coinciden.");
            }
            r.Mensajes.AddRange(ValidarClave(claveNueva, u.NombreUsuario).Mensajes);
            if (!string.IsNullOrEmpty(claveActual) && string.Equals(claveActual, claveNueva, StringComparison.Ordinal))
            {
                r.AgregarError("La nueva contraseña debe ser distinta de la actual.");
            }
            if (!r.Exito)
            {
                return r;
            }

            try
            {
                // Verificación de la contraseña actual con el mismo mecanismo del inicio de sesión.
                string hash, salt;
                Usuario guardado = dao.ObtenerCredenciales(u.NombreUsuario, out hash, out salt);
                if (guardado == null || guardado.IdUsuario != u.IdUsuario || !Hash.Verificar(claveActual, salt, hash))
                {
                    return ResultadoOperacion.Error("La contraseña actual no es correcta.");
                }

                string nuevaSalt = Hash.GenerarSalt();
                if (!dao.ActualizarClave(u.IdUsuario, Hash.CalcularHash(claveNueva, nuevaSalt), nuevaSalt))
                {
                    return ResultadoOperacion.Error("No se encontró su usuario.");
                }

                new AuditoriaNegocio().Registrar("CAMBIAR_CLAVE", "Usuario", u.IdUsuario, "El usuario cambió su contraseña");
                return new ResultadoOperacion();
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Usuarios - cambiar mi clave", ex));
            }
        }

        // ===================== Apoyo =====================

        /// <summary>
        /// Envoltura común de la gestión: verifica que sea Administrador (regla 1) y
        /// convierte las excepciones en mensajes: las reglas de negocio se muestran
        /// tal cual; los errores técnicos, con un mensaje genérico y código.
        /// </summary>
        private static ResultadoUsuario Ejecutar(string operacion, Func<ResultadoUsuario> accion)
        {
            if (!EsAdministrador())
            {
                return ResultadoUsuario.Fallo("Solo el Administrador puede administrar usuarios.");
            }
            try
            {
                return accion();
            }
            catch (ReglaNegocioException ex)
            {
                return ResultadoUsuario.Fallo(ex.Message);
            }
            catch (RegistroDuplicadoException ex)
            {
                return ResultadoUsuario.Fallo(ex.Message);
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException || ex is ArgumentException)
            {
                return ResultadoUsuario.Fallo(RegistroTecnico.ErrorParaUsuario("Usuarios - " + operacion, ex));
            }
        }

        private UsuarioResumen ObtenerExistente(int idUsuario)
        {
            UsuarioResumen u = idUsuario > 0 ? dao.ObtenerPorId(idUsuario) : null;
            if (u == null)
            {
                throw new ReglaNegocioException("El usuario no existe.");
            }
            return u;
        }

        private static bool EsUsuarioDeLaSesion(int idUsuario)
        {
            return Sesion.HaySesion && Sesion.UsuarioActual.IdUsuario == idUsuario;
        }

        // Regla 3: Admision necesita una posta existente; el Administrador no lleva posta.
        private string ValidarRolYPosta(RolUsuario rol, int? idPosta, out int? postaFinal)
        {
            postaFinal = null;
            if (!Enum.IsDefined(typeof(RolUsuario), rol))
            {
                return "Seleccione un rol válido.";
            }
            if (!RequierePosta(rol))
            {
                return null;   // Administrador: sin posta
            }
            if (!idPosta.HasValue)
            {
                return "El rol Admisión requiere una posta.";
            }
            if (!catalogoDao.Postas().Any(p => p.IdPosta == idPosta.Value))
            {
                return "La posta seleccionada no existe.";
            }
            postaFinal = idPosta;
            return null;
        }

        /// <summary>
        /// Contraseña temporal de 10 caracteres con RNGCryptoServiceProvider (no con
        /// Random, que es predecible). Se descartan los bytes que causarían "sesgo de
        /// módulo" (que unos caracteres salgan más que otros), y se repite hasta que
        /// cumpla ValidarClave (al menos una letra y un número).
        /// </summary>
        private string GenerarClaveTemporal(string nombreUsuario)
        {
            int limite = 256 - (256 % Alfabeto.Length);   // Bytes por encima se descartan
            byte[] unByte = new byte[1];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                while (true)
                {
                    char[] clave = new char[LargoClaveTemporal];
                    int i = 0;
                    while (i < clave.Length)
                    {
                        rng.GetBytes(unByte);
                        if (unByte[0] >= limite)
                        {
                            continue;
                        }
                        clave[i++] = Alfabeto[unByte[0] % Alfabeto.Length];
                    }

                    string candidata = new string(clave);
                    if (ValidarClave(candidata, nombreUsuario).Exito)
                    {
                        return candidata;
                    }
                }
            }
        }
    }
}
