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
        #region Variablen
        public TcpClient client;
        public NetworkStream netStream;
        public BinaryFormatter bFormatter;

        User individualUser; // Informationen über den aktuell eingeloggten User
        public List<User> listContacts = new List<User>(); //Die Kontaktliste des jeweiligen Benutzers

        Thread tcpThread;
        #endregion


        public SClient(TcpClient c)
        {
            //Für jeden Client soll ein neuer Thread erstellen werden.TODO:  observer design pattern anschauen.
            client = c;
            tcpThread = new Thread(SetupConn);
            tcpThread.Start();
            bFormatter = new BinaryFormatter();
        }


        #region Verbindungsauf- und Abbau

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
                        Receiver(); // Dem Client in einer Dauerschleife zuhören. 
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

        public void CloseConn()
        {

            if (individualUser != null)
            {
                // Benutzer als abgemeldet markieren
                UserController.ConnectedUsers[UserController.GetIndexOfUser(individualUser.email)].status = false;
                dbController.ChangeStatus(individualUser.email, false);
            }

            AdditionalHeader header = new AdditionalHeader(ComHeader.hDisconnect); //Bestätigung an Client senden
            bFormatter.Serialize(netStream, header);

            // Verbindung schließen
            //tcpThread.Abort();
            client.Close();

        }

        #endregion


        #region Anmeldung- und Registrierung

        public void CreateUser(string email, string password)
        {

            // Wenn die Email noch nicht existiert, kann der Benutzer erstellt werden
            if (dbController.CheckUserAndCreate(email, password))
            {
                // Benutzer in die Liste "ConnectedUsers" hinzufügen. Mithilfe dieser Liste wird der jeweilige Socket des Clients angesprochen.
                User user = new User();
                user.email = email;
                UserController.ConnectedUsers.Add(user);


                // Benutzer konnte erfolgreich erstellt werden
                // Rückmeldung, dass die Registrierung erfolgreich war
                Console.WriteLine("[{0}] Die Registrierung war erfolgreich", DateTime.Now);
                AdditionalHeader header = new AdditionalHeader(ComHeader.hRegistrationOk);
                bFormatter.Serialize(netStream, header);
            }
            else
            {
                //Email adresse existiert bereits
                Console.WriteLine("[{0}] Die E-Mail Adresse existiert bereits.", DateTime.Now);
                AdditionalHeader header = new AdditionalHeader(ComHeader.hRegistrationNotOk);
                bFormatter.Serialize(netStream, header);
            }
        }



        public void Login(string email, string password)
        {

            switch (dbController.Login(email, password))
            {
                case 0:
                    // Wenn alle Daten richtig sind


                    //User als angemeldet markieren
                    UserController.ConnectedUsers[UserController.GetIndexOfUser(email)].status = true;
                    dbController.ChangeStatus(email, true);


                    //Socket des jeweiligen Users speichern
                    UserController.ConnectedUsers[UserController.GetIndexOfUser(email)].Connection = this;
                    individualUser = UserController.ConnectedUsers[UserController.GetIndexOfUser(email)]; //Um zu wissen wer der aktuelle User ist
                    listContacts = dbController.LoadContacts(email);
                    Console.WriteLine("[{0}] Client ({1}) hat sich angemeldet.", DateTime.Now, individualUser.email);

                    AdditionalHeader header = new AdditionalHeader(ComHeader.hLoginOk);
                    bFormatter.Serialize(netStream, header);


                    ContactList contactList = new ContactList();
                    List<User> tmp = dbController.LoadContacts(email);
                    contactList.listContacts = dbController.LoadContacts(email);//Die Kontakte des eingeloggten Users laden
                    bFormatter.Serialize(netStream, contactList.listContacts);


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

        #endregion



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
                            //Ist der Empfänger Online ?
                            if (UserController.ConnectedUsers[indexReceiver].status == true)
                            {
                                NetworkStream netStreamOfReceiver = ((SClient)UserController.ConnectedUsers[indexReceiver].Connection).netStream;

                                //Zuerst den Header senden
                                AdditionalHeader sHeader = new AdditionalHeader(ComHeader.hReceived);
                                bFormatter.Serialize(netStreamOfReceiver, sHeader);

                                //Sende Nachricht zum Empfänger
                                MessageReceived messageReceived = new MessageReceived();
                                messageReceived.From = individualUser.email;
                                messageReceived.Message = message.Msg;
                                bFormatter.Serialize(netStreamOfReceiver, messageReceived);

                                //Speichere Nachricht in der Datenbank
                                dbController.SaveMessage(individualUser.email, message.To, message.Msg, false);
                            }
                            else
                            {
                                //Speichere die Nachricht in der Datenbank
                                dbController.SaveMessage(individualUser.email, message.To, message.Msg, false);
                            }
                            break;

                        case ComHeader.hDisconnect:
                            if (individualUser != null) //individualUser ist null, wenn der Benutzer sich nur registrieren möchte
                            {
                                Console.WriteLine("[{0}] Client ({1}) hat sich abgemeldet", DateTime.Now, individualUser.email);
                                CloseConn();
                            }
                            else
                            {
                                Console.WriteLine("[{0}] Client hat sich abgemeldet", DateTime.Now);
                                CloseConn();
                            }
                            break;
                        case ComHeader.hChat: // Wenn nach dem Inhalt eines "Chats" gefragt wird

                            AdditionalHeader h = new AdditionalHeader(ComHeader.hChat);
                            bFormatter.Serialize(netStream, h);
                            ChatPerson chatPerson = new ChatPerson();
                            chatPerson.Email = ((ChatPerson)bFormatter.Deserialize(netStream)).Email;

                            //Die ungelesenen Nachrichten als gelesen markieren
                            dbController.MarkNotReceivedMessagesAsReceived(individualUser.email, chatPerson.Email);


                            ChatContent chatContent = new ChatContent();
                            chatContent.chatContent = dbController.LoadChat(individualUser.email, chatPerson.Email);
                            bFormatter.Serialize(netStream, chatContent);
                            break;

                        case ComHeader.hAddContact:
                            #region Kontakt hinzufügen
                            ChatPerson friend = new ChatPerson();
                            friend = (ChatPerson)bFormatter.Deserialize(netStream);
                            // Nur wenn der zu Hinzufügende Freund existiert TODO: Fehlermeldung wenn Benutzer nicht existiert
                            if (dbController.DoesUserExist(friend.Email))
                            {
                                // neuen Kontakt in die Datenbank hinzufügen
                                dbController.AddContact(individualUser.email, friend.Email);

                                AdditionalHeader htmp = new AdditionalHeader(ComHeader.hAddContact);
                                bFormatter.Serialize(netStream, htmp);

                                bFormatter.Serialize(netStream, dbController.LoadContacts(individualUser.email));//Die Kontakte des Users erneut laden
                            }
                            #endregion
                            break;
                        case ComHeader.hState:

                            AdditionalHeader head = new AdditionalHeader(ComHeader.hState);
                            bFormatter.Serialize(netStream, head);

                            bFormatter.Serialize(netStream, dbController.LoadContacts(individualUser.email));//Die Kontakte des Users erneut laden

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


        public void LoadChat()
        {

        }



        #region Events
        public event EventHandler wrongEmail;

        virtual protected void OnWrongEmail()
        {
            if (wrongEmail != null) //TODO: Recherchieren
            {
                wrongEmail(this, EventArgs.Empty); //Event wird ausgelöst
            }
        }
        #endregion


    }
}

