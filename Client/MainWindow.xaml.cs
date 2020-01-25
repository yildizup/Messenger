using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // 

        public MainWindow()
        {
            InitializeComponent();

            LoopConnect();
        }

        private void SendLoop()
        {
            string req = "";

            req = "Hallo das ist meine Nachricht";

            byte[] buffer = Encoding.ASCII.GetBytes(req);
            _clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[1024];
            int rec = _clientSocket.Receive(receivedBuffer);
            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);
            txtChatplace.Text = "Empfangen:" + Encoding.ASCII.GetString(data);


        }

        private void LoopConnect()
        {
            int attempts = 0;

            while (!_clientSocket.Connected) //solange der Client nicht verbunden ist
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(IPAddress.Loopback, 100); //Loopback ist die lokale IP-Adresse: 127.0.0.1

                    txtStatus.Text = "Verbindung wurde erfolgreich hergestellt";
                    SendLoop();
                }
                catch (SocketException)
                {

                }

            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
