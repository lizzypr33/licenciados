namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Elemento de catálogo (solo lectura): un miembro del personal de salud.
    /// ToString() muestra "Apellidos, Nombres (Especialidad)" en el ComboBox.
    /// </summary>
    public class ProfesionalItem
    {
        public int IdPersonal { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        /// <summary>Medico, Obstetra, Enfermero, Tecnico o Administrativo.</summary>
        public string Tipo { get; set; }
        public string Especialidad { get; set; }
        public int IdPosta { get; set; }

        public string NombreCompleto
        {
            get { return Apellidos + ", " + Nombres; }
        }

        public override string ToString()
        {
            string detalle = string.IsNullOrEmpty(Especialidad) ? Tipo : Especialidad;
            return NombreCompleto + " (" + detalle + ")";
        }
    }
}
