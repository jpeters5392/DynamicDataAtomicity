using System;
namespace DynamicDataAtomicity
{
    public interface ICountable
    {
        int ChangeCount { get; }
    }
}
