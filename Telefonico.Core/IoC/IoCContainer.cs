using Ninject;
using System;

namespace Telefonico.Core
{

    /// <summary>
    /// Der IoC Container der Anwendung 
    /// </summary>
    public static class IoCContainer
    {

        /// <summary>
        /// Der Kernel für den IoC Container
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();
 
        #region Konstruktion
        /// <summary>
        /// "Bindet" alle beötigten Informationen
        /// Anmerkung: Muss aufgerufen werden, sobald die Anwendung startet
        /// </summary>
        public static void Setup()
        {
            BindViewModels();
        }

        // "Bindet" alle singleton ViewModels
        static void BindViewModels()
        {
            // "Bindet" zu einer einzigen Instanz von ApplicationViewModel
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
        }

        #endregion

        /// <summary>
        /// Kriegt ein Service vom IoC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }
    }
}
