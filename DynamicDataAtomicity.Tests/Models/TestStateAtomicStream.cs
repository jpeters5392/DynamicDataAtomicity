using System;
using DynamicDataAtomicity;

namespace DynamicDataAtomicity.Tests.Models
{
    public class TestStateAtomicStream
    {
        public ICountableChangeSet<TestStateType1, Guid> Data1 { get; internal set; }

        public ICountableChangeSet<TestStateType2, Guid> Data2 { get; internal set; }

        public ICountableList<TestStateType1> Data3 { get; internal set; }
    }
}
