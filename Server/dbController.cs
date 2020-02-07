using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Server
{
    static class dbController
    {

        internal static MySqlConnection con;  // wird verwendet, um eine Verbindung mit der Datenbank herzustellen.
        internal static string connectionString;

        static dbController()
        {
            //Datenbank Verbindung
            connectionString = @"host=127.0.0.1;user=root;database=telefonico";
            con = new MySqlConnection(connectionString);
        }

        #region Benutzer erstellen und Email prüfen

        static internal void CreateUser(string email, string password)
        {
            #region Befehl
            MySqlCommand insertCommand = new MySqlCommand("insert into user (email,password) values(@email,@password)");
            insertCommand.Parameters.AddWithValue("@email", email);
            insertCommand.Parameters.AddWithValue("@password", password);
            insertCommand.Connection = con;
            #endregion

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

            #region Abfrage

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select * from user where email=@email";
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Connection = con;
            #endregion

            con.Open();

            MySqlDataReader check = cmd.ExecuteReader();
            bool valueOfRead = check.Read();
            check.Close();

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
            // Wenn der user nicht existiert, kann ein Konto erstellt werden
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
        #endregion

        #region Benutzeranmeldung


        //TODO: gibt es alternativen zu enums für verschiedene Fehlermeldungen ?
        static internal void Login(string email, string password)
        {

            // Wenn User nicht existiert
            if (!DoesUserExist(email))
            {
                // Rückmeldung, dass der Benutzer nicht existiert

            }
            else
            {
                //Prüfe Passwort Übereinstimmung

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
            #region Abfrage

            MySqlCommand cmd = new MySqlCommand();

            cmd.CommandText = "Select password from user where email=@email";
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Connection = con;
            #endregion

            con.Open();

            MySqlDataReader dr = cmd.ExecuteReader();

            // In ein 'DataTable' Objekt schreiben.
            DataTable dt = new DataTable();
            dt.Load(dr);

            // eingegebenes Passwort mit der Datenbank vergleichen
            if (password == (string)dt.Rows[0][0])
            {
                Console.WriteLine("Passwort ist richtig");
                return true;
            }
            else
            {
                Console.WriteLine("Passwort ist falsch");
                return false;
            }

        }

        #endregion
    }
}
