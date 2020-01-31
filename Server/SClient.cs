using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class SClient
    {
        public TcpClient client;
        public NetworkStream netStream;
        public BinaryReader br;
        public BinaryWriter bw;

        public SClient(TcpClient c)
        {
            //Für jeden Client soll ein neuer Thread erstellen werden. TODO:  observer design pattern anschauen.
            client = c;

            (new Thread(new ThreadStart(SetupConn))).Start();

        }


        public void SetupConn()
        {
            Console.WriteLine("[{0}] Neue Verbindung!", DateTime.Now);
            netStream = client.GetStream();

            br = new BinaryReader(netStream);
            bw = new BinaryWriter(netStream);

            byte header = br.ReadByte(); //damit der Server weiß, um was für eine Art von Packet es sich handelt, liest er zuerst den "Header".

            switch (header)
            {
                case ComHeader.hRegister:
                    Register();
                    break;
            }


            Receiver();
        }


        /// <summary>
        /// Frage: Darf diese Klasse den dbController kennen ? Welche Lösung ist bewährter ?
        /// </summary>
        void Register()
        {
            //Nach dem "Header" werden diese Werte übergeben
            string email = br.ReadString();
            string password = br.ReadString();
            dbController.CreateUser(email, password);

        }


        /// <summary>
        /// Wartet fortlaufend auf Packete vom Client
        /// </summary>
        public void Receiver()
        {
            try
            {
                while (client.Connected) //solange der Client verbunden ist
                {
                    string msg = br.ReadString();
                    Console.WriteLine("Nachricht empfangen: {0}", msg);
                }
            }

            catch (IOException) { }

        }

        public void CloseConn()
        {
        }
    }
}
