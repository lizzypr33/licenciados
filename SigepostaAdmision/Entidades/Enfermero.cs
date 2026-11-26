using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Enfermero(a) de la posta.
    ///
    /// ¿Por qué herencia (Enfermero : PersonalSalud)? Igual que el Médico,
    /// "ES UN" personal de salud; agrega solo su AreaAsignada (triaje,
    /// vacunación, tópico, etc.) e implementa su propia forma de Atender().
    /// </summary>
    public class Enfermero : PersonalSalud
    {
        private string areaAsignada;

        public Enfermero() : base()
        {
        }

        public Enfermero(string dni, string nombres, string apellidos,
                         DateTime fechaNacimiento, string telefono,
                         string codigoColegiatura, string postaAsignada, string areaAsignada)
            : base(dni, nombres, apellidos, fechaNacimiento, telefono, codigoColegiatura, postaAsignada)
        {
            AreaAsignada = areaAsignada;
        }

        public string AreaAsignada
        {
            get { return areaAsignada; }
            set { areaAsignada = Validacion.TextoObligatorio(value, "Área asignada"); }
        }

        /// <summary>
        /// POLIMORFISMO: implementación del enfermero, distinta a la del Médico.
        /// </summary>
        public override string Atender(Paciente p)
        {
            Validacion.ObjetoObligatorio(p, "Paciente");
            return "El/la enfermero(a) " + Nombres + " " + Apellidos + " (área: " + areaAsignada + ")" +
                   " toma los signos vitales y realiza el triaje del paciente " +
                   p.Nombres + " " + p.Apellidos + ".";
        }

        public override string MostrarDatos()
        {
            return "[Enfermero] " + base.MostrarDatos() + " | Área: " + areaAsignada;
        }
    }
}
