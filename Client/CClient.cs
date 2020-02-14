using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using SharedClass;

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
        public BinaryFormatter bFormatter;
        string email; //TODO: schönes Feature "Passwort vergessen ? --> Email senden"
        string password;
        public ContactList contactList;

        bool registrationMode = false;

        public CClient()
        {
            bFormatter = new BinaryFormatter();
            contactList = new ContactList();

        }


        public void SetupConn()  // Verbindung aufbauen
        {
            netStream = client.GetStream();

            br = new BinaryReader(netStream);
            bw = new BinaryWriter(netStream);


            if (!registrationMode) //Wenn der Client sich nicht registrieren möchte
            {

                AdditionalHeader header = new AdditionalHeader(ComHeader.hLogin);
                bFormatter.Serialize(netStream, header);

                LoginData loginData = new LoginData();
                loginData.Email = email;
                loginData.Password = password;

                bFormatter.Serialize(netStream, loginData);

                byte answer = ((AdditionalHeader)bFormatter.Deserialize(netStream)).PHeader; // Um welche Art von Paket handelt es sich

                if (answer == ComHeader.hLoginOk)
                {
                    contactList.listContacts = (List<string>)bFormatter.Deserialize(netStream); //TODO: Nach dem Ausdruck "typeof" recherchieren
                    OnLoginOK(); //Publisher aufrufen
                    Receiver();
                }
            }
            else
            {

                Register(email, password);

                Byte answer = br.ReadByte();

                switch (answer)
                {
                    // Wenn die Registrierung erfolgreich war
                    case ComHeader.hRegistrationOk:
                        SmoothDisconnect();
                        OnRegistrationOK();
                        break;
                    // Wenn die Registrierung nicht erfolgreich war
                    case ComHeader.hRegistrationNotOk:
                        SmoothDisconnect();
                        OnRegistrationNotOk();
                        break;

                }

            }

        }


        public void Connect(string email, string password)
        {
            this.email = email;
            this.password = password;
            tcpThread = new Thread(EstablishConnection);
            tcpThread.Start();
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
        /// Zum registrieren
        /// </summary>
        /// <param name="email">Email Adresse des Users</param>
        /// <param name="password">Paswort des Users</param> TODO: Passwort verschlüsseln
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
            while (client.Connected)
            {
                byte header = ((AdditionalHeader)bFormatter.Deserialize(netStream)).PHeader; // Um welche Art von Paket handelt es sich

                switch (header)
                {
                    case ComHeader.hReceived:
                        // Eine Nachricht von einem anderen Client
                        MessageReceived messageReceived = (MessageReceived)bFormatter.Deserialize(netStream);
                        OnMessageReceived(new CReceivedEventArgs(messageReceived.From, messageReceived.Msg)); //Event auslösen
                        break;
                    case ComHeader.hChat: //Chat Inhalt 


                        break;


                }
            }
        }

        public void SendMessage(string to, string msg)
        {
            AdditionalHeader header = new AdditionalHeader(ComHeader.hSend);
            bFormatter.Serialize(netStream, header);
            MessageSend message = new MessageSend();
            message.To = to;
            message.Msg = msg;
            bFormatter.Serialize(netStream, message);
        }

        #endregion


        public void SmoothDisconnect()
        {
            // Wenn der Client verbunden ist, kann man auch wieder die Verbindung schließen

            bw.Write(ComHeader.hDisconnect);
            bw.Flush();

            netStream.Close(); //Stream beenden, bevor die Verbindung geschlossen wird.
            client.Close();


        }

        public void LoadChat()
        {
            bw.Write(ComHeader.hChat);
            bw.Flush();
        }




        public bool AreWeConnected { get; set; }


        // Events
        public event EventHandler LoginOK;
        public event CReceivedEventHandler MessageReceived;
        public event EventHandler RegistrationOK;
        public event EventHandler RegistrationNotOk;

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


    }
}
