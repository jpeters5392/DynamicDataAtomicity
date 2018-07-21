using System;
using System.Collections.Generic;
using DynamicDataAtomicity;

namespace DynamicDataAtomicity.ObservableExtensions
{
    public static class Enumerables
    {
        public static ICountableList<TType> ToCountableList<TType>(this IEnumerable<TType> source)
        {
            return new CountableList<TType>(source);
        }
    }
}
