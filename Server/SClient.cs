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


        public SClient(/*TcpClient c*/)
        {
            //Für jeden Client soll ein neuer Thread erstellen werden. TODO:  observer design pattern anschauen.


            //client = c;
            //(new Thread(new ThreadStart(SetupConn))).Start();

        }


        public void SetupConn()
        {
            Console.WriteLine("[{0}] Neue Verbindung!", DateTime.Now);
            netStream = client.GetStream();

            br = new BinaryReader(netStream);
            bw = new BinaryWriter(netStream);

            byte clientMode = br.ReadByte(); //Abfragen, ob Client sich registrieren oder einloggen möchte.
            string email = br.ReadString();
            string password = br.ReadString(); // Die potenziellen login oder Registrierungsdaten bereits speichern.


            // Wenn der Client sich registrieren möchte
            if (clientMode == ComHeader.hRegister)
            {
                Receiver();
            }
        }


        public void CreateUser(string email, string password)
        {
            // prüfen, ob die email Adresse bereits existiert
            if (!dbController.DoesUserExist(email))
            {
                // Frage: Darf diese Klasse den dbController kennen ? Welche Lösung ist bewährter ?
                dbController.CreateUser(email, password);

            }
            else
            {
                //TODO: Rückmeldung Benutzer existiert bereits
            }
        }


        /// <summary>
        /// Wartet fortlaufend auf Packete vom Client
        /// </summary>
        public void Receiver()
        {
            try
            {

                while (client.Client.Connected) //solange der Client verbunden ist
                {
                    string msg = br.ReadString();
                    Console.WriteLine("Nachricht empfangen: {0}", msg);
                }
            }

            catch (IOException e)
            {
                //Falls während eines Vorgangs ein Fehler auftreten sollte, wird von einer Verbindungsunterbrechung ausgegangen.
                Console.WriteLine("[{0}] Client hat sich abgemeldet", DateTime.Now);

            }

        }

        public void CloseConn()
        {
        }
    }
}

