using SharedClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

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

                GeneralPackage package = new GeneralPackage();
                package = (GeneralPackage)bFormatter.Deserialize(netStream);



                LoginData loginData = ((LoginData)package.Content);
                string email = loginData.Email;
                string password = loginData.Password;
                string fsName = loginData.FsName;


                switch (package.Header)
                {
                    // Wenn der Client sich registrieren möchte
                    case ComHeader.hRegister:
                        Console.WriteLine("[{0}] Ein Client möchte sich registrieren...", DateTime.Now);
                        CreateUser(email, password, fsName);
                        //Receiver(); // Dem Client in einer Dauerschleife zuhören. 
                        break;
                    case ComHeader.hLogin:
                        Login(email, password);
                        break;
                }
            }
            catch (Exception e)
            {
                //Falls während eines Vorgangs ein Fehler auftreten sollte, wird von einer Verbindungsunterbrechung ausgegangen.
                //Console.WriteLine("[{0}] Client ({1}) hat sich abgemeldet", DateTime.Now, individualUser.Email);
                Console.WriteLine("[{0}] Client ({1}) hat sich abgemeldet", DateTime.Now, "ABC");
                Console.WriteLine("{0}", e.ToString());
            }



        }

        public void CloseConn()
        {

            if (individualUser != null)
            {
                // Benutzer als abgemeldet markieren
                UserController.ConnectedUsers[UserController.GetIndexOfUser(individualUser.Email)].Status = false;
                dbController.ChangeStatus(individualUser.Email, false);
            }

            GeneralPackage package = new GeneralPackage();
            package.Header = ComHeader.hDisconnect;

            SendPackage(package);

            // Verbindung schließen
            //tcpThread.Abort();
            client.Close();

        }

        #endregion


        #region Anmeldung- und Registrierung

        public void CreateUser(string email, string password, string fsname)
        {

            // Wenn die Email noch nicht existiert, kann der Benutzer erstellt werden
            if (dbController.CheckUserAndCreate(email, password, fsname))
            {
                // Benutzer in die Liste "ConnectedUsers" hinzufügen. Mithilfe dieser Liste wird der jeweilige Socket des Clients angesprochen.
                User user = new User();
                user.Email = email;
                UserController.ConnectedUsers.Add(user);


                // Benutzer konnte erfolgreich erstellt werden
                // Rückmeldung, dass die Registrierung erfolgreich war
                Console.WriteLine("[{0}] Die Registrierung war erfolgreich", DateTime.Now);
                GeneralPackage package = new GeneralPackage();
                package.Header = ComHeader.hRegistrationOk;
                SendPackage(package);
                Receiver(); //Dem Client in einer Dauerschleife zuhören
            }
            else
            {
                //Email adresse existiert bereits
                Console.WriteLine("[{0}] Die E-Mail Adresse existiert bereits.", DateTime.Now);
                GeneralPackage package = new GeneralPackage();
                package.Header = ComHeader.hRegistrationNotOk;
                SendPackage(package);
            }
        }



        public void Login(string email, string password)
        {
            GeneralPackage package = new GeneralPackage();

            switch (dbController.Login(email, password))
            {
                case 0:
                    // Wenn alle Daten richtig sind


                    //User als angemeldet markieren
                    UserController.ConnectedUsers[UserController.GetIndexOfUser(email)].Status = true;
                    dbController.ChangeStatus(email, true);


                    //Socket des jeweiligen Users speichern
                    UserController.ConnectedUsers[UserController.GetIndexOfUser(email)].Connection = this;
                    individualUser = UserController.ConnectedUsers[UserController.GetIndexOfUser(email)]; //Um zu wissen wer der aktuelle User ist
                    listContacts = dbController.LoadContacts(email);
                    Console.WriteLine("[{0}] Client ({1}) hat sich angemeldet.", DateTime.Now, individualUser.Email);

                    package.Header = ComHeader.hLoginOk;

                    ContactList contactList = new ContactList();
                    List<User> tmp = dbController.LoadContacts(email);
                    contactList.listContacts = dbController.LoadContacts(email);//Die Kontakte des eingeloggten Users laden

                    package.Content = contactList;

                    SendPackage(package);


                    Receiver(); // Dem Client in einer Dauerschleife zuhören
                    break;

                case 1:
                    //Benutzer existiert nicht
                    package.Header = ComHeader.hDoesntExist;
                    SendPackage(package);
                    break;

                case 2:
                    //Passwort ist falsch
                    package.Header = ComHeader.hWrongPass;
                    SendPackage(package);
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
                while (client.Client.Connected)
                {
                    GeneralPackage receivedPackage = (GeneralPackage)bFormatter.Deserialize(netStream);

                    GeneralPackage sendPackage = new GeneralPackage();

                    switch (receivedPackage.Header)
                    {
                        case ComHeader.hSend:
                            MessageSend message = new MessageSend();
                            message = (MessageSend)receivedPackage.Content;
                            int indexReceiver = UserController.GetIndexOfUser(message.To);
                            //Ist der Empfänger Online ?
                            if (UserController.ConnectedUsers[indexReceiver].Status == true)
                            {
                                NetworkStream netStreamOfReceiver = ((SClient)UserController.ConnectedUsers[indexReceiver].Connection).netStream;

                                // An den Empfänger senden
                                sendPackage.Header = ComHeader.hReceived;

                                //Sende Nachricht zum Empfänger
                                MessageReceived messageReceived = new MessageReceived();
                                messageReceived.From = individualUser.Email;
                                messageReceived.Message = message.Msg;
                                sendPackage.Content = messageReceived;

                                bFormatter.Serialize(netStreamOfReceiver, sendPackage);

                                //Speichere Nachricht in der Datenbank
                                dbController.SaveMessage(individualUser.Email, message.To, message.Msg, false);
                            }
                            else
                            {
                                //Speichere die Nachricht in der Datenbank
                                dbController.SaveMessage(individualUser.Email, message.To, message.Msg, false);
                            }
                            break;

                        case ComHeader.hDisconnect:
                            if (individualUser != null) //individualUser ist null, wenn der Benutzer sich nur registrieren möchte
                            {
                                Console.WriteLine("[{0}] Client ({1}) hat sich abgemeldet", DateTime.Now, individualUser.Email);
                                CloseConn();
                            }
                            else
                            {
                                Console.WriteLine("[{0}] Client hat sich abgemeldet", DateTime.Now);
                                CloseConn();
                            }
                            break;
                        case ComHeader.hChat: // Wenn nach dem Inhalt eines "Chats" gefragt wird

                            sendPackage.Header = ComHeader.hChat;

                            ChatPerson chatPerson = ((ChatPerson)receivedPackage.Content);


                            //Die ungelesenen Nachrichten als gelesen markieren
                            dbController.MarkNotReceivedMessagesAsReceived(individualUser.Email, chatPerson.Email);


                            ChatContent chatContent = new ChatContent();
                            chatContent.chatContent = dbController.LoadChat(individualUser.Email, chatPerson.Email);

                            sendPackage.Content = chatContent;

                            SendPackage(sendPackage);
                            break;

                        case ComHeader.hAddContact:
                            #region Kontakt hinzufügen
                            ChatPerson friend = ((ChatPerson)receivedPackage.Content);

                            // Wenn der Kontakt hinzugefügt wurden konnte 
                            if (dbController.AddContact(individualUser.Email, friend.Email))
                            {
                                sendPackage.Header = ComHeader.hAddContact;
                                sendPackage.Content = dbController.LoadContacts(individualUser.Email);

                                SendPackage(sendPackage);
                            }
                            else
                            {
                                // Wenn der Kontakt nicht hinzugeüft werden kann
                                sendPackage.Header = ComHeader.hAddContactWrong;
                                SendPackage(sendPackage);
                            }

                            #endregion
                            break;
                        case ComHeader.hState:
                            sendPackage.Header = ComHeader.hState;
                            sendPackage.Content = dbController.LoadContacts(individualUser.Email);
                            SendPackage(sendPackage);
                            break;
                        case ComHeader.hMessagesRead:
                            ChatPerson chat_friend = ((ChatPerson)receivedPackage.Content);
                            //Die Nachrichten als gelesen markieren
                            dbController.MarkNotReceivedMessagesAsReceived(individualUser.Email, chat_friend.Email);
                            break;
                    }
                }
            }

            catch (IOException e)
            {
                //Falls während eines Vorgangs ein Fehler auftreten sollte, wird von einer Verbindungsunterbrechung ausgegangen.
                Console.WriteLine("[{0}] Client ({1}) hat sich abgemeldet", DateTime.Now, individualUser.Email);
                Console.WriteLine("{0}", e.ToString()); //TODO: 
                /*
                 * Wenn ein "Client" sicht  abmeldet, erscheint eine Fehlermeldung("[...] connection was forcibly closed").
                 * Besser wäre eine Abmeldung mit einer Benachrichtigung an den Server
                 */

            }

        }

        /// <summary>
        /// Vor jeder Nachricht wird dem Client ein Header gesendet
        /// </summary>
        /// <param name="h"></param>
        void SendPackage(GeneralPackage p)
        {
            bFormatter.Serialize(netStream, p);
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



