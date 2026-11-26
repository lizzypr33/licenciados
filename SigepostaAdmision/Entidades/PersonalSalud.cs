using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Clase ABSTRACTA para todo el personal de salud de la posta.
    ///
    /// ¿Por qué herencia (PersonalSalud : Persona)? El personal de salud también
    /// es una persona, así que hereda sus datos y agrega colegiatura y posta.
    ///
    /// ¿Por qué abstracta? Porque no existe un "personal de salud genérico":
    /// siempre es un Médico o un Enfermero. Además, cada tipo atiende de forma
    /// distinta, por eso el método Atender() es ABSTRACTO: aquí solo se declara
    /// (no tiene cuerpo) y cada clase hija está OBLIGADA a implementarlo.
    /// Es una jerarquía de 3 niveles: Persona -> PersonalSalud -> Medico/Enfermero.
    /// </summary>
    public abstract class PersonalSalud : Persona
    {
        // ----- Campos privados -----
        private string codigoColegiatura;
        private string postaAsignada;

        // ----- Constructores (protected: solo para las clases hijas) -----

        protected PersonalSalud() : base()
        {
        }

        protected PersonalSalud(string dni, string nombres, string apellidos,
                                DateTime fechaNacimiento, string telefono,
                                string codigoColegiatura, string postaAsignada)
            : base(dni, nombres, apellidos, fechaNacimiento, telefono)
        {
            CodigoColegiatura = codigoColegiatura;
            PostaAsignada = postaAsignada;
        }

        // ----- Propiedades -----

        /// <summary>Código del colegio profesional (CMP para médicos, CEP para enfermeros).</summary>
        public string CodigoColegiatura
        {
            get { return codigoColegiatura; }
            set { codigoColegiatura = Validacion.TextoObligatorio(value, "Código de colegiatura"); }
        }

        public string PostaAsignada
        {
            get { return postaAsignada; }
            set { postaAsignada = Validacion.TextoObligatorio(value, "Posta asignada"); }
        }

        // ----- Métodos -----

        /// <summary>
        /// Método ABSTRACTO: cada tipo de personal lo implementa a su manera.
        /// Gracias al POLIMORFISMO, si tenemos una List&lt;PersonalSalud&gt; y
        /// llamamos a Atender(), se ejecuta la versión del Médico o del
        /// Enfermero según el objeto real, sin usar if ni switch.
        /// </summary>
        public abstract string Atender(Paciente p);

        /// <summary>Agrega a los datos de la persona la colegiatura y la posta.</summary>
        public override string MostrarDatos()
        {
            return base.MostrarDatos() +
                   " | Colegiatura: " + codigoColegiatura +
                   " | Posta: " + postaAsignada;
        }
    }
}
