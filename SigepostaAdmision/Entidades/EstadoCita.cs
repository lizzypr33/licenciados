namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Estados posibles de una cita. Se usa un enum (y no un string) para que
    /// solo existan valores válidos y el compilador detecte errores de escritura.
    /// </summary>
    public enum EstadoCita
    {
        Pendiente,
        EnCola,
        Atendida,
        Cancelada
    }
}
