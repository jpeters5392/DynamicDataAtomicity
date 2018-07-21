using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicDataAtomicity;

namespace DynamicDataAtomicity.ObservableExtensions
{
    public static class AtomicStream
    {
        /// <summary>
        /// Returns the source observable when any of the supplied countable properties have a non-zero length
        /// </summary>
        /// <returns>The source observable.</returns>
        /// <param name="source">The observable to filter based on countable properties.</param>
        /// <param name="selectors">The property selectors containing the desired countable properties.</param>
        /// <typeparam name="TState">The type of the object from the source observable.</typeparam>
        public static IObservable<TState> WhenAnyCountable<TState>(this IObservable<TState> source, params Func<TState, ICountable>[] selectors)
        {
            return source.Where(x =>
            {
                return selectors.Any(selector => selector(x).ChangeCount > 0);
            });
        }
    }
}
