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

        // Um eine Verbindung mit der Datenbank herzustellen.
        static internal void ConnectToLocalDb()
        {
            using (con)
            {
                try
                {
                    con.Open();

                    //hierhin kommen weitere Befehle.
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        //zum schliessen der Verbindung
        static internal void DB_Disconnect()
        {
            con.Close();
        }


    }
}
