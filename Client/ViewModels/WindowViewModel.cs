using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class WindowViewModel : ViewModelBase
    {

        /// <summary>
        /// Aktuelles "Page" der Anwendung
        /// (Vorerst Default LoginPage)
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;




    }
}
