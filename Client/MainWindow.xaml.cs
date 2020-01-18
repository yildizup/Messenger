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
        private Thread _thread = null;
        public MainWindow()
        {
            InitializeComponent();
            _thread = new Thread(LoopConnect);
            _thread.Start();
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

        }
    }
}
