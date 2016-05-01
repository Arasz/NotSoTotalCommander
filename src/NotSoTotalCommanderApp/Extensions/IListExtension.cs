using System;
using System.Collections.Generic;
using System.Linq;

namespace NotSoTotalCommanderApp.Extensions
{
    public static class IListExtension
    {
        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified <paramref name="predicate"/>. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns> Removed items count </returns>
        /// <exception cref="Exception"> A delegate callback throws an exception. </exception>
        public static int RemoveAll<T>(this IList<T> list, Predicate<T> predicate)
        {
            var itemsToRemove = list.Where(arg => predicate(arg)).ToList();
            itemsToRemove.ForEach(item => list.Remove(item));
            return itemsToRemove.Count();
        }
    }
}