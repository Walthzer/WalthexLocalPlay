using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WalthexLocalPlay.Modules.LocalMatchmaking
{
    public class PortRange
    {
        public int low { get; private set; }
        public int high { get; private set; }


        public PortRange(int nlow, int nhigh)
        {
            this.low = Math.Min(nlow, nhigh);
            this.high = Math.Max(nlow, nhigh);
        }

        public int Size()
        {
            return Math.Abs(high - low);
        }

        public IEnumerable<int> Ports()
        {
            return Enumerable.Range(low, Size()+1).ToArray();
        }

        public PortRange NextRange()
        {
            return new PortRange(high + 1, high + Size() + 1);
        }
    }
}
