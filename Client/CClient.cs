using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using SharedClass;

namespace Client
{
    public class CClient
    {
        #region Variablen

        Thread tcpThread;
        public string Server { get { return "localhost"; } }
        public int Port { get { return 2000; } }

        public TcpClient client;
        public NetworkStream netStream; //Die Klasse stellt Methoden zum Senden und empfangen von Daten über Stream Sockets bereit.
        public BinaryFormatter bFormatter;
        string email; //TODO: schönes Feature "Passwort vergessen ? --> Email senden"
        string password;

        public ContactList contactList;

        public List<string> contactEmails; // Die E-Mail Adresse der Kontakte befindet sich auch in contactList, doch die Views sollen diese Klasse nicht kennen. TODO: Wie kann man das anders lösen ?

        bool registrationMode = false;

        #endregion


        public CClient()
        {
            bFormatter = new BinaryFormatter();
            contactList = new ContactList();
            contactEmails = new List<string>();

        }


        #region Verbindungsauf- und Abbau

        public void Connect(string email, string password)
        {
            this.email = email;
            this.password = password;
            tcpThread = new Thread(EstablishConnection);
            tcpThread.Start();
        }

        public void SetupConn()  // Verbindung aufbauen
        {
            netStream = client.GetStream();

            if (!registrationMode) //Wenn der Client sich nicht registrieren möchte
            {

                Login(email, password);

                byte answer = ((AdditionalHeader)bFormatter.Deserialize(netStream)).PHeader; // Um welche Art von Paket handelt es sich

                if (answer == ComHeader.hLoginOk)
                {
                    contactList.listContacts = (List<User>)bFormatter.Deserialize(netStream); //TODO: Nach dem Ausdruck "typeof" recherchieren

                    // Die Email Adressen in die Kontaktliste hinzufügen
                    foreach (User user in contactList.listContacts)
                    {
                        contactEmails.Add(user.email);
                    }

                    OnLoginOK(); //Publisher aufrufen
                    Receiver();
                }
            }
            else
            {

                Register(email, password);

                byte answer = ((AdditionalHeader)bFormatter.Deserialize(netStream)).PHeader; // Um welche Art von Paket handelt es sich

                switch (answer)
                {
                    // Wenn die Registrierung erfolgreich war
                    case ComHeader.hRegistrationOk:
                        OnRegistrationOK();
                        CloseConn();
                        Receiver();
                        /* TODO: Wie läuft das zeitlich ab ? Was passiert, wenn der Client eine Anfra  sendet, um die Verbindung zu beenden und bevor er 
                         * dem Server lauschen kann, der Server bereits ein Paket gesendet hat, um die Verbindung zu schließen ?
                         */
                        break;
                    // Wenn die Registrierung nicht erfolgreich war
                    case ComHeader.hRegistrationNotOk:
                        OnRegistrationNotOk();
                        //CloseConn();
                        client.Client.Disconnect(true);
                        break;

                }

            }

        }


        public void ConnectToRegistrate(string email, string password)
        {
            this.email = email;
            this.password = password;
            registrationMode = true;
            tcpThread = new Thread(EstablishConnection);
            tcpThread.Start();
        }

        /// <summary>
        /// Versucht eine Verbindung aufzubauen
        /// </summary>
        public void EstablishConnection()
        {
            //try
            //{
            client = new TcpClient(Server, Port); //Verbindung zum Server aufbauen
            SetupConn();
            //}

            //catch (Exception e)
            //{
            //AreWeConnected = false;
            //}


        }

