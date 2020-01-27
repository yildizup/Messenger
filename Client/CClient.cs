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


        #region Packettypen
        /* Hier kommen die Packettypen rein. Jeder Byte wird eine bestimmte Bedeutung haben. Zum Beispiel wird ein Packet "send" heißen. So kann der Server die Packete zuordnen.
         */

        #endregion

        Thread tcpThread;

        public string Server { get { return "localhost"; } }
        public int Port { get { return 2000; } }

        public TcpClient client;
        public NetworkStream netStream; //Die Klasse stellt Methoden zum Senden und empfangen von Daten über Stream Sockets bereit.
        public BinaryReader br;
        public BinaryWriter bw;

        public CClient()
        {
        }

        public void SetupConn()  // Verbindung aufbauen
        {
            client = new TcpClient(Server, Port); //Verbindung zum Server aufbauen
            netStream = client.GetStream(); 

            br = new BinaryReader(netStream);
            bw = new BinaryWriter(netStream);

        }

        public void CloseConn() // Verbindung beenden
        {

        }

        void Receiver()  // Empfange alle Einkommenden Packete.
        {

        }


        public void SendMessage(string msg)
        {
            bw.Write(msg);
            bw.Flush(); // Löscht sämtliche Puffer für den aktuellen Writer und veranlasst die Ausgabe aller gepufferten Daten an das zugrunde liegende Gerät. (.NET-Dokumentation)
        }

        public void Disconnect()
        {
            // Wenn der Client verbunden ist, kann man auch wieder die Verbindung schließen
        }

    }
}
