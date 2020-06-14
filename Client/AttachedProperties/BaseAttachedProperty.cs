using System;
using System.Runtime.Remoting.Channels;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Eine "base attached property", um die "Vanilla attached property" von WPF zu ersetzen
    /// </summary>
    /// <typeparam name="Parent"></typeparam>
    /// <typeparam name="Property"></typeparam>
    public abstract class BaseAttachedProperty<Parent, Property>
        where Parent : BaseAttachedProperty<Parent, Property>, new()
    {
        #region Public Events


        /// <summary>
        /// wird ausgelöst, wenn ein Wert sich ändert
        /// </summary>
        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (IChannelSender, e) => { };

        /// <summary>
        /// wird ausgelöst, wenn ein Wert sich ändert, auch wenn der Wert gleich bleibt
        /// </summary>
        public event Action<DependencyObject, object> ValueUpdated = (sender, value) => { };

        #endregion

        #region Public Properties

        /// <summary>
        /// Eine singleton Instanz der Parent Klasse
        /// </summary>
        public static Parent Instance { get; private set; } = new Parent();


        #endregion

        #region Attached Property Definition


        /// <summary>
        /// Der Attached Property für diese Klasse
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value",
            typeof(Property),
            typeof(BaseAttachedProperty<Parent, Property>),
            new UIPropertyMetadata(
                default(Property),
                new PropertyChangedCallback(OnValuePropertyChanged),
                new CoerceValueCallback(OnValuePropertyUpdated)
                ));


        /// <summary>
        /// Callback Event, wenn <see cref="ValueProperty"/> sich geändert hat, auch wenn der Wert gleich bleibt
        /// </summary>
        /// <param name="d">UI Element dessen Property sich geändert hat</param>
        /// <param name="e"></param>
        private static object OnValuePropertyUpdated(DependencyObject d, object value)
        {

            // Parent Funktion aufrufen
            Instance.OnValueUpdated(d, value);

            // Event Listener aufrufen
            Instance.ValueUpdated(d, value);

            return value;
        }


        /// <summary>
        /// Callback Event, wenn <see cref="ValueProperty"/> sich geändert hat
        /// </summary>
        /// <param name="d">UI Element dessen Property sich geändert hat</param>
        /// <param name="e"></param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            // Parent Funktion aufrufen
            Instance.OnValueChanged(d, e);

            // Event Listener aufrufen
            Instance.ValueChanged(d, e);
        }



        /// <summary>
        /// Gibt den "attached property" zurück. 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Property GetValue(DependencyObject d) => (Property)d.GetValue(ValueProperty);
        // dasselbe wie:
        //public static Property GetValue(DependencyObject d)
        //{
        //    return (Property)d.GetValue(ValueProperty);
        //}


        /// <summary>
        /// Setzt den "attached property"
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        public static void SetValue(DependencyObject d, Property value) => d.SetValue(ValueProperty, value);


        #endregion

        #region Event Methoden


        /// <summary>
        /// Wird aufgerufen, wenn der Wert eines "attached Propertys" sich ändert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// Wird aufgerufen, wenn der Wert eines "attached Propertys" sich ändert, auch wenn der Wert gleich bleibt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnValueUpdated(DependencyObject sender, object value) { }

        #endregion
    }
}
