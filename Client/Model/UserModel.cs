using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class UserModel
    {
    }

    public class User : INotifyPropertyChanged
    {

        private string email;

        public string Email
        {
            get { return email; }

            set
            {
                if (email != value)
                {
                    email = value;
                    RaisePropertyChanged("Email");
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }


    }
}
