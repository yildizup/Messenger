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

        /// <summary>
        /// Gibt an, ob Login noch ausgeführt wird
        /// </summary>
        public bool LoginIsRunning { get; set; }

        #endregion

        /// <summary>
        /// Command um sich anzumelden
        /// </summary>
        public ICommand LoginCommand { get; set; }


        public LoginViewModel()
        {
            // Erstellt ein Command
            LoginCommand = new RelayCommand(async () => await Login());


        }

        /// <summary>
        /// Versucht den Benutzer anzumelden
        /// </summary>
        /// <returns></returns>
        public async Task Login()
        {

            if (LoginIsRunning)
                return; //Mache garnichts

            try
            {
                LoginIsRunning = true;
                await Task.Delay(3000); 
                /* Task.Delay(3000) simuliert den Server, der die Daten noch verarbeitet. Wenn der Server noch die Daten verarbeitet, kann der User den Button nicht mehrmals verwenden,
                 * deswegen wird LoginIsRunning verwendet. Solange LoginIsRunning = true ist, wird nicht erneut eine Verbindung aufgebaut.
                 */

                var email = this.Email;
                var password = this.Password;
            }
            catch { }
            finally
            {

                LoginIsRunning = false;
            }

        }
    }
}
