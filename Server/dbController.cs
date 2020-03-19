using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using SharedClass;

namespace Server
{
    static class dbController
    {
        #region Variablen

        internal static MySqlConnection con;  // wird verwendet, um eine Verbindung mit der Datenbank herzustellen.
        internal static string connectionString;


        #endregion

        static dbController()
        {
            //Datenbank Verbindung
            connectionString = @"host=127.0.0.1;user=root;database=telefonico";
            con = new MySqlConnection(connectionString);
        }

        #region Anmeldung und Registrierung

        static internal void CreateUser(string email, string password, string fsname)
        {
            MySqlCommand insertCommand = new MySqlCommand("insert into user (email,password,fsname, status) values(@email,@password,@fsname, false)");
            insertCommand.Parameters.AddWithValue("@email", email);
            insertCommand.Parameters.AddWithValue("@password", password);
            insertCommand.Parameters.AddWithValue("@fsname", fsname);
            insertCommand.Connection = con;

            con.Open();
            insertCommand.ExecuteNonQuery();
            con.Close();

        }


        /// <summary>
        /// Prüft, ob der User existiert
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true, wenn user existiert</returns>
        static internal bool DoesUserExist(string email)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select * from user where email=@email";
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Connection = con;

            con.Open();

            MySqlDataReader check = cmd.ExecuteReader();
            bool valueOfRead = check.Read();
            check.Close(); // Warum muss das DataReader Objekt "geschlossen" werden ?
            con.Close();

            return valueOfRead;


        }


        /// <summary>
        /// Erstellt nach einer erfolgreichen Überprüfung einen User
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>true, wenn der User erstellt werden konnte</returns>
        static internal bool CheckUserAndCreate(string email, string password, string fsname)
        {
            // Wenn ein User nicht existiert, kann ein Konto erstellt werden
            if (!DoesUserExist(email))
            {
                CreateUser(email, password, fsname);
                return true;
            }
            else
            {
                return false;
            }

        }



