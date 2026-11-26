using System;
using System.Collections.Generic;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Establecimiento de salud (posta) del primer nivel de atención.
    ///
    /// No usa herencia: AGRUPA a su personal y a sus pacientes mediante listas
    /// (agregación: la posta "tiene" personal y pacientes).
    ///
    /// La ListaPersonal es de tipo List&lt;PersonalSalud&gt; (la clase abstracta):
    /// gracias a la herencia y al POLIMORFISMO, en una misma lista caben
    /// médicos y enfermeros, y al llamar a Atender() cada uno responde a su manera.
    /// </summary>
    public class Posta
    {
        // Categorías válidas del primer nivel de atención (MINSA).
        private static readonly string[] CategoriasValidas = { "I-1", "I-2", "I-3" };

        // ----- Campos privados -----
        private int idPosta;
        private string nombre;
        private string distrito;
        private string categoria;
        private readonly List<PersonalSalud> listaPersonal;
        private readonly List<Paciente> listaPacientes;

        // ----- Constructores -----

        public Posta()
        {
            listaPersonal = new List<PersonalSalud>();
            listaPacientes = new List<Paciente>();
        }

        public Posta(int idPosta, string nombre, string distrito, string categoria)
            : this()
        {
            IdPosta = idPosta;
            Nombre = nombre;
            Distrito = distrito;
            Categoria = categoria;
        }

        // ----- Propiedades -----

        public int IdPosta
        {
            get { return idPosta; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("El Id de la posta no puede ser negativo.");
                }
                idPosta = value;
            }
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = Validacion.TextoObligatorio(value, "Nombre de la posta"); }
        }

        public string Distrito
        {
            get { return distrito; }
            set { distrito = Validacion.TextoObligatorio(value, "Distrito"); }
        }

        /// <summary>Solo se aceptan las categorías I-1, I-2 o I-3.</summary>
        public string Categoria
        {
            get { return categoria; }
            set
            {
                string valor = Validacion.TextoObligatorio(value, "Categoría").ToUpperInvariant();
                if (Array.IndexOf(CategoriasValidas, valor) < 0)
                {
                    throw new ArgumentException("La categoría de la posta debe ser I-1, I-2 o I-3.");
                }
                categoria = valor;
            }
        }

        /// <summary>Personal de salud (médicos y enfermeros) de la posta.</summary>
        public List<PersonalSalud> ListaPersonal
        {
            get { return listaPersonal; }
        }

        /// <summary>Pacientes adscritos a la posta.</summary>
        public List<Paciente> ListaPacientes
        {
            get { return listaPacientes; }
        }
    }
}
