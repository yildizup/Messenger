using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server
{

    class Program
    {

        /// <summary>
        /// InterNetwork = IPv4, Stream = Stream-oriented socket where both the sender and receiver can lump data in any size during sending or receiving of data
        /// </summary>
        //private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //private static List<Socket> _clientSockets = new List<Socket>();
        //private static byte[] _buffer = new byte[1024];

        public IPAddress ip = IPAddress.Parse("127.0.0.1");
        public int port = 2000;
        public bool running = true;
        public TcpListener server;  // TCP server

        public Program()
        {
            Console.Title = "Telefonico Server";

            server = new TcpListener(ip, port); //Server erstellen und starten
            Console.WriteLine("----- Telefonico Server -----");
            Console.WriteLine("[{0}] Server wird gestartet...", DateTime.Now);

            server.Start();
            Listen();
        }

        void Listen()  // Nach Verbindung ausschau halten.
        {
            while (running)
            {
                TcpClient tcpClient = server.AcceptTcpClient(); //wartet auf Verbindungen. Bei erfolgreicher Verbindung wird ein Objekt 'TcpClient' zurückgegeben.
                SClient client = new SClient(tcpClient); //Behandel den Client in einem neuen Thread.
            }
        }


        static void Main(string[] args)
        {
            Program p = new Program();
            Console.ReadLine();

        }



    }
}
