using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicDataAtomicity;

namespace DynamicDataAtomicity.ObservableExtensions
{
    public static class ChangeSets
    {
        public static ICountableChangeSet<TType, TKey> CombineChangeSets<TType, TKey>(this IEnumerable<IChangeSet<TType, TKey>> changeSets)
        {
            return new CountableChangeSet<TType, TKey>(changeSets.SelectMany(changes => changes));
        }

        public static IObservable<IList<IChangeSet<TType, TKey>>> AtomicBuffer<TType, TKey>(this IObservableCache<TType, TKey> source, IObservable<long> atomic)
        {
            return source.Connect().Buffer(atomic);
        }
    }
}
