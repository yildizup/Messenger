using System.Windows.Controls;
using Telefonico.Core;

namespace Client
{

    /// <summary>
    /// A base page for all pages to gain basic functionality
    /// </summary>
    public class BasePage<VM> : Page
        where VM : BaseViewModel, new()
    {

        /// <summary>
        /// The ViewModel which is binded to the View
        /// </summary>
        private VM _ViewModel;


        /// <summary>
        /// The ViewModel which is binded to the View
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
