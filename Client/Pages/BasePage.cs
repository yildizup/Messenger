using System.Windows.Controls;
using Telefonico.Core;

namespace Client
{

    /// <summary>
    /// Ein base page für alle Pages
    /// </summary>
    public class BasePage<VM> : Page
        where VM : BaseViewModel, new()
    {

        /// <summary>
        /// Das ViewModel, das mit der Seite "verbunden" wird
        /// </summary>
        private VM _ViewModel;


        /// <summary>
        /// Das ViewModel, das mit der Seite "verbunden" wird
        /// </summary>
        public VM ViewModel
        {
            get { return _ViewModel; }
            set
            {
                // If nothing has changed, return
                if (_ViewModel == value)
                    return;

                // Update the value
                _ViewModel = value;

                // Set the data context for this page
                this.DataContext = _ViewModel;
            }
        }

        #region Konstruktor

        public BasePage()
        {
            // Standard View erstellen
            this.ViewModel = new VM();
        }

        #endregion


    }
}
