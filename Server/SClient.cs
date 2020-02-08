﻿using System;
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

        IndividualUser individualUser; // Informationen über den aktuell eingeloggten User



        public SClient(TcpClient c)
        {
            //Für jeden Client soll ein neuer Thread erstellen werden.TODO:  observer design pattern anschauen.
            client = c;
            (new Thread(new ThreadStart(SetupConn))).Start();


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

            switch (clientMode)
            {
                // Wenn der Client sich registrieren möchte
                case ComHeader.hRegister:
                    CreateUser(email, password);
                    break;
                case ComHeader.hLogin:
                    Login(email, password);
                    break;
            }
        }


        public void CreateUser(string email, string password)
        {
            if(dbController.CreateUserAndCheck(email, password))
            {
                // Benutzer konnte erfolgreich erstellt werden
            }
            else
            {
                //Email adresse existiert bereits
            }
        }



        public void Login(string email, string password)
        {

            switch (dbController.Login(email, password))
            {
                case 0:
                    // Alle Daten richtig
                    Console.WriteLine("Alles richtig");
                    break;

                case 1:
                    //Benutzer existiert nicht
                    Console.WriteLine("Benutzer existiert nicht");
                    break;

                case 2:
                    //Passwort ist falsch
                    Console.WriteLine("Passwort ist falsch");
                    break;
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

        //Events 
        public event EventHandler wrongEmail;

        virtual protected void OnWrongEmail()
        {
            if (wrongEmail != null) //TODO: Recherchieren
            {
                wrongEmail(this, EventArgs.Empty); //Event wird ausgelöst
            }
        }


    }
}

