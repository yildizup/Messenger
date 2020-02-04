using System;
using System.Collections.Generic;
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


    }
}
