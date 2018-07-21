using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DynamicDataAtomicity
{
    public abstract class CompositeAtomic<TState> : ICompositeAtomic<TState>
    {
        private Subject<long> _lastAtomicOperationCompleted = new Subject<long>();
        public IObservable<long> LastAtomicOperationCompleted => _lastAtomicOperationCompleted.AsObservable();

        public IConnectableObservable<TState> AtomicStream { get; protected set; }

        protected CompositeAtomic()
        {
        }

        public void CompleteUpdateOperation()
        {
            _lastAtomicOperationCompleted.OnNext(DateTimeOffset.UtcNow.Ticks);
        }
    }
}
