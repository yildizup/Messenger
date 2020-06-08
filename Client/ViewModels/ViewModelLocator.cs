using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telefonico.Core;

namespace Client
{
    /// <summary>
    /// Findet ViewModels vom IoC, um diese in den Xaml Dateien zu binden
    /// </summary>
    public class ViewModelLocator
    {

        /// <summary>
        /// Singleton Instanz des Locators
        /// </summary>
        public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();

        public static ApplicationViewModel ApplicationViewModel => IoCContainer.Get<ApplicationViewModel>();

    }
}
