﻿using System;

public static class ComHeader
{

    /* Hier kommen die Header rein. Jeder Byte wird eine bestimmte Bedeutung haben. 
     * Die Header werden verwendet, damit der Server die Pakete zuordnen kann */

    public const byte hRegister = 0;     // Registrieren
    public const byte hLogin = 1; //Login


}
