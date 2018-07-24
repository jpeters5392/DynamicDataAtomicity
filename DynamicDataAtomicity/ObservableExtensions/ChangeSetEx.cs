using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;

namespace DynamicDataAtomicity.ObservableExtensions
{
    public static class ChangeSetEx
    {
        public static IChangeSet<TType, TKey> CombineChangeSets<TType, TKey>(this IEnumerable<IChangeSet<TType, TKey>> changeSets)
        {
            return new ChangeSet<TType, TKey>(changeSets.SelectMany(changes => changes));
        }

        public static IChangeSet<TType> CombineChangeSets<TType>(this IEnumerable<IChangeSet<TType>> changeSets)
        {
            return new ChangeSet<TType>(changeSets.SelectMany(changes => changes));
        }
    }
}