        public void CloseConn() // Verbindung beenden
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hDisconnect); //Server benachrichtigen, dass die Verbindung geschlossen wird
            bFormatter.Serialize(netStream, header);
        }

        #endregion


        #region Anmeldung und Registrierung

        /// <summary>
        /// Zum registrieren
        /// </summary>
        /// <param name="email">Email Adresse des Users</param>
        /// <param name="password">Paswort des Users</param> TODO: Passwort verschlüsseln
        public void Register(string email, string password)
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hRegister);
            bFormatter.Serialize(netStream, header);

            LoginData loginData = new LoginData();
            loginData.Email = email;
            loginData.Password = password;

            bFormatter.Serialize(netStream, loginData);

        }

        public void Login(string email, string password)
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hLogin);
            bFormatter.Serialize(netStream, header);

            LoginData loginData = new LoginData();
            loginData.Email = email;
            loginData.Password = password;

            bFormatter.Serialize(netStream, loginData);
        }


        #endregion


        #region Nachrichten empfangen

        /// <summary>
        /// Wartet auf Einkommende Pakete
        /// </summary>
        void Receiver()
        {
            while (client.Connected)
            {
                byte header = ((AdditionalHeader)bFormatter.Deserialize(netStream)).PHeader; // Um welche Art von Paket handelt es sich

                switch (header)
                {
                    case ComHeader.hReceived:
                        // Eine Nachricht von einem anderen Client
                        MessageReceived messageReceived = (MessageReceived)bFormatter.Deserialize(netStream);
                        OnMessageReceived(new CReceivedEventArgs(messageReceived.From, messageReceived.Message, DateTime.Now.ToString())); //Event auslösen
                        break;
                    case ComHeader.hChat: //Chat Inhalt 
                        DataTable dtChat = ((ChatContent)bFormatter.Deserialize(netStream)).chatContent;
                        OnChatReceived(new CChatContentEventArgs(dtChat)); // DataTable als Parameter übergeben. Siehe Klasse "CEvents"
                        break;
                    case ComHeader.hDisconnect:
                        tcpThread.Abort(); //In diesem Thread läuft die Methode zum Empfangen von Paketen. 
                        client.Close();
                        break;
                    case ComHeader.hAddContact: //Kontaktliste aktualisieren, wenn ein neuer Kontakt hinzugefügt wurde
                        contactList.listContacts = (List<User>)bFormatter.Deserialize(netStream);

                        contactEmails.Clear();
                        // Die Email Adressen in die Kontaktliste hinzufügen
                        foreach (User user in contactList.listContacts)
                        {
                            contactEmails.Add(user.email);
                        }

                        OnRefreshContacts(); //Event auslösen
                        break;
                    case ComHeader.hState:

                        break;
                }
            }
        }

        /// <summary>
        /// Fragt wer Online ist
        /// </summary>
        void WhoIsOnline()
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hState);
            bFormatter.Serialize(netStream, header);
        }


        #endregion



        #region Chat-Methoden

        public void LoadChat(string friend_email)
        {

            AdditionalHeader header = new AdditionalHeader(ComHeader.hChat);
            bFormatter.Serialize(netStream, header);

            ChatPerson chatPerson = new ChatPerson();
            chatPerson.Email = friend_email;

            bFormatter.Serialize(netStream, chatPerson);

        }

        /// <summary>
        /// Sendet eine Nachrichtig an einen anderen Client
        /// </summary>
        /// <param name="to">Empfänger</param>
        /// <param name="msg">Nachricht</param>
        public void SendMessage(string to, string msg)
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hSend);
            bFormatter.Serialize(netStream, header);
            MessageSend message = new MessageSend();
            message.To = to;
            message.Msg = msg;
            bFormatter.Serialize(netStream, message);
        }

        /// <summary>
        /// Ein Kontakt in die Kontaktliste hinzufügen
        /// </summary>
        /// <param name="friend_email"></param>
        public void AddContact(string friend_email)
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hAddContact);
            bFormatter.Serialize(netStream, header);
            ChatPerson friend = new ChatPerson();
            friend.Email = friend_email;
            bFormatter.Serialize(netStream, friend);
        }


        #endregion




        #region Events

        public event EventHandler LoginOK;
        public event CReceivedEventHandler MessageReceived;
        public event EventHandler RegistrationOK;
        public event EventHandler RegistrationNotOk;
        public event CChatContentEventHandler ChatReceived;
        public event EventHandler RefreshContacts;

        virtual protected void OnLoginOK()
        {
            if (LoginOK != null) // Wenn keiner "subscribet" hat, brauch man auch kein Publisher aufzurufen
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

        virtual protected void OnChatReceived(CChatContentEventArgs e)
        {
            if (ChatReceived != null)
            {
                ChatReceived(this, e);
            }
        }

        virtual protected void OnRegistrationOK()
        {
            if (RegistrationOK != null)
            {
                RegistrationOK(this, EventArgs.Empty);
            }

        }

        virtual protected void OnRegistrationNotOk()
        {
            if (RegistrationNotOk != null) // Wenn keiner "subscribet" hat, brauch man auch kein Publisher aufzurufen
            {
                RegistrationNotOk(this, EventArgs.Empty);
            }
        }

        virtual protected void OnRefreshContacts()
        {
            if (RefreshContacts != null)
            {
                RefreshContacts(this, EventArgs.Empty);

            }

        }

        #endregion

    }
}
