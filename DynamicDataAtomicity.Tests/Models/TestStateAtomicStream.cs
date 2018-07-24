using System;
using DynamicData;

namespace DynamicDataAtomicity.Tests.Models
{
    public class TestStateAtomicStream
    {
        public IChangeSet<TestStateType1, Guid> Data1 { get; internal set; }

        public IChangeSet<TestStateType2, Guid> Data2 { get; internal set; }

        public IChangeSet<TestStateType1> Data3 { get; internal set; }
    }
}
