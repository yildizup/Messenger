using SharedClass;
using System.Collections.Generic;
using System.Data;

namespace Server
{
    // TODO: Recherchieren nach einer besseren Lösung

    static class UserController
    {
        public static List<User> ConnectedUsers = new List<User>();

        static internal void LoadUsers()
        {
            DataTable dt = dbController.LoadUsers();

            foreach (DataRow row in dt.Rows)
            {
                User individualUser = new User();
                individualUser.Email = row["email"].ToString();
                ConnectedUsers.Add(individualUser);
            }

        }

        static internal User FindUser(string mail)
        {
            User user = ConnectedUsers.Find(i => i.Email == mail); //TODO: Recherchieren über Lambda Expressions
            return user;
        }

        static internal int GetIndexOfUser(string mail)
        {
            int index = ConnectedUsers.FindIndex(i => i.Email == mail);
            return index;
        }


    }
}
