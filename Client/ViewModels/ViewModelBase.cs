using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Client
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public event PropertyChangingEventHandler PropertyChanging;

        /* INotifyPropertyChanged schreibt das Event PropertyChanged vor. 
        * Jenes erhält als EventArgs immer den Namen der Eigenschaft, die sich geändert hat. 
        * Liegt eine Änderung vor, kann sich das entsprechende Element, das auf die Eigenschaft gebunden ist, aktualisieren und zeigt dann die neuen Daten an. 
        */
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

        }


        #region Command Helper

        /// <summary>
        /// Führt einen Befehl aus, wenn updatingFlag nicht gesetzt ist.
        /// Wenn updatingFlag = true (was anzeigt, dass eine Funktion bereits ausgeführt wird), dann wird die Aktion nicht ausgeführt
        /// Wenn updatingFlag = false, dann wird die Aktion ausgeführt
        /// Sobald die Aktion beendet ist, wird das Flag auf false zurückgesetzt
        /// </summary>
        /// <param name="updatingFlag"></param>
        /// <param name="action">Die "Action", die ausgeführt werden soll</param>
        /// <returns></returns>
        protected async Task RunCommand(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            // prüfen, ob updatingFlag = true
            if (updatingFlag.GetPropertyValue())
                return;

            // Set the property flag to true to indicate we are running
            updatingFlag.SetPropertyValue(true);

            try
            {
                // Die übergebene "Action" ausführen
                await action();
            }
            finally
            {
                // Nach beendeter Ausführung  wieder "false" setzen 
                updatingFlag.SetPropertyValue(false);
            }
        }


        #endregion
    }
}
