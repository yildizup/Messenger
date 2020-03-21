# Telefonico

## Inhaltsverzeichnis
- [Telefonico](#telefonico)
  * [toc](#toc)
  * [Einrichten](#einrichten)
    + [Adressen festlegen](#adressen-festlegen)
  * [Testdaten](#testdaten)
  * [Server und Client starten](#server-und-client-starten)
  * [Chat](#chat)

## Einrichten

Bitte zuerst die Datenbank einrichten. Bitte führen Sie dafür den Code in folgender Datei aus: "datenbank/skelett.sql".

**Achtung ! Falls Sie bereits eine Datenbank mit dem Namen Telefonico haben. Bitte folgenden Befehl ausführen. `drop database telefonico;`**

![Rückmeldung](./medien/doku/datenbank_reply.jpg)

### Adressen festlegen

Die Adresse können Sie in folgenden Klassen festlegen:

![](./medien/doku/ip-adressen_festlegen.jpg)

## Testdaten

Zum testen können Sie gerne folgende Datensätze verwenden.

| Email         | Passwort     |
|---------------|--------------|
|neo@matrix.com | 1234|
|cooper@interstellar.com | 1234|
| absender@gmail.com | 1|
| empfänger@gmail.com | 1|

## Server und Client starten

Nun können Sie den Server und den Client starten.

![Hauptmenü. Links: Cient Rechts: Server](./medien/doku/hauptmenue.jpg)


## Chat

![Chatmenü](./medien/doku/chatmenue.jpg)

Der Client fragt wiederholend in einem bestimmten Zeitintervall den Server nach neuen Nachrichten. Die Anzahl neuer Nachrichten wird in der Kontaktliste angezeigt:

![neue Nachrichten](./medien/doku/chatmenue_neue-nachrichten.jpg)

![Ihre neuen Nachrichten](./medien/doku/chatmenue_chatinhalt.jpg)
