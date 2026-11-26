using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Médico de la posta.
    ///
    /// ¿Por qué herencia (Medico : PersonalSalud)? Un médico "ES UN" personal
    /// de salud: hereda todo (datos de Persona + colegiatura y posta) y solo
    /// agrega su Especialidad. Al heredar de una clase abstracta está obligado
    /// a implementar Atender().
    /// </summary>
    public class Medico : PersonalSalud
    {
        private string especialidad;

        public Medico() : base()
        {
        }

        public Medico(string dni, string nombres, string apellidos,
                      DateTime fechaNacimiento, string telefono,
                      string codigoColegiatura, string postaAsignada, string especialidad)
            : base(dni, nombres, apellidos, fechaNacimiento, telefono, codigoColegiatura, postaAsignada)
        {
            Especialidad = especialidad;
        }

        public string Especialidad
        {
            get { return especialidad; }
            set { especialidad = Validacion.TextoObligatorio(value, "Especialidad"); }
        }

        /// <summary>
        /// POLIMORFISMO: implementación del médico. El mismo mensaje Atender()
        /// produce un resultado distinto al del Enfermero.
        /// </summary>
        public override string Atender(Paciente p)
        {
            Validacion.ObjetoObligatorio(p, "Paciente");
            return "El Dr(a). " + Nombres + " " + Apellidos + " (" + especialidad + ")" +
                   " realiza la consulta médica, evalúa y diagnostica al paciente " +
                   p.Nombres + " " + p.Apellidos + ".";
        }

        public override string MostrarDatos()
        {
            return "[Médico] " + base.MostrarDatos() + " | Especialidad: " + especialidad;
        }
    }
}
