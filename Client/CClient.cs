using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net.Sockets;

namespace Client
{
    class CClient
    {

        Thread tcpThread;

        public string Server { get { return "localhost"; } }
        public int Port { get { return 2000; } }

        public TcpClient client;
        public NetworkStream netStream; //Die Klasse stellt Methoden zum Senden und empfangen von Daten über Stream Sockets bereit.
        public BinaryReader br;
        public BinaryWriter bw;
        string email; //TODO: schönes Feature "Passwort vergessen ? --> Email senden"
        string password;

        public CClient()
        {
            tcpThread = new Thread(testcode);
            tcpThread.Start();
        }


        void testcode()
        {
            EstablishConnection();
            Login("test@gmail.com", "abcdef");

        }

        public void SetupConn()  // Verbindung aufbauen
        {
            netStream = client.GetStream();

            br = new BinaryReader(netStream);
            bw = new BinaryWriter(netStream);

            //bw.Write(ComHeader.hRegister);
            //bw.Flush();


        }


        public void Register(string mail, string pw)
        {
            bw.Write(ComHeader.hRegister);
            bw.Write(mail);
            bw.Write(pw);
            bw.Flush();
        }

        public void Login(string mail, string pw)
        {
            bw.Write(ComHeader.hLogin);
            bw.Write(mail);
            bw.Write(pw);
            bw.Flush();
        }

        public void SendMessage(string msg)
        {
            bw.Write(msg);
            bw.Flush(); // Löscht sämtliche Puffer für den aktuellen Writer und veranlasst die Ausgabe aller gepufferten Daten an das zugrunde liegende Gerät. (.NET-Dokumentation)
        }


        /// <summary>
        /// Versucht eine Verbindung aufzubauen
        /// </summary>
        public void EstablishConnection()
        {
            try
            {
                client = new TcpClient(Server, Port); //Verbindung zum Server aufbauen
                AreWeConnected = true;
                SetupConn();
            }

            catch (Exception e)
            {
                AreWeConnected = false;
            }


        }

        public void CloseConn() // Verbindung beenden
        {

        }

        void Receiver()  // Empfange alle Einkommenden Packete.
        {

        }

        /// <summary>
        /// Zum registrieren
        /// </summary>
        /// <param name="email">Email Adresse des Users</param>
        /// <param name="password">Paswort des Users</param> TODO: Passwort verschlüsseln



        public void Disconnect()
        {
            // Wenn der Client verbunden ist, kann man auch wieder die Verbindung schließen
        }

        public bool AreWeConnected { get; set; }

    }
}
