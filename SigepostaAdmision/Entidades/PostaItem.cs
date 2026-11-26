namespace SigepostaAdmision.Entidades
{
    /// <summary>Elemento de catálogo (solo lectura): una posta. ToString() devuelve el nombre para el ComboBox.</summary>
    public class PostaItem
    {
        public int IdPosta { get; set; }
        public string Nombre { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
