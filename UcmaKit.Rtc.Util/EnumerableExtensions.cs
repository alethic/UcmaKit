using System;
using System.Collections.Generic;
using System.Linq;

namespace ISI.Rtc.Util
{

    public static class EnumerableExtensions
    {

        /// <summary>
        /// Returns an empty enumerable if the source is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source != null ? source : Enumerable.Empty<T>();
        }

        /// <summary>
        /// Returns an enumeration of the source with the item appended.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item)
        {
            // existing
            foreach (var i in source)
                yield return i;

            // appended
            yield return item;
        }

        /// <summary>
        /// Implementation of Select which passes the previous value as well.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> SelectWithPrevious<T>(this IEnumerable<T> source, Func<T, T, T> selector)
            where T : class
        {
            T previous = null;

            foreach (var item in source)
            {
                yield return selector(item, previous);
                previous = item;
            }
        }

        /// <summary>
        /// Implementation of Select which passes the previous value as well.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectWithPreviousResult<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult, TResult> selector)
            where TResult : class
        {
            TResult previous = null;

            foreach (var item in source)
            {
                var result = selector(item, previous);
                yield return result;
                previous = result;
            }
        }

    }

}
