using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Telefonico.Core
{
    /// <summary>
    /// Helper für expressions
    /// </summary>
    public static class ExpressionHelpers
    {

        /// <summary>
        /// Kompiliert eine "Expression" und erhält den Rückgabewert der Funktion
        /// </summary>
        /// <typeparam name="T">Typ des Rückgabewertes</typeparam>
        /// <param name="lamba">zu kompilierende Expression</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this Expression<Func<T>> lamba)
        {
            return lamba.Compile().Invoke();
        }

        /// <summary>
        /// Setzt den zugrunde liegenden "Property" Wert auf einen Wert von einer "Expression" die den "Property" beinhält
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lamba">Die "expression"</param>
        /// <param name="value">Der Wert, der gesetzt werden Soll</param>
        public static void SetPropertyValue<T>(this Expression<Func<T>> lamba, T value)
        {
            //TODO: Reflections anschauen

            // Konvertiert ein lambda () => ein.Property, zu ein.Property (lambda wird gewissermaßen entfernt)
            var expression = (lamba as LambdaExpression).Body as MemberExpression;

            // Nimmt den Inhalt des Propertys
            var propertyInfo = (PropertyInfo)expression.Member;
            var target = Expression.Lambda(expression.Expression).Compile().DynamicInvoke();

            // Den Wert setzen
            propertyInfo.SetValue(target, value);

        }
    }
}
