using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    // TODO: Recherchieren nach einer besseren Lösung

    static class UserController
    {
        public static List<IndividualUser> individualUsers = new List<IndividualUser>();

        static internal void LoadUsers()
        {
            DataTable dt = dbController.LoadUsers();

            foreach (DataRow row in dt.Rows)
            {
                IndividualUser individualUser = new IndividualUser();

                individualUser.email = row["email"].ToString();
                individualUser.password = row["password"].ToString();

                individualUsers.Add(individualUser);
            }

        }



    }
}
