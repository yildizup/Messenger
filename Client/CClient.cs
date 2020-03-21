using SharedClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Client
{
    public class CClient
    {
        #region Variablen

        Thread tcpThread;
        Thread pollingThread;

        readonly object _object = new object();

        public string Server { get { return "127.0.0.1"; } }
        public int Port { get { return 2000; } }

        public TcpClient client;
        public NetworkStream netStream; //Die Klasse stellt Methoden zum Senden und empfangen von Daten über Stream Sockets bereit.
        public BinaryFormatter bFormatter;
        public string email; //TODO: schönes Feature "Passwort vergessen ? --> Email senden"
        string password;

        public ContactList contactList;

        bool registrationMode = false;

        #endregion


        public CClient()
        {
            bFormatter = new BinaryFormatter();
            contactList = new ContactList();
        }

        public string FsName { get; set; }


        #region Verbindungsauf- und Abbau

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="regMode">true --> Registrierungsanfrage</param>
        public void Connect(string email, string password, bool regMode)
        {
            this.email = email;
            this.password = Hasher.SHA1(password);
            registrationMode = regMode;

            try
            {
                client = new TcpClient(Server, Port); //Verbindung zum Server aufbauen
                tcpThread = new Thread(SetupConn);
                tcpThread.Start();
            }
            // Wenn keine Verbindung aufgebaut werden konnte.
            catch (Exception e)
            {
                OnLoginNotOk();
            }

        }

        public void SetupConn()  // Verbindung aufbauen
        {
            netStream = client.GetStream();

            if (!registrationMode) //Wenn der Client sich nicht registrieren möchte
            {
                Login(email, password);

                byte answer = ((AdditionalHeader)bFormatter.Deserialize(netStream)).PHeader; // Um welche Art von Paket handelt es sich

                switch (answer)
                {
                    case ComHeader.hLoginOk:
                        contactList.listContacts = (List<User>)bFormatter.Deserialize(netStream); //TODO: Nach dem Ausdruck "typeof" recherchieren
                        OnLoginOK(); //Publisher aufrufen
                        pollingThread = new Thread(WhoIsOnline);
                        pollingThread.Start();
                        Receiver();
                        break;
                    case ComHeader.hWrongPass:
                        OnLoginNotOk();
                        client.Close(); //Socket "schließen"
                        break;
                    case ComHeader.hDoesntExist:
                        OnLoginNotOk();
                        client.Close(); //Socket "schließen"
                        break;
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
                        /* TODO: Wie läuft das zeitlich ab ? Was passiert, wenn der Client eine Anfrage  sendet, um die Verbindung zu beenden und bevor er 
                         * dem Server lauschen kann, der Server bereits ein Paket gesendet hat, um die Verbindung zu schließen ?
                         */
                        break;
                    // Wenn die Registrierung nicht erfolgreich war
                    case ComHeader.hRegistrationNotOk:
                        OnRegistrationWrong();
                        //CloseConn();
                        client.Client.Disconnect(true);
                        break;

                }

            }

        }

        public void CloseConn() // Verbindung beenden
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hDisconnect); //Server benachrichtigen, dass die Verbindung geschlossen wird
            SendHeader(header);
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
            SendHeader(header);

            LoginData loginData = new LoginData();
            loginData.Email = email;
            loginData.Password = password;
            loginData.FsName = FsName;

            bFormatter.Serialize(netStream, loginData);

        }

        public void Login(string email, string password)
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hLogin);
            SendHeader(header);

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
                        pollingThread.Abort(); 
                        tcpThread.Abort(); //In diesem Thread läuft die Methode zum Empfangen von Paketen. 
                        client.Close();
                        break;
                    case ComHeader.hAddContact: //Kontaktliste aktualisieren, wenn ein neuer Kontakt hinzugefügt wurde
                    case ComHeader.hState: // Wenn Aktivitätsstatus der User mitgeteilt wird
                        //Kontaktliste aktualiseren
                        contactList.listContacts = (List<User>)bFormatter.Deserialize(netStream);
                        OnRefreshContacts(); //Event auslösen
                        break;
                    case ComHeader.hAddContactWrong:
                        //Wenn der Kontakt nicht hinzugefügt werden kann
                        OnAddContactWrong();
                        break;
                }
            }
        }

        /// <summary>
        /// Fragt wiederholend in einem bestimmten Zeitintervall wer online ist
        /// </summary>
        public void WhoIsOnline()
        {
            while (client.Connected)
            {
                AdditionalHeader header = new AdditionalHeader(ComHeader.hState);
                SendHeader(header);
                Thread.Sleep(1500);
            }
        }


        #endregion


        #region Chat-Methoden

        public void LoadChat(string friend_email)
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hChat);
            SendHeader(header);

            ChatPerson chatPerson = new ChatPerson();
            chatPerson.Email = friend_email;

            bFormatter.Serialize(netStream, chatPerson);
        }

        public void MessagesRead(string friend_email)
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hMessagesRead);
            SendHeader(header);

            ChatPerson chatPerson = new ChatPerson();
            chatPerson.Email = friend_email;
            bFormatter.Serialize(netStream, chatPerson);
        }

        /// <summary>
        /// Sendet eine Nachricht an einen anderen Client
        /// </summary>
        /// <param name="to">Empfänger</param>
        /// <param name="msg">Nachricht</param>
        public void SendMessage(string to, string msg)
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hSend);
            SendHeader(header);
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
            SendHeader(header);
            ChatPerson friend = new ChatPerson();
            friend.Email = friend_email;
            bFormatter.Serialize(netStream, friend);
        }

        void SendHeader(AdditionalHeader h)
        {
            lock (_object)
            {
                bFormatter.Serialize(netStream, h);
            }
        }

        #endregion


        #region Events

        public event EventHandler LoginOK;
        public event EventHandler LoginNotOk;
        public event CReceivedEventHandler MessageReceived;
        public event EventHandler RegistrationOK;
        public event EventHandler RegistrationNotOk;
        public event CChatContentEventHandler ChatReceived;
        public event EventHandler RefreshContacts;
        public event EventHandler AddContactWrong;

        virtual protected void OnLoginOK()
        {
            if (LoginOK != null) // Wenn keiner "subscribet" hat, brauch man auch kein Publisher aufzurufen
            {
                LoginOK(this, EventArgs.Empty);
            }
        }
        virtual protected void OnLoginNotOk()
        {
            if (LoginNotOk != null) // Wenn keiner "subscribet" hat, brauch man auch kein Publisher aufzurufen
            {
                LoginNotOk(this, EventArgs.Empty);
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
        virtual protected void OnRegistrationWrong()
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
        virtual protected void OnAddContactWrong()
        {
            if (AddContactWrong != null)
            {
                AddContactWrong(this, EventArgs.Empty);
            }
        }

        #endregion

    }
}
