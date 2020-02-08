using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace Server
{
    public class IndividualUser
    {
        public string email;
        public string password;
        public bool LoggedIn; //ist der Benutzer eingeloggt ?

        public SClient Connection; //Um die jeweiligen Clients anzusprechen






    }
}
