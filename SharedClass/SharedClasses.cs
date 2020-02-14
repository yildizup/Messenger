using System;
using System.Collections.Generic;
using System.Data;

namespace SharedClass
{
    [Serializable]
    public class ContactList
    {
        public List<string> listContacts = new List<string>(); //TODO: Wie kann man das besser lösen ?

    }

    public class ComHeader
    {
        /* Hier kommen die Header rein. Jeder Byte wird eine bestimmte Bedeutung haben. 
         * Die Header werden verwendet, damit der Server die Pakete zuordnen kann */

        public const byte hRegister = 0;     // Registrieren
        public const byte hLogin = 1; //Login
        public const byte hWrongPass = 2;    // falsches Passwort
        public const byte hDoesntExist = 3;     // Benutzer existiert nicht
        public const byte hExists = 4; //Benutzer existiert bereits
        public const byte hReceived = 5; // Nachricht empfangen
        public const byte hSend = 6; // Nachrichten senden
        public const byte hLoginOk = 7; // Benutzerdaten sind richtig
        public const byte hRegistrationOk = 8; // Die Registrierung war erfolgreich
        public const byte hRegistrationNotOk = 9; // Die Registrierung ist fehlgeschlagen
        public const byte hDisconnect = 10; //Um den Server zu benachrichten, dass der Client die Verbindung schließt
        public const byte hChat = 11; //Anfrage nach Chat Inhalten
    }


    [Serializable] //TODO: Recherchieren
    public class ChatContent
    {
        public DataTable chatContent = new DataTable();
    }


    [Serializable]
    public class MessageSend
    {
        public string To { get; set; }
        public string Msg { get; set; }
    }

    [Serializable]
    public class MessageReceived
    {
        public string From { get; set; }
        public string Msg { get; set; }
    }


    /// <summary>
    /// alle möglichen Header
    /// </summary>
    [Serializable]
    public class AdditionalHeader
    {
        public byte PHeader { get; }

        public AdditionalHeader(byte b)
        {
            switch (b)
            {
                case ComHeader.hLoginOk:
                    PHeader = ComHeader.hLoginOk;
                    break;
                case ComHeader.hLogin:
                    PHeader = ComHeader.hLogin;
                    break;
                case ComHeader.hRegister:
                    PHeader = ComHeader.hLogin;
                    break;
                case ComHeader.hRegistrationOk:
                    PHeader = ComHeader.hRegistrationOk;
                    break;
                case ComHeader.hRegistrationNotOk:
                    PHeader = ComHeader.hRegistrationNotOk;
                    break;
                case ComHeader.hDisconnect:
                    PHeader = ComHeader.hDisconnect;
                    break;
                case ComHeader.hReceived:
                    PHeader = ComHeader.hReceived;
                    break;
                case ComHeader.hSend:
                    PHeader = ComHeader.hSend;
                    break;
            }
        }
    }

}



