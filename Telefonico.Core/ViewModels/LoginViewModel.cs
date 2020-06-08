using System.Threading.Tasks;
using System.Windows.Input;

namespace Telefonico.Core
{
    public class LoginViewModel : BaseViewModel
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

        /// <summary>
        /// Command um sich zu registrieren
        /// </summary>
        public ICommand RegisterCommand { get; set; }

        public LoginViewModel()
        {
            // Erstellt ein Command
            LoginCommand = new RelayCommand(async () => await LoginAsync());
            RegisterCommand = new RelayCommand(async () => await RegisterAsync());
        }

        /// <summary>
        /// Versucht den Benutzer anzumelden
        /// </summary>
        /// <returns></returns>
        public async Task LoginAsync()
        {
            await RunCommand(() => LoginIsRunning, async () =>
            {
                await Task.Delay(3000);
                /* Task.Delay(3000) simuliert den Server, der die Daten noch verarbeitet. Wenn der Server noch die Daten verarbeitet, kann der User den Button nicht mehrmals verwenden,
                 * deswegen wird LoginIsRunning verwendet. Solange LoginIsRunning = true ist, wird nicht erneut eine Verbindung aufgebaut.
                 */

                var email = this.Email;
                var password = this.Password;
            });
        }

        /// <summary>
        /// Registrierenpage öffnen
        /// </summary>
        /// <returns></returns>
        public async Task RegisterAsync()
        {
            IoCContainer.Get<ApplicationViewModel>().CurrentPage = ApplicationPage.Register;
            await Task.Delay(1);
        }


    }
}
