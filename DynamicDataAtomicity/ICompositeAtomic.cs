using System;

namespace DynamicDataAtomicity
{
    public interface ICompositeAtomic<TState>
    {
        IObservable<TState> AtomicStream { get; }
        IObservable<long> LastAtomicOperationCompleted { get; }

        void CompleteUpdateOperation();
    }
}
