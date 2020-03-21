using MySql.Data.MySqlClient;
using SharedClass;
using System.Collections.Generic;
using System.Data;

namespace Server
{
    static class dbController
    {
        #region Variablen

        //internal static MySqlConnection con;  // wird verwendet, um eine Verbindung mit der Datenbank herzustellen.
        internal static string connectionString;


        #endregion

        static dbController()
        {
            //Datenbank Verbindung
            connectionString = @"host=127.0.0.1;user=root;database=telefonico";
        }

        #region Anmeldung und Registrierung

        static internal void CreateUser(string email, string password, string fsname)
        {
            string query = "insert into users (email,password,fsname, status) values (@email, @password, @fsname, false)";

            using (var conn = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@fsname", fsname);
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }

        }


        /// <summary>
        /// Prüft, ob der User existiert
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true, wenn user existiert</returns>
        static internal bool DoesUserExist(string email)
        {
            bool valueOfRead;
            string query = "Select * from users where email=@email";

            using (var conn = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@email", email);

                conn.Open();
                using (var reader = command.ExecuteReader())
                {
                    valueOfRead = reader.Read();
                }
                conn.Close();
            }

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
            DataTable dt = new DataTable();
            string query = "Select password from users where email=@email";

            using (var conn = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@email", email);

                conn.Open();
                using (var reader = command.ExecuteReader())
                {
                    // In ein 'DataTable'  schreiben.
                    dt.Load(reader);
                }
                conn.Close();
            }

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
                string query = "update users set status = true where email=@email";
                using (var conn = new MySqlConnection(connectionString))
                using (var command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@email", email);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }

            }
            else
            {
                string query = "update users set status = false where email=@email";
                using (var conn = new MySqlConnection(connectionString))
                using (var command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@email", email);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        #endregion


        #region Datensätze laden


        static internal DataTable LoadUsers()
        {
            string query = "Select * from users";
            DataTable dt = new DataTable();
            using (var conn = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
                conn.Close();
            }


            return dt;
        }

        //TODO: Recherchieren über Vor- und Nachteilen von Events in einer static Class

        static internal List<User> LoadContacts(string email)
        {
            List<User> listContacts = new List<User>();
            string query = "Select main_email, friend_email, status, fsname from contacts c join users u on (c.friend_email = u.email) where main_email=@email"; // Abfrage nach allen Kontakten des Users
            DataTable dt = new DataTable();


            using (var conn = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@email", email);
                conn.Open();
                using (var reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
                conn.Close();
            }

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
            DataTable dt = new DataTable();
            // Alle Nachrichten, die vom "Freund" gesendet wurden und noch nicht gelesen wurden zählen
            string query = "Select count(main_email) as sum from chat where (( main_email=@friendemail)&& (friend_email=@mainemail) && received = 0)";
            using (var conn = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@mainemail", email);
                command.Parameters.AddWithValue("@friendemail", friend_email);
                conn.Open();
                using (var reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
                conn.Close();
            }
            DataRow tmpRow = dt.Rows[0];
            return int.Parse(tmpRow["sum"].ToString()); //Die Anzahl der neuen Nachrichten zurücksenden
        }

        static internal DataTable LoadChat(string main_email, string friend_email)
        {
            DataTable dtChat = new DataTable();
            string query = "Select * from chat where (main_email=@mainemail || main_email=@friendemail)&& (friend_email=@friendemail || friend_email=@mainemail) order by thetime asc";
            using (var conn = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@mainemail", main_email);
                command.Parameters.AddWithValue("@friendemail", friend_email);
                conn.Open();
                using (var reader = command.ExecuteReader())
                {
                    dtChat.Load(reader);
                }
                conn.Close();
            }

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
            string query = "update chat set received = true where main_email=@friendemail && friend_email=@mainemail && received = false;";
            using (var conn = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@mainemail", main_email);
                command.Parameters.AddWithValue("@friendemail", friend_email);

                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }
        }


        #endregion


        #region Datensätze speichern


        static internal void SaveMessage(string main_email, string friend_email, string message, bool received)
        {
            string query = "insert into chat (main_email, friend_email, message, thetime, received) values (@mainemail,@friendemail,@message, now(), @received);";

            using (var conn = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@mainemail", main_email);
                command.Parameters.AddWithValue("@friendemail", friend_email);
                command.Parameters.AddWithValue("@message", message);
                command.Parameters.AddWithValue("@received", received);

                conn.Open();
                command.ExecuteNonQuery();
                //conn.Close(); Kann man weglassen, da automatisch disposed wird
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="main_email"></param>
        /// <param name="friend_email"></param>
        /// <returns>true, wenn der Benutzer bereits in der Kontaktliste ist. </returns>
        static internal bool AlreadyFriends(string main_email, string friend_email)
        {
            // Wenn dieser Kontakt existiert und nicht er selber ist
            if (!(main_email == friend_email) && DoesUserExist(friend_email))
            {
                bool valueOfRead;
                // prüfen, ob der Kontakt bereits in der Kontaktliste existiert
                string query = "Select * from contacts where main_email=@main_email && friend_email=@friend_email";

                using (var conn = new MySqlConnection(connectionString))
                using (var command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@main_email", main_email);
                    command.Parameters.AddWithValue("@friend_email", friend_email);

                    conn.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        valueOfRead = reader.Read();
                    }
                    conn.Close();
                }

                return valueOfRead;
            }
            else
            {
                return true; // Man darf sich nicht als Kontakt hinzufügen
            }

        }

        /// <summary>
        /// Kontakt des Benutzers in die Datenbank hinzufügen
        /// </summary>
        /// <param name="main_email"></param>
        /// <param name="friend_email"></param>
        /// <returns>true, wenn der Kontakt hinzugefügt werden konnte.</returns>
        static internal bool AddContact(string main_email, string friend_email)
        {
            // Wenn der Kontakt noch nicht existiert, wird er hinzugefügt
            if (!AlreadyFriends(main_email, friend_email))
            {
                string query = "insert into contacts (main_email, friend_email) values (@mainemail,@friendemail);";

                using (var conn = new MySqlConnection(connectionString))
                using (var command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@mainemail", main_email);
                    command.Parameters.AddWithValue("@friendemail", friend_email);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
                return true;

            }
            else { return false; }
        }



        #endregion



    }
}
