using System.Windows;

namespace Client
{
    public class WindowViewModel : ViewModelBase
    {

        #region Private Variablen
        /// <summary>
        /// Der Window, der von diesem ViewModel kontrolliert wird
        /// </summary>
        private Window _Window;
        #endregion

        #region public Eigenschaften

        #endregion


        #region Konstruktoren
        public WindowViewModel(Window window)
        {
            _Window = window;
        }


        #endregion


        /// <summary>
        /// Aktuelles "Page" der Anwendung
        /// (Vorerst Default LoginPage)
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;




    }
}
