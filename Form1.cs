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
        private const int ServerPort = 5678; // Puerto del ControlServer
        private readonly char FieldSeparator = (char)28; // ASCII 28

        private Button btnPonerEnLinea;
        private Button btnSacarFueraDeLinea;
        private Button btnPedirContadores;
        private ListBox listBoxCajeros;
        private ListBox listBoxLogs;

        private enum Comandos
        {
            PonerEnLinea = 1, // 01
            SacarFueraDeLinea = 2, // 02
            PedirContadores = 3 // 03
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnPonerEnLinea = new Button();
            this.btnSacarFueraDeLinea = new Button();
            this.btnPedirContadores = new Button();
            this.listBoxCajeros = new ListBox();
            this.listBoxLogs = new ListBox();

            // Configuración del Formulario
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Text = "Panel de Control ATM";

            // Lista de cajeros predefinidos
            this.listBoxCajeros.Items.AddRange(new object[] { "c001", "c002", "c003" });
            this.listBoxCajeros.Location = new System.Drawing.Point(20, 20);
            this.listBoxCajeros.Size = new System.Drawing.Size(150, 100);

            // Botón Poner en Línea
            this.btnPonerEnLinea.Text = "Poner en Línea";
            this.btnPonerEnLinea.Location = new System.Drawing.Point(200, 20);
            this.btnPonerEnLinea.Click += new EventHandler(this.btnPonerEnLinea_Click);

            // Botón Sacar Fuera de Línea
            this.btnSacarFueraDeLinea.Text = "Sacar Fuera de Línea";
            this.btnSacarFueraDeLinea.Location = new System.Drawing.Point(200, 60);
            this.btnSacarFueraDeLinea.Click += new EventHandler(this.btnSacarFueraDeLinea_Click);

            // Botón Pedir Contadores
            this.btnPedirContadores.Text = "Pedir Contadores";
            this.btnPedirContadores.Location = new System.Drawing.Point(200, 100);
            this.btnPedirContadores.Click += new EventHandler(this.btnPedirContadores_Click);

            // ListBox para mostrar logs
            this.listBoxLogs.Location = new System.Drawing.Point(20, 150);
            this.listBoxLogs.Size = new System.Drawing.Size(400, 200);

            // Agregar controles al formulario
            this.Controls.Add(this.listBoxCajeros);
            this.Controls.Add(this.btnPonerEnLinea);
            this.Controls.Add(this.btnSacarFueraDeLinea);
            this.Controls.Add(this.btnPedirContadores);
            this.Controls.Add(this.listBoxLogs);
        }

        private async void btnPonerEnLinea_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.PonerEnLinea);
        }

        private async void btnSacarFueraDeLinea_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.SacarFueraDeLinea);
        }

        private async void btnPedirContadores_Click(object sender, EventArgs e)
        {
            await EnviarComando(Comandos.PedirContadores);
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

                _stream.Close();
                _client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con el servidor: " + ex.Message);
            }
        }
    }
}
