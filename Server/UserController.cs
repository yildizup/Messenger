﻿using System.Collections.Generic;
using System.Data;

namespace Server
{
    // TODO: Recherchieren nach einer besseren Lösung

    static class UserController
    {
        public static List<IndividualUser> ConnectedUsers = new List<IndividualUser>();

        static internal void LoadUsers()
        {
            DataTable dt = dbController.LoadUsers();

            foreach (DataRow row in dt.Rows)
            {
                IndividualUser individualUser = new IndividualUser();

                individualUser.email = row["email"].ToString();
                individualUser.password = row["password"].ToString();

                ConnectedUsers.Add(individualUser);
            }

        }

        static internal IndividualUser FindUser(string mail)
        {
            IndividualUser user = ConnectedUsers.Find(i => i.email == mail); //TODO: Recherchieren über Lambda Expressions
            return user;
        }

        static internal int GetIndexOfUser(string mail)
        {
            int index = ConnectedUsers.FindIndex(i => i.email == mail); 
            return index;
        }


    }
}
