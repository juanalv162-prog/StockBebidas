using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using StockBebidas.Logica;

namespace StockBebidas.UI
{
    public partial class Form1 : Form
    {
        private GestorStock gestor;

        private TextBox txtNombre;
        private TextBox txtCantidad;
        private TextBox txtPrecio;
        private DataGridView tabla;
        private Label lblTotal;

        public Form1()
        {
            InitializeComponent();

            gestor = new GestorStock();

            Text = "Sistema de Stock de Bebidas";
            Size = new Size(900, 550);
            StartPosition = FormStartPosition.CenterScreen;

            CrearInterfaz();
            ActualizarTabla();
        }

        private void CrearInterfaz()
        {
            Label titulo = new Label();
            titulo.Text = "STOCK DE BEBIDAS";
            titulo.Font = new Font("Arial", 18, FontStyle.Bold);
            titulo.Location = new Point(20, 15);
            titulo.Size = new Size(400, 35);
            Controls.Add(titulo);

            CrearEtiqueta("Nombre:", 20, 70);
            txtNombre = CrearCampo(110, 68, 180);

            CrearEtiqueta("Cantidad:", 310, 70);
            txtCantidad = CrearCampo(390, 68, 100);

            CrearEtiqueta("Precio:", 500, 70);
            txtPrecio = CrearCampo(560, 68, 220);

            Button btnAgregar = new Button();
            btnAgregar.Text = "Agregar bebida";
            btnAgregar.Location = new Point(20, 115);
            btnAgregar.Size = new Size(150, 35);
            btnAgregar.Click += BtnAgregar_Click;
            Controls.Add(btnAgregar);

            Button btnActualizar = new Button();
            btnActualizar.Text = "Actualizar stock";
            btnActualizar.Location = new Point(185, 115);
            btnActualizar.Size = new Size(150, 35);
            btnActualizar.Click += BtnActualizar_Click;
            Controls.Add(btnActualizar);

            Button btnEliminar = new Button();
            btnEliminar.Text = "Eliminar bebida";
            btnEliminar.Location = new Point(350, 115);
            btnEliminar.Size = new Size(150, 35);
            btnEliminar.Click += BtnEliminar_Click;
            Controls.Add(btnEliminar);

            tabla = new DataGridView();
            tabla.Location = new Point(20, 170);
            tabla.Size = new Size(740, 280);
            tabla.ReadOnly = true;
            tabla.AllowUserToAddRows = false;
            tabla.AllowUserToDeleteRows = false;
            tabla.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            tabla.MultiSelect = false;
            tabla.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(tabla);

            lblTotal = new Label();
            lblTotal.Location = new Point(20, 465);
            lblTotal.Size = new Size(740, 30);
            lblTotal.Font = new Font("Arial", 11, FontStyle.Bold);
            Controls.Add(lblTotal);
        }

        private void CrearEtiqueta(string texto, int x, int y)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.Location = new Point(x, y);
            etiqueta.Size = new Size(90, 25);
            Controls.Add(etiqueta);
        }

        private TextBox CrearCampo(int x, int y, int ancho)
        {
            TextBox campo = new TextBox();
            campo.Location = new Point(x, y);
            campo.Size = new Size(ancho, 35);
            campo.AutoSize = false;
            campo.Multiline = true;
            campo.WordWrap = false;
            campo.ScrollBars = ScrollBars.None;
            campo.TextAlign = HorizontalAlignment.Center;
            campo.ForeColor = Color.Black;
            campo.BackColor = Color.White;
            campo.Font = new Font("Arial", 12);
            campo.Controls.Clear();
            Controls.Add(campo);
            return campo;
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            int cantidad;
            decimal precio;

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingresá el nombre de la bebida.");
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out cantidad)
                || cantidad < 0)
            {
                MessageBox.Show("Ingresá una cantidad válida.");
                return;
            }

            if (!decimal.TryParse(
                txtPrecio.Text,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out precio) || precio < 0)
            {
                MessageBox.Show("Ingresá un precio válido.");
                return;
            }

            try
            {
                Bebida bebida = new Bebida(
                    txtNombre.Text,
                    cantidad,
                    precio);

                gestor.Agregar(bebida);

                ActualizarTabla();

                txtNombre.Clear();
                txtCantidad.Clear();
                txtPrecio.Clear();

                MessageBox.Show("Bebida agregada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            if (tabla.CurrentRow == null)
            {
                MessageBox.Show("Seleccioná una bebida.");
                return;
            }

            int cantidad;

            if (!int.TryParse(txtCantidad.Text, out cantidad)
                || cantidad < 0)
            {
                MessageBox.Show("Ingresá una cantidad válida.");
                return;
            }

            try
            {
                gestor.ActualizarStock(
                    tabla.CurrentRow.Index,
                    cantidad);

                ActualizarTabla();

                MessageBox.Show("Stock actualizado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (tabla.CurrentRow == null)
            {
                MessageBox.Show("Seleccioná una bebida.");
                return;
            }

            int indice = tabla.CurrentRow.Index;

            DialogResult respuesta = MessageBox.Show(
                "¿Deseás eliminar esta bebida?",
                "Confirmar",
                MessageBoxButtons.YesNo);

            if (respuesta == DialogResult.Yes)
            {
                gestor.Bebidas.RemoveAt(indice);
                gestor.Guardar();
                ActualizarTabla();

                MessageBox.Show("Bebida eliminada.");
            }
        }

        private void ActualizarTabla()
        {
            tabla.DataSource = null;

            tabla.DataSource = gestor.Bebidas.ConvertAll(
                b => new
                {
                    Nombre = b.Nombre,
                    Cantidad = b.Cantidad,
                    Precio = b.Precio
                });

            decimal total = 0;

            foreach (Bebida bebida in gestor.Bebidas)
            {
                total += bebida.Cantidad * bebida.Precio;
            }

            lblTotal.Text =
                "Tipos de bebidas: " + gestor.Bebidas.Count +
                " | Valor total del stock: " +
                total.ToString("C2");
        }
    }
}
