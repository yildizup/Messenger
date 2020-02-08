using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Data;

namespace Server
{

    class Program
    {
        public IPAddress ip = IPAddress.Parse("127.0.0.1");
        public int port = 2000;
        public bool running = true;
        public TcpListener server;  // TCP server


        public Program()
        {
            Console.Title = "Telefonico Server";

            server = new TcpListener(ip, port); //Server erstellen und starten
            Console.WriteLine("----- Telefonico Server -----");
            LoadUsers();
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

        void LoadUsers()
        {
            Console.WriteLine("[{0}] Benutzer werden geladen...", DateTime.Now);
            UserController.LoadUsers();
            Console.WriteLine("[{0}] Benutzer wurden erfolgreich geladen!", DateTime.Now);
        }




    }
}
