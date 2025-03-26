using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace panel_control
{
    public partial class Form1 : Form
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private const string ServerIP = "192.168.101.2"; // IP del ControlServer
        private const int ServerPort = 5001; // Puerto del ControlServer
        private readonly char FieldSeparator = (char)28; // ASCII 28

        private Button btnColocarEnServicio;
        private Button btnColocarFueraDeServicio;
        private Button btnObtenerContadores;
        private Button btnAplicarCarga;
        private Button btnObtenerStatus;
        private Button btnFueraDeLinea;
        private Button btnInactivar;
        private ListBox listBoxCajeros;
        private ListBox listBoxLogs;

        private enum Comandos
        {
            ColocarEnServicio = 01, // 01
            ColocarFueraDeServicio = 02, // 02
            ObtenerContadores = 03, // 03
            AplicarCarga = 04, // 04
            ObtenerStatus = 05, // 05
            FueraDeLinea = 06, // 06
            Inactivar = 07 // 07
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnColocarEnServicio = new Button();
            this.btnColocarFueraDeServicio = new Button();
            this.btnObtenerContadores = new Button();
            this.btnAplicarCarga = new Button();
            this.btnObtenerStatus = new Button();
            this.btnFueraDeLinea = new Button();
            this.btnInactivar = new Button();
            this.listBoxCajeros = new ListBox();
            this.listBoxLogs = new ListBox();

            // Configuración del Formulario
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "Panel de Control ATM";

            // Lista de cajeros predefinidos
            this.listBoxCajeros.Items.AddRange(new object[] { "C001", "C002", "C003" });
            this.listBoxCajeros.Location = new System.Drawing.Point(20, 20);
            this.listBoxCajeros.Size = new System.Drawing.Size(150, 100);

            // Tamaño de los botones
            int buttonWidth = 150;
            int buttonHeight = 50;
            int buttonSpacing = 10;

            // Botón Colocar en Servicio
            this.btnColocarEnServicio.Text = "Colocar en Servicio";
            this.btnColocarEnServicio.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnColocarEnServicio.Location = new System.Drawing.Point(200, 20);
            this.btnColocarEnServicio.Click += new EventHandler(this.btnColocarEnServicio_Click);

            // Botón Colocar Fuera de Servicio
            this.btnColocarFueraDeServicio.Text = "Colocar Fuera de Servicio";
            this.btnColocarFueraDeServicio.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnColocarFueraDeServicio.Location = new System.Drawing.Point(200 + buttonWidth + buttonSpacing, 20);
            this.btnColocarFueraDeServicio.Click += new EventHandler(this.btnColocarFueraDeServicio_Click);

            // Botón Obtener Contadores
            this.btnObtenerContadores.Text = "Obtener Contadores";
            this.btnObtenerContadores.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnObtenerContadores.Location = new System.Drawing.Point(200 + 2 * (buttonWidth + buttonSpacing), 20);
            this.btnObtenerContadores.Click += new EventHandler(this.btnObtenerContadores_Click);

            // Botón Aplicar Carga
            this.btnAplicarCarga.Text = "Aplicar Carga";
            this.btnAplicarCarga.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnAplicarCarga.Location = new System.Drawing.Point(200 + 3 * (buttonWidth + buttonSpacing), 20);
            this.btnAplicarCarga.Click += new EventHandler(this.btnAplicarCarga_Click);

            // Botón Obtener Status
            this.btnObtenerStatus.Text = "Obtener Status";
            this.btnObtenerStatus.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnObtenerStatus.Location = new System.Drawing.Point(200, 20 + buttonHeight + buttonSpacing);
            this.btnObtenerStatus.Click += new EventHandler(this.btnObtenerStatus_Click);

            // Botón Fuera de Línea
            this.btnFueraDeLinea.Text = "Fuera de Línea";
            this.btnFueraDeLinea.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnFueraDeLinea.Location = new System.Drawing.Point(200 + buttonWidth + buttonSpacing, 20 + buttonHeight + buttonSpacing);
            this.btnFueraDeLinea.Click += new EventHandler(this.btnFueraDeLinea_Click);

            // Botón Inactivar
            this.btnInactivar.Text = "Inactivar";
            this.btnInactivar.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnInactivar.Location = new System.Drawing.Point(200 + 2 * (buttonWidth + buttonSpacing), 20 + buttonHeight + buttonSpacing);
            this.btnInactivar.Click += new EventHandler(this.btnInactivar_Click);

            // ListBox para mostrar logs
            this.listBoxLogs.Location = new System.Drawing.Point(20, 150);
            this.listBoxLogs.Size = new System.Drawing.Size(760, 400);

            // Agregar controles al formulario
            this.Controls.Add(this.listBoxCajeros);
            this.Controls.Add(this.btnColocarEnServicio);
            this.Controls.Add(this.btnColocarFueraDeServicio);
            this.Controls.Add(this.btnObtenerContadores);
            this.Controls.Add(this.btnAplicarCarga);
            this.Controls.Add(this.btnObtenerStatus);
            this.Controls.Add(this.btnFueraDeLinea);
            this.Controls.Add(this.btnInactivar);
            this.Controls.Add(this.listBoxLogs);
        }

        private async void btnColocarEnServicio_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.ColocarEnServicio);
        }

        private async void btnColocarFueraDeServicio_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.ColocarFueraDeServicio);
        }

        private async void btnObtenerContadores_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.ObtenerContadores);
        }

        private async void btnAplicarCarga_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.AplicarCarga);
        }

        private async void btnObtenerStatus_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.ObtenerStatus);
        }

        private async void btnFueraDeLinea_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.FueraDeLinea);
        }

        private async void btnInactivar_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.Inactivar);
        }

        private async Task EnviarComando(Comandos comando)
        {
            if (listBoxCajeros.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un cajero primero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cajero = listBoxCajeros.SelectedItem.ToString();
            string mensaje = $"{cajero}{FieldSeparator}{((int)comando).ToString("D2")}";

            try
            {
                _client = new TcpClient(ServerIP, ServerPort);
                _stream = _client.GetStream();

                byte[] mensajeBytes = Encoding.UTF8.GetBytes(mensaje);
                await _stream.WriteAsync(mensajeBytes, 0, mensajeBytes.Length);

                listBoxLogs.Items.Add($"Enviado: {mensaje}");

                // Recibir respuesta del servidor
                string respuesta = await RecibirMensaje();
                ProcesarMensaje(respuesta);

                _stream.Close();
                _client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con el servidor: " + ex.Message);
            }
        }

        private async Task<string> RecibirMensaje()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        private void ProcesarMensaje(string mensaje)
        {
            string[] partes = mensaje.Split(FieldSeparator);
            if (partes.Length < 2)
            {
                listBoxLogs.Items.Add("Mensaje recibido inválido.");
                return;
            }

            string codigo = partes[1];
            switch (codigo)
            {
                case "01":
                    listBoxLogs.Items.Add("Comando Ejecutado");
                    break;
                case "02":
                    listBoxLogs.Items.Add("Comando Rechazado");
                    break;
                case "03":
                    listBoxLogs.Items.Add("Cajero Inexistente");
                    break;
                default:
                    listBoxLogs.Items.Add("Código de respuesta desconocido.");
                    break;
            }
        }
    }
}
