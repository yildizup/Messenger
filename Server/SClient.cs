﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using SharedClass;

namespace Server
{
    public class SClient
    {
        public TcpClient client;
        public NetworkStream netStream;
        public BinaryFormatter bFormatter;

        IndividualUser individualUser; // Informationen über den aktuell eingeloggten User
        public List<string> listContacts = new List<string>(); //Die Kontaktliste des jeweiligen Benutzers



        public SClient(TcpClient c)
        {
            //Für jeden Client soll ein neuer Thread erstellen werden.TODO:  observer design pattern anschauen.
            client = c;
            (new Thread(new ThreadStart(SetupConn))).Start();
            bFormatter = new BinaryFormatter();


        }

        public void SetupConn()
        {
            try
            {
                Console.WriteLine("[{0}] Neue Verbindung!", DateTime.Now);
                netStream = client.GetStream();

                byte clientMode = ((AdditionalHeader)bFormatter.Deserialize(netStream)).PHeader; //Abfragen, ob Client sich registrieren oder einloggen möchte.

                LoginData loginData = ((LoginData)bFormatter.Deserialize(netStream));
                string email = loginData.Email;
                string password = loginData.Password;


                switch (clientMode)
                {
                    // Wenn der Client sich registrieren möchte
                    case ComHeader.hRegister:
                        Console.WriteLine("[{0}] Ein Client möchte sich registrieren...", DateTime.Now);
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
                Console.WriteLine("[{0}] Client ({1}) hat sich abgemeldet", DateTime.Now, individualUser.email);
                Console.WriteLine("{0}", e.ToString());
            }



        }


        public void CreateUser(string email, string password)
        {
            if (dbController.CreateUserAndCheck(email, password))
            {
                // Benutzer konnte erfolgreich erstellt werden
                // Rückmeldung, dass die Registrierung erfolgreich war
                Console.WriteLine("[{0}] Die Registrierung war erfolgreich", DateTime.Now);
                AdditionalHeader header = new AdditionalHeader(ComHeader.hRegistrationOk);
                bFormatter.Serialize(netStream, header);


                Receiver();
            }
            else
            {
                //Email adresse existiert bereits
                Console.WriteLine("[{0}] Die E-Mail Adresse existiert bereits.", DateTime.Now);
                AdditionalHeader header = new AdditionalHeader(ComHeader.hRegistrationNotOk);
                bFormatter.Serialize(netStream, header);
                Receiver();
            }
        }


        public void Login(string email, string password)
        {

            switch (dbController.Login(email, password))
            {
                case 0:
                    // Alle Daten richtig
                    //Socket des jeweiligen Users speichern
                    UserController.individualUsers[UserController.GetIndexOfUser(email)].Connection = this;
                    individualUser = UserController.individualUsers[UserController.GetIndexOfUser(email)]; //Um zu wissen wer der aktuelle User ist
                    listContacts = dbController.LoadContacts(email); //Die Kontakte des eingeloggten Users laden
                    Console.WriteLine("[{0}] Client ({1}) hat sich angemeldet.", DateTime.Now, individualUser.email);

                    AdditionalHeader header = new AdditionalHeader(ComHeader.hLoginOk);
                    bFormatter.Serialize(netStream, header);


                    ContactList tst = new ContactList();
                    tst.listContacts = dbController.LoadContacts(email);
                    bFormatter.Serialize(netStream, tst.listContacts);

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
                    byte header = ((AdditionalHeader)bFormatter.Deserialize(netStream)).PHeader; // Um welche Art von Paket handelt es sich

                    switch (header)
                    {
                        case ComHeader.hSend:
                            MessageSend message = new MessageSend();
                            message = (MessageSend)bFormatter.Deserialize(netStream);
                            int indexReceiver = UserController.GetIndexOfUser(message.To);
                            NetworkStream netStreamOfReceiver = UserController.individualUsers[indexReceiver].Connection.netStream;

                            //Zuerst den Header senden
                            AdditionalHeader sHeader = new AdditionalHeader(ComHeader.hReceived);
                            bFormatter.Serialize(netStreamOfReceiver, sHeader);

                            //Sende Nachricht zum Empfänger
                            MessageReceived messageReceived = new MessageReceived();
                            messageReceived.From = individualUser.email;//TODO: Hier muss der Absender hin
                            messageReceived.Msg = message.Msg;
                            bFormatter.Serialize(netStreamOfReceiver, messageReceived);



                            break;
                        case ComHeader.hDisconnect:
                            client.Close();  //Die Verbindung schließen
                            Console.WriteLine("[{0}] Client ({1}) hat sich abgemeldet", DateTime.Now, individualUser.email);
                            break;
                        case ComHeader.hChat: // Wenn nach dem Inhalt eines "Chats" gefragt wird
                            //bw.Write(ComHeader.hChat);
                            //bw.Flush();

                            break;
                    }
                }
            }

            catch (IOException e)
            {
                //Falls während eines Vorgangs ein Fehler auftreten sollte, wird von einer Verbindungsunterbrechung ausgegangen.
                Console.WriteLine("[{0}] Client ({1}) hat sich abgemeldet", DateTime.Now, individualUser.email);
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

