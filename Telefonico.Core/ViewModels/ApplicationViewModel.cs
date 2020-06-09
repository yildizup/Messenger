using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telefonico.Core
{
    public class ApplicationViewModel : BaseViewModel
    {

        private ApplicationPage currentPage;

        /// <summary>
        /// Die aktuelle Seite der Anwendung
        /// </summary>
        public ApplicationPage CurrentPage
        {
            get { return currentPage; }

            set
            {
                if (value == currentPage)
                    return;

                currentPage = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// True, wenn Side Menu angezeigt werden soll
        /// </summary>
        public bool SideMenuVisible { get; set; } = false;



    }
}
