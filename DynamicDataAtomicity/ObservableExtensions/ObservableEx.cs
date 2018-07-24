using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData;

namespace DynamicDataAtomicity.ObservableExtensions
{
    public static class ObservableEx
    {
        public static IObservable<IChangeSet<TType>> AsChangeSet<TType>(this IObservable<TType> source, TType initialValue = default(TType))
        {
            var previousValue = initialValue;
            return Observable.Merge(
                source.Select(item =>
                {
                    previousValue = item;
                    var change = new Change<TType>(ListChangeReason.Add, item, 0);
                    return new ChangeSet<TType>(new List<Change<TType>> { change });
                })
                .Take(1),
                source.Select(item =>
                {
                    var update = new Change<TType>(ListChangeReason.Replace, item, previousValue);
                    previousValue = item;
                    return new ChangeSet<TType>(new List<Change<TType>> { update });
                })
                .Skip(1)
            );
        }
    }
}
