using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DynamicDataAtomicity.ObservableExtensions
{
    public static class Observables
    {
        public static IObservable<IList<TType>> AtomicBuffer<TType>(this IObservable<TType> source, IObservable<long> atomic)
        {
            return source.Buffer(atomic);
        }
    }
}
