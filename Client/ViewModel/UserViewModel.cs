using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModel
{
    public class UserViewModel
    {
        public ObservableCollection<Model.User> Users
        {
            get;set;
        } 

        public void LoadUsers()
        {
            ObservableCollection<Model.User> users = new ObservableCollection<Model.User>();
            users.Add(new Model.User { Email = "mustermann@gmail.com" });
            users.Add(new Model.User { Email = "abc@gmail.com" });

            Users = users;
        }


    }


}
