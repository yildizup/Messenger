using Ninject;
using System;
using System.Diagnostics;
using System.Globalization;
using Telefonico.Core;

namespace Client
{

    /// <summary>
    /// Konvertiert ein String zu einem "Service"
    /// </summary>
    public class IoCConverter : BaseValueConverter<IoCConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Page finden
            switch ((string)parameter)
            {
                //TODO: Verbessern
                case nameof(ApplicationViewModel):
                    return IoCContainer.Get<ApplicationViewModel>();

                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }



    }
}
