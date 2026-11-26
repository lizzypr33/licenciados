namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Estados de una historia clínica. Los nombres coinciden con los valores
    /// permitidos en la columna Estado de la tabla HistoriaClinica.
    /// No existe "Eliminada": las historias clínicas nunca se borran.
    /// </summary>
    public enum EstadoHistoria
    {
        Activa,
        Inactiva
    }
}
