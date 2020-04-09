using System;
using System.Diagnostics;
using System.Globalization;
using Telefonico.Core;

namespace Client
{

    /// <summary>
    /// Konvertiert <see cref="ApplicationPage"/> see zu einem view bzw. page />
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Page finden
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Login:
                    return new LoginPage();

                case ApplicationPage.Chat:
                    return new ChatPage();

                case ApplicationPage.Register:
                    return new RegisterPage();

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
