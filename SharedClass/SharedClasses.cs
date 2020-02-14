using System;
using System.Collections.Generic;

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
    }

}



