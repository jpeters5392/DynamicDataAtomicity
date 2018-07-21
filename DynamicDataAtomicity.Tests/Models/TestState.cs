using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using DynamicData;
using DynamicDataAtomicity;
using DynamicDataAtomicity.ObservableExtensions;

namespace DynamicDataAtomicity.Tests.Models
{
    public class TestState : CompositeAtomic<TestStateAtomicStream>
    {
        internal ISourceCache<TestStateType1, Guid> Data1Cache { get; }
        public IObservableCache<TestStateType1, Guid> Data1 => Data1Cache.AsObservableCache();

        internal ISourceCache<TestStateType2, Guid> Data2Cache { get; }
        public IObservableCache<TestStateType2, Guid> Data2 => Data2Cache.AsObservableCache();

        internal ISubject<TestStateType1> Data3Scalar { get; }
        public IObservable<TestStateType1> Data3 => Data3Scalar.AsObservable();

        public TestState() : base()
        {
            Data2Cache = new SourceCache<TestStateType2, Guid>(i => i.Id);
            Data1Cache = new SourceCache<TestStateType1, Guid>(i => i.Id);
            Data3Scalar = new Subject<TestStateType1>();

            AtomicStream = Observable.Create<TestStateAtomicStream>(observer =>
            {
                var data1Updates = this.Data1.AtomicBuffer(LastAtomicOperationCompleted);
                var data2Updates = this.Data2.AtomicBuffer(LastAtomicOperationCompleted);
                var data3Updates = this.Data3.AtomicBuffer(LastAtomicOperationCompleted);

                return Observable.Zip(LastAtomicOperationCompleted, data1Updates, data2Updates, data3Updates,
                                      (lastOperation, data1, data2, data3) => (Data1Changes: data1, Data2Changes: data2, Data3Changes: data3, LastOperation: lastOperation))
                                 .DistinctUntilChanged(x => x.LastOperation)
                                 .Select(CreateStateUpdates)
                                 .Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted);
            }).Publish();
        }

        private TestStateAtomicStream CreateStateUpdates((IList<IChangeSet<TestStateType1, Guid>> Data1Changes, IList<IChangeSet<TestStateType2, Guid>> Data2Changes, IList<TestStateType1> Data3Changes, long LastOperation) args) => 
            new TestStateAtomicStream { Data1 = args.Data1Changes.CombineChangeSets(), Data2 = args.Data2Changes.CombineChangeSets(), Data3 = args.Data3Changes.ToCountableList() };

        public void UpdateCacheProperty<TDataType, TKeyType>(Expression<Func<TestState, ISourceCache<TDataType, TKeyType>>> selector, Action<ISourceUpdater<TDataType, TKeyType>> updater)
        {
            var prop = (PropertyInfo)((MemberExpression)selector.Body).Member;
            var propValue = (ISourceCache<TDataType, TKeyType>)prop.GetValue(this);

            propValue.Edit(updater);
        }

        public void UpdateScalarProperty<TDataType>(Expression<Func<TestState, ISubject<TDataType>>> selector, TDataType newValue)
        {
            var prop = (PropertyInfo)((MemberExpression)selector.Body).Member;
            var propValue = (ISubject<TDataType>)prop.GetValue(this);

            propValue.OnNext(newValue);
        }
    }
}
