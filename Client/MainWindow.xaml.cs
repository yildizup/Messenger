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
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Thread _threadTryToConnect = null;
        private Thread _threadSendMessage = null;

        public MainWindow()
        {
            InitializeComponent();
            _threadTryToConnect = new Thread(LoopConnect);
            _threadTryToConnect.Start();
            //_threadSendMessage = new Thread(SendLoop);
            //_threadSendMessage.Start();
        }

        private void SendLoop()
        {
                string req = "";
                this.Dispatcher.Invoke(() =>
                {
                     req = txtChatplace.Text;
                });
                byte[] buffer = Encoding.ASCII.GetBytes(req);
                _clientSocket.Send(buffer);

                byte[] receivedBuffer = new byte[1024];
                int rec = _clientSocket.Receive(receivedBuffer);
                byte[] data = new byte[rec];
                Array.Copy(receivedBuffer, data, rec);
                this.Dispatcher.Invoke(() =>
                {
                    txtChatplace.Text = "Empfangen:" + Encoding.ASCII.GetString(data);
                });


        }

        private void LoopConnect()
        {
            int attempts = 0;

            while (!_clientSocket.Connected) //solange der Client nicht verbunden ist
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(IPAddress.Loopback, 100);

                    this.Dispatcher.Invoke(() =>
                    {
                        txtStatus.Text = "Verbindung wurde erfolgreich hergestellt";
                    });
                }
                catch (SocketException)
                {

                    // TODO: Recherchieren wieso ein Thread nicht auf andere Threads zugreifen kann.
                    this.Dispatcher.Invoke(() =>
                    {
                        txtStatus.Text = "versuche Verbindung herzustellen: " + attempts.ToString();
                    });
                }

            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendLoop();
        }
    }
}
