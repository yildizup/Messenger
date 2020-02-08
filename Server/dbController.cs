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

            #endregion
        }

        //TODO: Recherchieren über Vor- und Nachteile von Events in einer static Class


    }
}
