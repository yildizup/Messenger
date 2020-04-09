using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telefonico.Core
{
    public class ApplicationViewModel : ViewModelBase
    {

        /// <summary>
        /// Die aktuelle Seite der Anwendung
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;

    }
}
