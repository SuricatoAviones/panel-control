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
        private const string ServerIP = "192.168.1.107"; // IP del ControlServer
        private const int ServerPort = 5001; // Puerto del ControlServer

        private Button btnConectar;
        private Button btnDesconectar;
        private ListBox listBoxCajeros;
        private ListBox listBoxLogs;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnConectar = new Button();
            this.btnDesconectar = new Button();
            this.listBoxCajeros = new ListBox();
            this.listBoxLogs = new ListBox();

            // Configuración del Formulario
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Text = "Panel de Control ATM";

            // Botón Conectar
            this.btnConectar.Text = "Conectar";
            this.btnConectar.Location = new System.Drawing.Point(20, 20);
            this.btnConectar.Click += new EventHandler(this.btnConectar_Click);

            // Botón Desconectar
            this.btnDesconectar.Text = "Desconectar";
            this.btnDesconectar.Location = new System.Drawing.Point(120, 20);
            this.btnDesconectar.Click += new EventHandler(this.btnDesconectar_Click);

            // ListBox para mostrar cajeros conectados
            this.listBoxCajeros.Location = new System.Drawing.Point(20, 60);
            this.listBoxCajeros.Size = new System.Drawing.Size(250, 200);

            // ListBox para mostrar logs
            this.listBoxLogs.Location = new System.Drawing.Point(300, 60);
            this.listBoxLogs.Size = new System.Drawing.Size(250, 200);

            // Agregar controles al formulario
            this.Controls.Add(this.btnConectar);
            this.Controls.Add(this.btnDesconectar);
            this.Controls.Add(this.listBoxCajeros);
            this.Controls.Add(this.listBoxLogs);
        }

        private async void btnConectar_Click(object sender, EventArgs e)
        {
            try
            {
                _client = new TcpClient(ServerIP, ServerPort);
                _stream = _client.GetStream();
                listBoxLogs.Items.Add("Conectado al servidor de control.");
                await RecibirListaCajeros();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message);
            }
        }

        private async Task RecibirListaCajeros()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                listBoxCajeros.Items.Clear();
                string[] cajeros = data.Split(',');
                foreach (var cajero in cajeros)
                {
                    listBoxCajeros.Items.Add(cajero);
                }

                listBoxLogs.Items.Add("Lista de cajeros recibida.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al recibir datos: " + ex.Message);
            }
        }

        private void btnDesconectar_Click(object sender, EventArgs e)
        {
            _stream?.Close();
            _client?.Close();
            listBoxLogs.Items.Add("Desconectado del servidor.");
        }
    }
}
