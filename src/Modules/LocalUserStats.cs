using System;
using System.Collections.Generic;
using System.Text;

namespace WalthexLocalPlay.Modules
{
    public class LocalUserStats
    {
        public Dictionary<string, float> Data = new Dictionary<string, float>();

        public LocalUserStats()
        {
            Data["rank"] = 1000f;
            Data["level"] = 1f;
        }

        public bool GetStat(string pchName, out int pData)
        {
            if (!Data.TryGetValue(pchName, out float value))
            {
                pData = 0;
                return false;
            }
            pData = (int)value;
            return true;
        }
        public bool GetStat(string pchName, out float pData)
        {
            if (!Data.TryGetValue(pchName, out pData))
            {
                pData = 0;
                return false;
            }
            return true;
        }

        public bool SetStat(string pchName, int pData)
        {
            Data[pchName] = (float)pData;
            return true;
        }
        public bool SetStat(string pchName, float pData)
        {
            Data[pchName] = pData;
            return true;
        }
    }
}