        /// <summary>
        /// vergleicht Passwort mit der Datenbank
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>true, wenn Passwort mit der Datenbank übereinstimmt</returns>
        static internal bool CheckPassword(string email, string password)
        {

            MySqlCommand cmd = new MySqlCommand();

            cmd.CommandText = "Select password from user where email=@email";
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Connection = con;

            con.Open();

            MySqlDataReader dr = cmd.ExecuteReader();

            // In ein 'DataTable' Objekt schreiben.
            DataTable dt = new DataTable();
            dt.Load(dr);

            con.Close(); //TODO: Schauen, ob ein Fehler auftritt

            // eingegebenes Passwort mit der Datenbank vergleichen
            if (password == (string)dt.Rows[0][0])
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>
        /// 0: Wenn die Daten korrekt sind
        /// 1: Wenn Benutzer nicht existiert
        /// 2: Wenn das Passwort falsch ist
        /// </returns>
        static internal int Login(string email, string password)
        {
            // Wenn User nicht existiert
            if (!dbController.DoesUserExist(email))
            {
                //Rückmeldung, dass solch ein User nicht existiert
                return 1;

            }
            else
            {
                //Wenn das Passwort richtig ist
                if (CheckPassword(email, password))
                {
                    return 0;

                }
                else
                {

                    return 2;
                }
            }

        }

        /// <summary>
        /// Ändert Aktivitätsstatus des Users
        /// </summary>
        /// <param name="email"></param>
        /// <param name="status"></param>
        static internal void ChangeStatus(string email, bool status)
        {
            if (status)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "update user set status = true where email=@email";
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            else
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "update user set status = false where email=@email";
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        #endregion


        #region Datensätze laden


        static internal DataTable LoadUsers()
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select * from user";
            cmd.Connection = con;

            con.Open();

            MySqlDataReader dr = cmd.ExecuteReader();

            // In ein 'DataTable' Objekt schreiben.
            DataTable dt = new DataTable();
            dt.Load(dr);

            con.Close();

            return dt;
        }

        //TODO: Recherchieren über Vor- und Nachteilen von Events in einer static Class

        static internal List<User> LoadContacts(string email)
        {
            List<User> listContacts = new List<User>();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select main_email, friend_email, status, fsname from contacts c join user u on (c.friend_email = u.email) where main_email=@email"; // Abfrage nach allen Kontakten des Users
            //cmd.CommandText = "Select main_email, friend_email, status from contacts c join user u on (c.friend_email = u.email) where main_email=@email"; // Abfrage nach allen Kontakten des Users
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Connection = con;

            con.Open();

            MySqlDataReader dr = cmd.ExecuteReader();

            // In ein 'DataTable' Objekt schreiben.
            DataTable dt = new DataTable();
            dt.Load(dr);

            con.Close();


            foreach (DataRow row in dt.Rows)
            {
                User tmpUser = new User();
                tmpUser.Email = row["friend_email"].ToString();
                tmpUser.Status = (bool)row["status"];
                tmpUser.FsName = row["fsname"].ToString();

                #region Neue Nachrichten

                tmpUser.NewMessages = CountNewMessages(email, row["friend_email"].ToString());

                #endregion

                listContacts.Add(tmpUser);
            }

            return listContacts;
        }

        static internal int CountNewMessages(string email, string friend_email)
        {
            MySqlCommand cmd = new MySqlCommand();
            // Alle Nachrichten, die vom "Freund" gesendet wurden und noch nicht gelesen wurden zählen
            cmd.CommandText = "Select count(main_email) as sum from chat where (( main_email=@friendemail)&& (friend_email=@mainemail) && received = 0)";
            cmd.Parameters.AddWithValue("@mainemail", email);
            cmd.Parameters.AddWithValue("@friendemail", friend_email);
            cmd.Connection = con;

            con.Open();
            MySqlDataReader dr = cmd.ExecuteReader();

            // In ein 'DataTable' Objekt schreiben.
            DataTable dt = new DataTable();
            dt.Load(dr);

            DataRow tmpRow = dt.Rows[0];

            con.Close();


            return int.Parse(tmpRow["sum"].ToString()); //Die Anzahl der neuen Nachrichten zurücksenden
        }

        static internal DataTable LoadChat(string main_email, string friend_email)
        {
            DataTable dtChat = new DataTable();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select * from chat where (main_email=@mainemail || main_email=@friendemail)&& (friend_email=@friendemail || friend_email=@mainemail) order by thetime asc";
            cmd.Parameters.AddWithValue("@mainemail", main_email);
            cmd.Parameters.AddWithValue("@friendemail", friend_email);
            cmd.Connection = con;

            con.Open();

            MySqlDataReader dr = cmd.ExecuteReader();

            // In ein 'DataTable' Objekt schreiben.
            dtChat.Load(dr);
            con.Close();



            // Wenn die Nachricht vom aktuellen Client ist, soll statt der Email "Sie" stehen
            foreach (DataRow r in dtChat.Rows)
            {
                if ((string)r["main_email"] == main_email)
                {
                    r["main_email"] = "Sie"; // Namen ändern
                }
            }

            return dtChat;
        }

        static internal void MarkNotReceivedMessagesAsReceived(string main_email, string friend_email)
        {

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "update chat set received = true where main_email=@friendemail && friend_email=@mainemail && received = false;";
            cmd.Parameters.AddWithValue("@mainemail", main_email);
            cmd.Parameters.AddWithValue("@friendemail", friend_email);
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

        }


        #endregion


        #region Datensätze speichern


        static internal void SaveMessage(string main_email, string friend_email, string message, bool received)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "insert into chat (main_email, friend_email, message, thetime, received) values (@mainemail,@friendemail,@message, now(), @received);";
            cmd.Parameters.AddWithValue("@mainemail", main_email);
            cmd.Parameters.AddWithValue("@friendemail", friend_email);
            cmd.Parameters.AddWithValue("@message", message);
            cmd.Parameters.AddWithValue("@received", received);
            cmd.Connection = con;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        /// <summary>
        /// Kontakt des Benutzers in die Datenbank hinzufügen
        /// </summary>
        /// <param name="main_email"></param>
        /// <param name="friend_email"></param>
        static internal void AddContact(string main_email, string friend_email)
        {
            #region prüfen, ob der Kontakt bereits existiert
            MySqlCommand checkCmd = new MySqlCommand();
            checkCmd.CommandText = "Select * from contacts where main_email=@main_email && friend_email=@friend_email";
            checkCmd.Parameters.AddWithValue("@main_email", main_email);
            checkCmd.Parameters.AddWithValue("@friend_email", friend_email);
            checkCmd.Connection = con;
            con.Open();
            MySqlDataReader check = checkCmd.ExecuteReader();
            bool valueOfRead = check.Read();
            check.Close(); // Warum muss das DataReader Objekt "geschlossen" werden ?
            con.Close();
            #endregion

            // Man darf sich nicht als Kontakt hinzufügen
            if (!(main_email == friend_email)) 
            {
                // Wenn der Kontakt noch nicht existiert, wird er hinzugefügt
                if (!valueOfRead)
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "insert into contacts (main_email, friend_email) values (@mainemail,@friendemail);";
                    cmd.Parameters.AddWithValue("@mainemail", main_email);
                    cmd.Parameters.AddWithValue("@friendemail", friend_email);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            else
            {
                //TODO: Fehlermeldung implementieren
            }
        }



        #endregion



    }
}
