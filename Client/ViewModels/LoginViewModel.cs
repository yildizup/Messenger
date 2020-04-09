using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels
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
            LoginCommand = new RelayParameterizedCommand(async (parameter) => await Login(parameter));


        }

        /// <summary>
        /// Versucht den Benutzer anzumelden
        /// </summary>
        /// <param name="parameter">Das Passwort des Users, welcher vom View übergeben wird </param>
        /// <returns></returns>
        public async Task Login(object parameter)
        {
            await Task.Delay(500);


        }
    }
}
