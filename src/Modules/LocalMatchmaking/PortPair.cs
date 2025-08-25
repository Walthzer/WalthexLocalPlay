using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WalthexLocalPlay.Modules.LocalMatchmaking
{
    public class PortPair
    {
        public int tcp { get; private set; }
        public int udp { get; private set; }


        public PortPair(int tcp, int udp)
        {
            this.tcp = tcp;
            this.udp = udp;
        }
    }
}
