using System;
using DynamicData;

namespace DynamicDataAtomicity
{
    public interface ICountableChangeSet<TType, TKey> : IChangeSet<TType, TKey>, ICountable
    {
    }
}
