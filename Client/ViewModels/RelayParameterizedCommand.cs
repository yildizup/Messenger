using System;
using System.Windows.Input;


namespace Client
{
   /// <summary>
    /// Ein Befehl, der eine Aktion ausführt
    /// </summary>
    public class RelayParameterizedCommand : ICommand
    {
        #region Private Members

        /// <summary>
        /// Die Aktion, die ausgeführt werden soll
        /// </summary>
        private Action<object> _Action;

        #endregion

        #region Public Events

        /// <summary>
        /// Das Ereignis, das ausgelöst wird, wenn sich der <see cref="CanExecute(object)"/> Wert geändert hat
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        public RelayParameterizedCommand(Action<object> action)
        {
            _Action = action;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Ein relay Command kann immer ausgeführt werden
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _Action(parameter);
        }

        #endregion
    }}
