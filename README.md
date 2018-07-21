# DynamicDataAtomicity
This project allows you to leverage DynamicData's `IObservableCache<T, K>` as well as standard `IObservable<T>` for multiple properties on your application state object, yet allow for atomic updates to some or all of those properties.

## Example
Consider the following example with the given application state object.
```
public class TestState
{
    public IObservableCache<TestStateType1, Guid> Data1 { get; }

    public IObservableCache<TestStateType2, Guid> Data2 { get; }

    public IObservable<TestStateType1> Data3 { get; }
}
```

If your application needs to perform a single action that involves updates to both Data1 and Data3 then any observable subscriptions that watch for updates will fire for each discrete update, not when the cross-property update is complete.

## Setting up your state
This library introduces a `CompositeAtomic<T>` abstract class as well as an `ICompositeAtomic<T>` interface if you would rather not have your state extend from a base class.  This adds a new observable to your state that tracks when a discrete action completes, as well as a connectable observable that emits the combined state updates that happen in between each action completion.

The basic gist is that you will need to create a second state object that represents the aggregated updates for an individual action.  Each property from your normal state object should have a collection property in this "AtomicStream" version of the state with `IObservableCache<T, K>` updates mapping to `ICountableChangeSet<T, K>` and `IObservable<T>` updates mapping to `ICountableList<T>`.  (We need the countable variants to unify checking the count of updates across changesets and collections)
The example below shows what this will look like.

```
public class TestState : CompositeAtomic<TestStateAtomicStream>
{
    private ISourceCache<TestStateType1, Guid> Data1Cache { get; }
    public IObservableCache<TestStateType1, Guid> Data1 => Data1Cache.AsObservableCache();

    private ISourceCache<TestStateType2, Guid> Data2Cache { get; }
    public IObservableCache<TestStateType2, Guid> Data2 => Data2Cache.AsObservableCache();

    private ISubject<TestStateType1> Data3Scalar { get; }
    public IObservable<TestStateType1> Data3 => Data3Scalar.AsObservable();

    public TestState()
    {
        Data2Cache = new SourceCache<TestStateType2, Guid>(i => i.Id);
        Data1Cache = new SourceCache<TestStateType1, Guid>(i => i.Id);
        Data3Scalar = new Subject<TestStateType1>();
    }
}

public class TestStateAtomicStream
{
    public ICountableChangeSet<TestStateType1, Guid> Data1 { get; internal set; }

    public ICountableChangeSet<TestStateType2, Guid> Data2 { get; internal set; }

    public ICountableList<TestStateType1> Data3 { get; internal set; }
}
```

## Buffering the state updates
All of the logic to buffer and batch updates together needs to go in to the creation of the `AtomicStream` observable type.

```
AtomicStream = Observable.Create<TestStateAtomicStream>(observer =>
{
    // grab cold references to AtomicBuffers for all of your data properties
    var data1Updates = this.Data1.AtomicBuffer(LastAtomicOperationCompleted);
    var data2Updates = this.Data2.AtomicBuffer(LastAtomicOperationCompleted);
    var data3Updates = this.Data3.AtomicBuffer(LastAtomicOperationCompleted);

    // and then zip them together with the LastAtomicOperationCompleted observable so they only fire when the atomic action completes
    return Observable.Zip(LastAtomicOperationCompleted, data1Updates, data2Updates, data3Updates,
                          (lastOperation, data1, data2, data3) => (Data1Changes: data1, Data2Changes: data2, Data3Changes: data3, LastOperation: lastOperation))
                     .DistinctUntilChanged(x => x.LastOperation)
                     .Select(CreateStateUpdates)
                     .Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted);
}).Publish();

// this creates your `AtomicStream` data type
private TestStateAtomicStream CreateStateUpdates((IList<IChangeSet<TestStateType1, Guid>> Data1Changes, IList<IChangeSet<TestStateType2, Guid>> Data2Changes, IList<TestStateType1> Data3Changes, long LastOperation) args) => 
        new TestStateAtomicStream { Data1 = args.Data1Changes.CombineChangeSets(), Data2 = args.Data2Changes.CombineChangeSets(), Data3 = args.Data3Changes.ToCountableList() };
```

## Responding to state updates
There is a new WhenAny variant that allows you to subscribe to state changes, but only when there were changes to one or more properties that you care about.

```
var testState = new TestState();
// this observable will emit only when the atomic updates include changes to Data1 OR Data3, but not any other properties.
var testSubscription = testState.AtomicStream.WhenAnyCountable(x => x.Data1, x => x.Data3).Subscribe(observer);
var atomicStateSubscription = testState.AtomicStream.Connect();
```
