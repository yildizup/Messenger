﻿using System;
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
            tcpThread = new Thread(EstablishConnection);
            tcpThread.Start();

            /* Interessant: Wenn kein Thread erstellt wird, schließt die Verbindung wieder direkt.
             * Grund: Nächste Zeile wird ausgeführt und die Methode wird beendet. Deswegen werden paralelle "Prozesse" gebraucht.
            */

        }

        public void SetupConn()  // Verbindung aufbauen
        {
            netStream = client.GetStream();

            br = new BinaryReader(netStream);
            bw = new BinaryWriter(netStream);

            //bw.Write(ComHeader.hRegister);
            //bw.Flush();


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
        public void Register(string mail, string pw)
        {
            email = mail;
            password = pw;


            bw.Write(ComHeader.hRegister);
            bw.Write(email);
            bw.Write(password);
            bw.Flush();
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

        public bool AreWeConnected { get; set; }

    }
}
