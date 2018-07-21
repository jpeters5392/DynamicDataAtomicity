using System;
using System.Reactive.Subjects;

namespace DynamicDataAtomicity
{
    public interface ICompositeAtomic<TState>
    {
        IConnectableObservable<TState> AtomicStream { get; }
        IObservable<long> LastAtomicOperationCompleted { get; }

        void CompleteUpdateOperation();
    }
}
