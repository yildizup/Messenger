using System.Windows;
using System.Windows.Media.Animation;

namespace Client
{

    /// <summary>
    /// Basisklasse für Animationen mit zwei Zuständen.
    /// </summary>
    /// <typeparam name="Parent"></typeparam>
    public abstract class AnimateBaseProperty<Parent> : BaseAttachedProperty<Parent, bool>
        where Parent : BaseAttachedProperty<Parent, bool>, new()
    {

        #region Public Properties

        public bool FirstLoad { get; set; } = true;

        #endregion

        public override void OnValueUpdated(DependencyObject sender, object value)
        {

            if (!(sender is FrameworkElement element))
                return;

            // Keine Animation, wenn der Wert gleich bleibt
            if (sender.GetValue(ValueProperty) == value && !FirstLoad)
                return;


            if (FirstLoad)
            {
                // Create a single self-unhookable event
                // for the elements Loaded event
                RoutedEventHandler onLoaded = null;
                onLoaded = (ss, ee) =>
               {
                   // Unhook ourselves
                   element.Loaded -= onLoaded;

                   // gewünschte Animation ausführen
                   DoAnimation(element, (bool)value);

                   FirstLoad = false;
               };

                // Hook into the Loaded event of the element
                element.Loaded += onLoaded;
            }

            else
                DoAnimation(element, (bool)value);


        }


        /// <summary>
        ///  Die auszuführende Methode, wenn der Wert aktualisiert wird
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        protected virtual void DoAnimation(FrameworkElement element, bool value) { }

    }



    /// <summary>
    /// Animation für Framework Elemente zum Anzeigen oder Verstecken.
    /// </summary>
    public class AnimateSlideInFromLeftProperty : AnimateBaseProperty<AnimateSlideInFromLeftProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value)
        {
            if (value)
                // Anzeigen
                element.SlideAndFadeInFromLeftAsync();
            else
                // Verstecken
                element.SlideAndFadeOutToLeftAsync();
        }


    }

}
