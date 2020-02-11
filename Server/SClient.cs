﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class SClient
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
            try
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
            catch (Exception e)
            {
                //Falls während eines Vorgangs ein Fehler auftreten sollte, wird von einer Verbindungsunterbrechung ausgegangen.
                Console.WriteLine("[{0}] Client hat sich abgemeldet", DateTime.Now);
                Console.WriteLine("{0}", e.ToString());
            }



        }


        public void CreateUser(string email, string password)
        {
            if (dbController.CreateUserAndCheck(email, password))
            {
                // Benutzer konnte erfolgreich erstellt werden
                // Rückmeldung, dass die Registrierung erfolgreich war
                bw.Write(ComHeader.hRegistrationOk);
                bw.Flush();
                Receiver();

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
                    //Socket des jeweiligen Users speichern
                    UserController.individualUsers[UserController.GetIndexOfUser(email)].Connection = this;
                    individualUser = UserController.individualUsers[UserController.GetIndexOfUser(email)]; //Um zu wissen wer der aktuelle User ist


                    bw.Write(ComHeader.hLoginOk);
                    bw.Flush();

                    Receiver(); // Dem Client in einer Dauerschleife zuhören
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
                    byte type = br.ReadByte();
                    switch (type)
                    {
                        case ComHeader.hSend:
                            string to = br.ReadString();
                            string msg = br.ReadString();
                            //Sende Nachricht zum Empfänger
                            int indexReceiver = UserController.GetIndexOfUser(to);
                            UserController.individualUsers[indexReceiver].Connection.bw.Write(ComHeader.hReceived);
                            UserController.individualUsers[indexReceiver].Connection.bw.Write(individualUser.email); //TODO: Hier muss der Absender hin
                            UserController.individualUsers[indexReceiver].Connection.bw.Write(msg);
                            UserController.individualUsers[indexReceiver].Connection.bw.Flush();
                            break;
                        case ComHeader.hDisconnect:
                            client.Close();  //Die Verbindung schließen
                            Console.WriteLine("[{0}] Client hat sich abgemeldet", DateTime.Now);
                            break;
                    }
                }
            }

            catch (IOException e)
            {
                //Falls während eines Vorgangs ein Fehler auftreten sollte, wird von einer Verbindungsunterbrechung ausgegangen.
                Console.WriteLine("[{0}] Client hat sich abgemeldet", DateTime.Now);
                Console.WriteLine("{0}", e.ToString()); //TODO: 
                /*
                 * Wenn ein "Client" sicht  abmeldet, erscheint eine Fehlermeldung("[...] connection was forcibly closed").
                 * Besser wäre eine Abmeldung mit einer Benachrichtigung an den Server
                 */

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

