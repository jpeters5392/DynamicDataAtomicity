using System;
using System.Collections.Generic;
using DynamicData;

namespace DynamicDataAtomicity
{
    public class CountableChangeSet<TType, TKey> : ChangeSet<TType, TKey>, ICountableChangeSet<TType, TKey>
    {
        public int ChangeCount => Count;

        public CountableChangeSet(IEnumerable<Change<TType, TKey>> items) : base(items)
        {
        }
    }
}
