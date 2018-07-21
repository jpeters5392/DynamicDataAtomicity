using System;
using System.Collections.Generic;

namespace DynamicDataAtomicity
{
    public interface ICountableList<TType> : IList<TType>, ICountable
    {
    }
}
