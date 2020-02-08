using System;

public static class ComHeader
{

    /* Hier kommen die Header rein. Jeder Byte wird eine bestimmte Bedeutung haben. 
     * Die Header werden verwendet, damit der Server die Pakete zuordnen kann */

    public const byte hRegister = 0;     // Registrieren
    public const byte hLogin = 1; //Login
    public const byte hWrongPass = 2;    // falsches Passwort
    public const byte hDoesntExist = 3;     // Benutzer existiert nicht
    public const byte Exists = 4; //Benutzer existiert bereits


}
