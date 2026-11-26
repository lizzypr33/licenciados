namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Elemento de catálogo (solo lectura): un consultorio con su posta y servicio.
    /// ToString() devuelve el nombre para mostrarlo en un ComboBox.
    /// </summary>
    public class ConsultorioItem
    {
        public int IdConsultorio { get; set; }
        public string Nombre { get; set; }
        public int IdPosta { get; set; }
        public int IdServicio { get; set; }
        public string Servicio { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
