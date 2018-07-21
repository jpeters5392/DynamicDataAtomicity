using System;
using System.Collections.Generic;

namespace DynamicDataAtomicity
{
    public class CountableList<TType> : List<TType>, ICountableList<TType>
    {
        public int ChangeCount => Count;

        public CountableList(IEnumerable<TType> source) : base(source)
        {
        }
    }
}
