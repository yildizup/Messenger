using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Client
{
    /// <summary>
    /// Ein Basis value Convertert mit direkter XAML Bneutzung
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {

        /// <summary>
        /// Eine einzige Instanz dieses Konverters 
        /// </summary>
        private static T _Converter = null;


        #region Markup Extension Methoden
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            /* Dasselbe wie:
             * if (_Converter == null)
             *  _Converter = new T();
             *  return _Converter
             */

            return _Converter ?? (_Converter = new T());
        }
        #endregion



        #region Value Converter Methoden
        /// <summary>
        /// Zum Konvertieren eines Types zu einem anderen Typ
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);


        /// <summary>
        /// Zum eingentlichen Typ zurück konvertieren
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        #endregion
    }
}
