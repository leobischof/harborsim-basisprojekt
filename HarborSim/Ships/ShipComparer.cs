using System;
using System.Collections.Generic;

namespace HarborSim
{
    public class ShipComparer : IComparer<Ship>
    {
        public int Compare(Ship x, Ship y)
        {
            int result = x.Length.CompareTo(y.Length);
            if (result != 0)
            {
                return result;
            }
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
}
