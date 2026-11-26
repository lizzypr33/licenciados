namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Elemento de catálogo (solo lectura): un servicio de salud, por ejemplo
    /// "Medicina General". ToString() devuelve el nombre para que se muestre
    /// directamente en un ComboBox.
    /// </summary>
    public class ServicioItem
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
