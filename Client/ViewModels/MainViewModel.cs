using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private string _email;



        public string Email
        {
            set
            {
                _email = value;
            }
        }

        public MainViewModel()
        {
        }


    }

}
