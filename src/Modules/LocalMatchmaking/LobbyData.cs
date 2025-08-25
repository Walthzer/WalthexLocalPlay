using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace WalthexLocalPlay.Modules.LocalMatchmaking;

public class LobbyData
{
    public CSteamID m_id { get; private set; }
    public CSteamID m_ownerID { get; private set; }
    public ELobbyType m_type { get; private set; }
    public int m_members { get; private set; }
    public int m_maxMembers { get; private set; }
    public bool m_joinable { get; private set; }
    public int m_gameport { get; private set; }
    public Dictionary<string, string> Data { get; private set; } = null;

    public LobbyData(PortPair ports, ELobbyType eLobbyType, int cMaxMembers)
    {
        m_id = new CSteamID(Convert.ToUInt64(ports.tcp));
        m_ownerID = SteamUser.GetSteamID(); //Method has been patched, so this should be fine
        m_type = eLobbyType;
        m_maxMembers = cMaxMembers;
        m_joinable = true;
        m_gameport = ports.udp;
        Data = new Dictionary<string, string>();
    }

    public int GetLobbyDataCount()
    {
        return Data.Count;
    }

    public string GetLobbyData(string pchKey)
    {
        string value = "";
        if (Data.ContainsKey(pchKey))
        {
            value = Data[pchKey];
        }
        return value;
    }

    public bool SetLobbyData(string pchKey, string pchValue)
    {
        Data[pchKey] = pchValue;
        return true;
    }
}
