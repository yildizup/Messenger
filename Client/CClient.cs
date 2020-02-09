﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public class CClient
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

        }


        public void SetupConn()  // Verbindung aufbauen
        {
            netStream = client.GetStream();

            br = new BinaryReader(netStream);
            bw = new BinaryWriter(netStream);

            bw.Write(ComHeader.hLogin);
            bw.Write(email);
            bw.Write(password);
            bw.Flush();

            byte answer = br.ReadByte(); // Auf eine Antwort warten

            if (answer == ComHeader.hLoginOk)
            {
                OnLoginOK(); //Publisher aufrufen
                Receiver();
            }



        }


        public void Connect(string email, string password)
        {
            this.email = email;
            this.password = password;
            tcpThread = new Thread(EstablishConnection);
            tcpThread.Start();
        }


        public void Register(string email, string password)
        {
            bw.Write(ComHeader.hRegister);
            bw.Write(email);
            bw.Write(password);
            bw.Flush();
        }

        public void Login(string email, string password)
        {
            bw.Write(ComHeader.hLogin);
            bw.Write(email);
            bw.Write(password);
            bw.Flush();
        }



        /// <summary>
        /// Versucht eine Verbindung aufzubauen
        /// </summary>
        public void EstablishConnection()
        {
            //try
            //{
            client = new TcpClient(Server, Port); //Verbindung zum Server aufbauen
            AreWeConnected = true;
            SetupConn();
            //}

            //catch (Exception e)
            //{
            //AreWeConnected = false;
            //}


        }

        public void CloseConn() // Verbindung beenden
        {

        }

        #region Nachrichten senden und empfangen
        void Receiver()  // Empfange alle Einkommenden Packete.
        {

            byte type = br.ReadByte(); // um welche Art von Paket handelt es sich ?

            switch (type)
            {
                case ComHeader.hReceived:
                    // Eine Nachricht von einem anderen Client
                    string from = br.ReadString();
                    string msg = br.ReadString();
                    OnMessageReceived(new CReceivedEventArgs(from, msg)); //Event auslösen
                    break;
            }
        }

        public void SendMessage(string to, string msg)
        {
            bw.Write(ComHeader.hSend);
            bw.Write(to);
            bw.Write(msg);
            bw.Flush(); // Löscht sämtliche Puffer für den aktuellen Writer und veranlasst die Ausgabe aller gepufferten Daten an das zugrunde liegende Gerät. (.NET-Dokumentation)
        }

        #endregion


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


        // Events
        public event EventHandler LoginOK;
        public event CReceivedEventHandler MessageReceived;

        virtual protected void OnLoginOK()
        {
            if (LoginOK != null) //TODO: Recherchieren
            {
                LoginOK(this, EventArgs.Empty);
            }
        }

        virtual protected void OnMessageReceived(CReceivedEventArgs e)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, e);
            }

        }


    }
}
