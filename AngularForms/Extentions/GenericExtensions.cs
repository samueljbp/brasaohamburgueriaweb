using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrasaoHamburgueria.Web
{
    public static class GenericExtensions
    {
        public static string RemoveLineEndings(this string value)
        {
            string charToReplaWith = " ";
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", charToReplaWith).Replace("\n", charToReplaWith).Replace("\r", charToReplaWith).Replace(lineSeparator, charToReplaWith).Replace(paragraphSeparator, charToReplaWith);
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}