using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
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
    }
}
