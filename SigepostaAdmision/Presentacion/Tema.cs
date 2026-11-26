using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Tema visual de toda la aplicación (SOLO colores).
    ///
    /// Cada pantalla llama a Tema.Aplicar(this) una vez, al final de su constructor.
    /// Aplicar recorre los controles de forma RECURSIVA y pinta cada uno según su
    /// tipo. Nunca toca posiciones, tamaños, fuentes, textos, visibilidad ni
    /// eventos de la pantalla: solo BackColor/ForeColor, los colores de grillas y
    /// gráficos y, en botones, FlatStyle/FlatAppearance (sin "Flat" Windows dibuja
    /// el botón con el estilo del sistema e ignora el color de fondo).
    ///
    /// La paleta son campos "static readonly" porque C# no admite "const" para
    /// el tipo Color (es una estructura, no un tipo primitivo).
    ///
    /// Colores con significado: un color que el diseñador puso a propósito
    /// (avisos ámbar, verde de "copiado", tarjetas, etc.) se respeta; solo se
    /// convierten los colores neutros conocidos a su equivalente del tema. Los
    /// colores de fila que los formularios asignan DESPUÉS por código (citas
    /// canceladas, usuarios inactivos, pacientes llamados) tienen prioridad sobre
    /// los estilos de la grilla, porque DataGridViewRow.DefaultCellStyle y
    /// Cell.Style se aplican por encima de DefaultCellStyle y de las filas alternas.
    /// </summary>
    public static class Tema
    {
        // ===================== Paleta =====================

        // Fondos
        public static readonly Color FondoVentana = Hex(0xF5F7FA);
        public static readonly Color FondoContenido = Hex(0xFFFFFF);
        public static readonly Color FondoSuave = Hex(0xEEF2F7);        // Menú, barra de estado, tarjetas
        public static readonly Color Linea = Hex(0xE5E7EB);             // Separadores

        // Menú lateral
        public static readonly Color MenuFondo = Hex(0xEEF2F7);
        public static readonly Color MenuTexto = Hex(0x374151);
        public static readonly Color MenuActivoFondo = Hex(0xDCE8FA);
        public static readonly Color MenuActivoTexto = Hex(0x1E40AF);
        public static readonly Color MenuHover = Hex(0xE4EBF5);

        // Textos
        public static readonly Color TextoPrincipal = Hex(0x1F2937);
        public static readonly Color TextoSecundario = Hex(0x6B7280);
        // #6B7280 sobre #EEF2F7 da 4,3:1 (menos del mínimo 4,5:1 para texto normal);
        // sobre esos fondos suaves se usa esta variante más oscura (6,7:1).
        public static readonly Color TextoSecundarioOscuro = Hex(0x4B5563);
        public static readonly Color TituloSeccion = Hex(0x1E40AF);
        public static readonly Color TextoError = Hex(0xB91C1C);
        public static readonly Color TextoExito = Hex(0x15803D);        // Paciente llamado (cola)
        public static readonly Color TextoInactivo = Hex(0x9CA3AF);     // Canceladas, inactivos (distinción en gris)
        public static readonly Color Enlace = Hex(0x1E40AF);
        public static readonly Color EnlaceActivo = Hex(0x1E3A8A);

        // Botón primario
        public static readonly Color PrimarioFondo = Hex(0x2563EB);
        public static readonly Color PrimarioHover = Hex(0x1D4ED8);
        public static readonly Color PrimarioPresionado = Hex(0x1E40AF);
        public static readonly Color PrimarioTexto = Color.White;

        // Botón secundario
        public static readonly Color SecundarioFondo = Hex(0xFFFFFF);
        public static readonly Color SecundarioBorde = Hex(0xCBD5E1);
        public static readonly Color SecundarioTexto = Hex(0x1F2937);
        public static readonly Color SecundarioHover = Hex(0xF1F5F9);
        public static readonly Color SecundarioPresionado = Hex(0xE2E8F0);

        // Botón destructivo
        public static readonly Color DestructivoFondo = Hex(0xFFFFFF);
        public static readonly Color DestructivoBorde = Hex(0xFCA5A5);
        public static readonly Color DestructivoTexto = Hex(0xB91C1C);
        public static readonly Color DestructivoHover = Hex(0xFEF2F2);
        public static readonly Color DestructivoPresionado = Hex(0xFEE2E2);

        // Botón deshabilitado
        public static readonly Color DeshabilitadoFondo = Hex(0xF3F4F6);
        public static readonly Color DeshabilitadoTexto = Hex(0x9CA3AF);
        public static readonly Color DeshabilitadoBorde = Hex(0xE5E7EB);

        // Grillas
        public static readonly Color GrillaEncabezadoFondo = Hex(0xE8EEF7);
        public static readonly Color GrillaEncabezadoTexto = Hex(0x1E3A8A);
        public static readonly Color GrillaFilaAlterna = Hex(0xF8FAFC);
        public static readonly Color GrillaLinea = Hex(0xE5E7EB);
        public static readonly Color GrillaSeleccionFondo = Hex(0xDBEAFE);
        public static readonly Color GrillaSeleccionTexto = Hex(0x1E3A8A);

        // Barra de estado (texto oscurecido por contraste, ver TextoSecundarioOscuro)
        public static readonly Color EstadoFondo = Hex(0xEEF2F7);
        public static readonly Color EstadoTexto = TextoSecundarioOscuro;

        // Gráficos
        public static readonly Color GraficoSerie = Hex(0x3B82F6);
        public static readonly Color GraficoSerieAlterna = Hex(0x64748B);
        public static readonly Color GraficoTexto = Hex(0x374151);
        public static readonly Color GraficoLinea = Hex(0xE5E7EB);

        private const double ContrasteMinimo = 4.5;   // WCAG AA para texto normal

        // Textos de los botones primarios y destructivos (sin "..." ni "&").
        private static readonly HashSet<string> TextosPrimarios = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Ingresar", "Guardar", "Confirmar", "Generar", "Registrar llegada", "Registrar atención", "Buscar paciente"
        };
        private static readonly HashSet<string> TextosDestructivos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Cancelar cita", "Desactivar", "Importar claves", "Demo rollback", "Deshacer llegada"
        };

        // Pantallas ya tematizadas (para aplicar una sola vez) y botones ya enganchados.
        // ConditionalWeakTable no impide que el recolector libere las pantallas cerradas.
        private static readonly ConditionalWeakTable<Control, object> Aplicados = new ConditionalWeakTable<Control, object>();
        private static readonly ConditionalWeakTable<Button, object> BotonesEnganchados = new ConditionalWeakTable<Button, object>();

        // ===================== Punto de entrada =====================

        /// <summary>
        /// Aplica el tema a la pantalla y a todos sus controles (una sola vez por pantalla).
        /// </summary>
        public static void Aplicar(Control raiz)
        {
            if (raiz == null)
            {
                return;
            }
            object marca;
            if (Aplicados.TryGetValue(raiz, out marca))
            {
                return;
            }
            Aplicados.Add(raiz, new object());

            if (raiz is Form)
            {
                raiz.BackColor = FondoVentana;
            }
            foreach (Control hijo in raiz.Controls)
            {
                AplicarControl(hijo);
            }
        }

        // ===================== Por tipo de control =====================

        private static void AplicarControl(Control c)
        {
            // El orden importa: LinkLabel hereda de Label.
            if (c is LinkLabel)
            {
                LinkLabel enlace = (LinkLabel)c;
                enlace.LinkColor = Enlace;
                enlace.VisitedLinkColor = Enlace;
                enlace.ActiveLinkColor = EnlaceActivo;
            }
            else if (c is Label || c is CheckBox || c is RadioButton)
            {
                PintarTexto(c);
            }
            else if (c is Button)
            {
                PrepararBoton((Button)c);
                return;
            }
            else if (c is DataGridView)
            {
                PintarGrilla((DataGridView)c);
                return;   // Sus controles internos (edición) los maneja la grilla
            }
            else if (c is Chart)
            {
                PintarGrafico((Chart)c);
                return;
            }
            else if (c is StatusStrip)
            {
                PintarBarraEstado((StatusStrip)c);
                return;
            }
            else if (c is TextBoxBase || c is ComboBox || c is ListBox || c is DateTimePicker || c is UpDownBase)
            {
                // Controles de entrada: fondo del sistema (blanco); texto principal si no
                // tenía un color propio. Se fija explícito para que no herede el azul de
                // un GroupBox.
                if (!EsExplicito(c, "ForeColor"))
                {
                    c.ForeColor = TextoPrincipal;
                }
                return;
            }
            else if (c is GroupBox)
            {
                PintarContenedor(c);
                c.ForeColor = TituloSeccion;   // Título del grupo (los hijos tienen color propio)
            }
            else if (c is Panel || c is SplitContainer)
            {
                // Panel incluye FlowLayoutPanel, TableLayoutPanel y SplitterPanel.
                PintarContenedor(c);
            }

            foreach (Control hijo in c.Controls)
            {
                AplicarControl(hijo);
            }
        }

        /// <summary>
        /// Contenedores: solo se convierte un color de fondo PUESTO en el diseñador
        /// (blanco o casi blanco → contenido; gris claro → fondo suave; separador → línea).
        /// Si el contenedor no tiene color propio, hereda el de la ventana.
        /// Cualquier otro color (con significado) se respeta.
        /// </summary>
        private static void PintarContenedor(Control c)
        {
            if (!EsExplicito(c, "BackColor"))
            {
                return;
            }
            int argb = c.BackColor.ToArgb();
            if (argb == Color.White.ToArgb() || argb == Rgb(249, 250, 251))
            {
                c.BackColor = FondoContenido;
            }
            else if (argb == Rgb(243, 244, 246))
            {
                c.BackColor = FondoSuave;
            }
            else if (argb == Rgb(229, 231, 235))
            {
                c.BackColor = Linea;
            }
        }

        /// <summary>
        /// Etiquetas, casillas y opciones: el color heredado pasa a texto principal y
        /// los neutros conocidos a su equivalente del tema; los colores con significado
        /// (títulos azules, ámbar, verde, valores de tarjetas) se respetan. Al final se
        /// comprueba el contraste con el fondo real.
        /// </summary>
        private static void PintarTexto(Control c)
        {
            Color color;
            if (!EsExplicito(c, "ForeColor"))
            {
                color = TextoPrincipal;
            }
            else
            {
                int argb = c.ForeColor.ToArgb();
                if (argb == Rgb(55, 65, 81) || argb == Color.Black.ToArgb() || argb == SystemColors.ControlText.ToArgb())
                {
                    color = TextoPrincipal;
                }
                else if (argb == Rgb(75, 85, 99))
                {
                    color = TextoSecundario;
                }
                else if (argb == Color.Firebrick.ToArgb() || argb == Rgb(185, 28, 28))
                {
                    color = TextoError;
                }
                else
                {
                    color = c.ForeColor;   // Color con significado: se mantiene
                }
            }

            // Texto secundario sobre fondos suaves: variante más oscura para cumplir 4,5:1.
            if (color.ToArgb() == TextoSecundario.ToArgb() && Contraste(color, FondoEfectivo(c)) < ContrasteMinimo)
            {
                color = TextoSecundarioOscuro;
            }
            c.ForeColor = color;
        }

        // ===================== Botones =====================

        private enum TipoBoton { Primario, Secundario, Destructivo }

        /// <summary>
        /// Los botones planos SIN borde (así están diseñados los ítems del menú lateral)
        /// se pintan como ítems de menú; el resto según su tipo. Como el texto
        /// ("Activar"/"Desactivar") y Enabled cambian durante el uso, el botón se vuelve
        /// a pintar cuando cambian (solo colores; no altera lo que hace el botón).
        /// </summary>
        private static void PrepararBoton(Button b)
        {
            if (b.FlatStyle == FlatStyle.Flat && b.FlatAppearance.BorderSize == 0)
            {
                PintarItemMenu(b);
                return;
            }

            PintarBoton(b);
            object marca;
            if (!BotonesEnganchados.TryGetValue(b, out marca))
            {
                BotonesEnganchados.Add(b, new object());
                b.EnabledChanged += (s, e) => PintarBoton(b);
                b.TextChanged += (s, e) => PintarBoton(b);
            }
        }

        private static void PintarItemMenu(Button b)
        {
            bool esSalida = Normalizar(b.Text) == "Cerrar sesión";
            b.BackColor = MenuFondo;
            b.ForeColor = esSalida ? TextoError : MenuTexto;
            b.FlatAppearance.MouseOverBackColor = esSalida ? DestructivoHover : MenuHover;
            b.FlatAppearance.MouseDownBackColor = esSalida ? DestructivoPresionado : MenuActivoFondo;
        }

        private static void PintarBoton(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;

            if (!b.Enabled)
            {
                // Inactivo: gris claro, para que se note que no se puede usar.
                b.BackColor = DeshabilitadoFondo;
                b.ForeColor = DeshabilitadoTexto;
                b.FlatAppearance.BorderColor = DeshabilitadoBorde;
                b.FlatAppearance.MouseOverBackColor = DeshabilitadoFondo;
                b.FlatAppearance.MouseDownBackColor = DeshabilitadoFondo;
                return;
            }

            switch (Clasificar(b))
            {
                case TipoBoton.Primario:
                    b.BackColor = PrimarioFondo;
                    b.ForeColor = PrimarioTexto;
                    b.FlatAppearance.BorderColor = PrimarioFondo;
                    b.FlatAppearance.MouseOverBackColor = PrimarioHover;
                    b.FlatAppearance.MouseDownBackColor = PrimarioPresionado;
                    break;
                case TipoBoton.Destructivo:
                    b.BackColor = DestructivoFondo;
                    b.ForeColor = DestructivoTexto;
                    b.FlatAppearance.BorderColor = DestructivoBorde;
                    b.FlatAppearance.MouseOverBackColor = DestructivoHover;
                    b.FlatAppearance.MouseDownBackColor = DestructivoPresionado;
                    break;
                default:
                    b.BackColor = SecundarioFondo;
                    b.ForeColor = SecundarioTexto;
                    b.FlatAppearance.BorderColor = SecundarioBorde;
                    b.FlatAppearance.MouseOverBackColor = SecundarioHover;
                    b.FlatAppearance.MouseDownBackColor = SecundarioPresionado;
                    break;
            }
        }

        private static TipoBoton Clasificar(Button b)
        {
            string texto = Normalizar(b.Text);
            if (TextosPrimarios.Contains(texto))
            {
                return TipoBoton.Primario;
            }
            if (TextosDestructivos.Contains(texto))
            {
                return TipoBoton.Destructivo;
            }
            return TipoBoton.Secundario;
        }

        // "Importar claves..." → "Importar claves"; "&Guardar" → "Guardar".
        private static string Normalizar(string texto)
        {
            return (texto ?? string.Empty).Replace("&", string.Empty).Trim().TrimEnd('.').Trim();
        }

        // ===================== Grillas, gráficos y barra de estado =====================

        private static void PintarGrilla(DataGridView g)
        {
            g.EnableHeadersVisualStyles = false;   // Si no, Windows ignora los colores del encabezado
            g.BackgroundColor = FondoContenido;
            g.GridColor = GrillaLinea;

            // Se modifican los estilos existentes (así se conservan la fuente y la alineación).
            DataGridViewCellStyle encabezado = g.ColumnHeadersDefaultCellStyle;
            encabezado.BackColor = GrillaEncabezadoFondo;
            encabezado.ForeColor = GrillaEncabezadoTexto;
            encabezado.SelectionBackColor = GrillaEncabezadoFondo;   // El encabezado no "se selecciona"
            encabezado.SelectionForeColor = GrillaEncabezadoTexto;

            DataGridViewCellStyle filas = g.DefaultCellStyle;
            filas.BackColor = FondoContenido;
            filas.ForeColor = TextoPrincipal;
            filas.SelectionBackColor = GrillaSeleccionFondo;
            filas.SelectionForeColor = GrillaSeleccionTexto;

            // Solo el fondo: el color de texto de cada fila lo sigue decidiendo el formulario.
            g.AlternatingRowsDefaultCellStyle.BackColor = GrillaFilaAlterna;

            DataGridViewCellStyle cabeceraFila = g.RowHeadersDefaultCellStyle;
            cabeceraFila.BackColor = GrillaEncabezadoFondo;
            cabeceraFila.ForeColor = GrillaEncabezadoTexto;
            cabeceraFila.SelectionBackColor = GrillaSeleccionFondo;
            cabeceraFila.SelectionForeColor = GrillaSeleccionTexto;
        }

        private static void PintarGrafico(Chart grafico)
        {
            grafico.BackColor = FondoContenido;
            foreach (ChartArea area in grafico.ChartAreas)
            {
                area.BackColor = FondoContenido;
                foreach (Axis eje in area.Axes)
                {
                    eje.LabelStyle.ForeColor = GraficoTexto;
                    eje.TitleForeColor = GraficoTexto;
                    eje.LineColor = SecundarioBorde;
                    eje.MajorGrid.LineColor = GraficoLinea;
                    eje.MajorTickMark.LineColor = SecundarioBorde;
                }
            }
            int i = 0;
            foreach (Series serie in grafico.Series)
            {
                serie.Color = i++ % 2 == 0 ? GraficoSerie : GraficoSerieAlterna;
                serie.LabelForeColor = GraficoTexto;
            }
            foreach (Title titulo in grafico.Titles)
            {
                titulo.ForeColor = GraficoTexto;
            }
            foreach (Legend leyenda in grafico.Legends)
            {
                leyenda.BackColor = FondoContenido;
                leyenda.ForeColor = GraficoTexto;
            }
        }

        private static void PintarBarraEstado(StatusStrip barra)
        {
            barra.BackColor = EstadoFondo;
            barra.ForeColor = EstadoTexto;
            foreach (ToolStripItem item in barra.Items)
            {
                item.ForeColor = EstadoTexto;
            }
        }

        // ===================== Apoyo =====================

        /// <summary>
        /// ¿El color fue puesto en el diseñador (o por código) en ESTE control, o lo
        /// hereda de su contenedor? Usa la API pública de serialización del diseñador.
        /// </summary>
        private static bool EsExplicito(Control c, string propiedad)
        {
            PropertyDescriptor p = TypeDescriptor.GetProperties(c)[propiedad];
            return p != null && p.ShouldSerializeValue(c);
        }

        // Fondo sobre el que realmente se ve el control (salta los transparentes).
        private static Color FondoEfectivo(Control c)
        {
            Control actual = c;
            while (actual != null)
            {
                if (actual.BackColor.A == 255)
                {
                    return actual.BackColor;
                }
                actual = actual.Parent;
            }
            return FondoVentana;
        }

        /// <summary>Relación de contraste WCAG 2 entre dos colores (1 a 21).</summary>
        public static double Contraste(Color a, Color b)
        {
            double la = Luminancia(a);
            double lb = Luminancia(b);
            return (Math.Max(la, lb) + 0.05) / (Math.Min(la, lb) + 0.05);
        }

        private static double Luminancia(Color c)
        {
            return 0.2126 * Canal(c.R) + 0.7152 * Canal(c.G) + 0.0722 * Canal(c.B);
        }

        private static double Canal(byte valor)
        {
            double v = valor / 255.0;
            return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
        }

        private static Color Hex(int rgb)
        {
            return Color.FromArgb(255, (rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
        }

        private static int Rgb(int r, int g, int b)
        {
            return Color.FromArgb(255, r, g, b).ToArgb();
        }
    }
}
