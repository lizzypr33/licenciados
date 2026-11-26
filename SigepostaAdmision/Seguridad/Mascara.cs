namespace SigepostaAdmision.Seguridad
{
    /// <summary>
    /// Enmascarado de datos personales en pantalla (principio de "mínima exposición"):
    /// en las listas basta con reconocer al paciente, no hace falta ver su DNI
    /// completo. Así se reduce lo que puede verse por encima del hombro o en una
    /// captura de pantalla. El DNI completo solo se muestra donde se necesita
    /// (el panel de detalle de Pacientes).
    /// </summary>
    public static class Mascara
    {
        private const char Punto = '•';

        /// <summary>"45123478" → "45••••78" (2 primeros y 2 últimos dígitos).</summary>
        public static string Dni(string dni)
        {
            if (string.IsNullOrEmpty(dni))
            {
                return string.Empty;
            }
            string valor = dni.Trim();
            if (valor.Length <= 4)
            {
                return new string(Punto, valor.Length);
            }
            return valor.Substring(0, 2) + new string(Punto, valor.Length - 4) + valor.Substring(valor.Length - 2);
        }
    }
}
