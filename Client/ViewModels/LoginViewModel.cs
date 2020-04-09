using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    public class LoginViewModel : ViewModelBase
    {

        #region public Eigenschaften

        /// <summary>
        /// Email des Benutzers
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Passwort des Benutzers.
        /// (Wird nicht benötigt, da man nicht daran binden kann)
        /// </summary>
        public string Password { get; set; }

        #endregion

        /// <summary>
        /// Command um sich anzumelden
        /// </summary>
        public ICommand LoginCommand { get; set; }


        public LoginViewModel()
        {
            // Erstellt ein Command
            LoginCommand = new RelayCommand(Login);


        }

        /// <summary>
        /// Versucht den Benutzer anzumelden
        /// </summary>
        /// <param name="parameter">Das Passwort des Users, welcher vom View übergeben wird </param>
        /// <returns></returns>
        public void Login()
        {
            var email = this.Email;
            var password = this.Password;

        }
    }
}
