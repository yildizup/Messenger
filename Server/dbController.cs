using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

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

        static internal void CreateUser(string email, string password)
        {
            MySqlCommand insertCommand = new MySqlCommand("insert into user (email,password) values(@email,@password)");
            insertCommand.Parameters.AddWithValue("@email", email);
            insertCommand.Parameters.AddWithValue("@password", password);
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
        static internal bool CreateUserAndCheck(string email, string password)
        {
            // Wenn ein User nicht existiert, kann ein Konto erstellt werden
            if (!DoesUserExist(email))
            {
                CreateUser(email, password);
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

        //TODO: Recherchieren über Vor- und Nachteile von Events in einer static Class


        static internal List<string> LoadContacts(string email)
        {
            List<string> listContacts = new List<string>();


            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select * from contacts where main_email=@email"; // Abfrage nach allen Kontakten des Users
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
                listContacts.Add(row["friend_email"].ToString());
            }

            return listContacts;
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


        #endregion


        #region Datensätze speichern


        static internal void SaveMessage(string main_email, string friend_email, string message)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "insert into chat (main_email, friend_email, message, thetime) values (@mainemail,@friendemail,@message, now() );";
            cmd.Parameters.AddWithValue("@mainemail", main_email);
            cmd.Parameters.AddWithValue("@friendemail", friend_email);
            cmd.Parameters.AddWithValue("@message", message);
            cmd.Connection = con;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

        }



        #endregion


    }
}
