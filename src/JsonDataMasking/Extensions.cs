using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JsonDataMasking
{
    public static class Extensions
    {
        /// <summary>
        /// Convert a <c>IEnumerable</c> from any type to a generic type <c>T</c>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns><c>IEnumerable</c> with the specified type <c>T</c></returns>
        public static IEnumerable<T> ConvertToGenericType<T>(this IEnumerable items)
        {
            return items.Cast<object>().Select(value => (T) Convert.ChangeType(value, typeof(T))).ToList();
        }
    }
}
