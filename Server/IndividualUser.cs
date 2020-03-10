namespace Server
{
    public class IndividualUser
    {
        public string email;
        public string password;
        public bool status; //ist der Benutzer eingeloggt ?

        public SClient Connection; //Um die jeweiligen Clients anzusprechen


    }
}
