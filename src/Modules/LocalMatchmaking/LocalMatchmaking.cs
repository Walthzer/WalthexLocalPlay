using System;
using System.Collections.Generic;
using Steamworks;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine;
using FishNet.Managing;

namespace WalthexLocalPlay.Modules.LocalMatchmaking;

//Core of the LocalPlay system. Reimplements some of the SteamMatchmaking API. Refer to the SteamMatchmaking.cs under Patches for what functions are disabled.

public class LocalMatchmaking
{
    public IPAddress m_IpAddress { get; private set; }
    //PortRange of possible local lobby servers
    public PortRange m_PortRange { get; private set; }
    public LobbyServer m_server { get; private set; } = null;
    public LobbyClient m_client { get; private set; } = null;

    //Holders for API calls
    //RequestLobbyList
    private List<LobbyData> lobbyList = null;

    public LocalMatchmaking(IPAddress ipAddress, PortRange portRange)
    {
        this.m_IpAddress = ipAddress;
        this.m_PortRange = portRange;
        this.lobbyList = new List<LobbyData>();
    }



    //Try to fetch lobbyData from all ports in PortRange
    public void RequestLobbyList()
    {
        //Empty previous lobbyList
        lobbyList.Clear();
        List<Task<LobbyData>> tasks = new List<Task<LobbyData>>();
        //Probe all ports
        foreach (int port in m_PortRange.Ports())
        {
            //Freak case where we request lobbyList, whilst owning a lobby
            if (isServer() && port == m_server.m_ports.tcp)
            {
                //We do not allow this
                WLPPlugin.Logger.LogError("RequestLobbyList Error: Requested lobby data of locally hosted lobby. This should not happen");
                continue;
            }
            tasks.Add(LobbyFetcher.FetchLobbyData(m_IpAddress, port));
        }
        //Wait for probes to complete, max 2 seconds
        Task.WaitAll(tasks.ToArray(), 2000);

        //Succesful probes are real lobbies
        //Could be replaced with a lambda
        foreach (Task<LobbyData> task in tasks)
        {
            if (task.Result != null)
            {
                lobbyList.Add(task.Result);
            }
        }

        //Invoke LobbyMatchList_t
        LobbyMatchList_t LobbyMatchList = new LobbyMatchList_t()
        {
            m_nLobbiesMatching = Convert.ToUInt32(lobbyList.Count)
        };
        LocalDispatcher.createCall(LobbyMatchList, LobbyMatchList_t.k_iCallback);
    }

    public CSteamID GetLobbyByIndex(int iLobby)
    {
        if (lobbyList == null || iLobby < 0 || iLobby > lobbyList.Count)
        {
            return new CSteamID(0);
        }
        return lobbyList[iLobby].m_id;
    }

    public int GetLobbyDataCount(CSteamID lobbyID)
    {
        if (isServer())
        {
            lock (m_server)
            {
                if (lobbyID.m_SteamID == m_server.m_lobby.m_id.m_SteamID)
                {
                    return m_server.m_lobby.GetLobbyDataCount();
                }
            }

        }

        foreach (LobbyData lobby in lobbyList)
        {
            if (lobby.m_id.m_SteamID == lobbyID.m_SteamID)
            {
                return lobby.GetLobbyDataCount();
            }
        }
        return 0;
    }

    public bool RequestLobbyData(CSteamID lobbyID)
    {
        Dictionary<string, string> data = null;
        if (isServer())
        {
            lock (m_server)
            {
                if (lobbyID.m_SteamID == m_server.m_lobby.m_id.m_SteamID)
                {
                    //Copy lobby data. Should be fine with string due to being immutable.
                    data = new Dictionary<string, string>(m_server.m_lobby.Data);
                    //invoke OnLobbyData
                    InvokeLobbyDataUpdate(true, lobbyID, lobbyID);
                    return true;
                }
            }
        }

        foreach (LobbyData lobby in lobbyList)
        {
            if (lobby.m_id.m_SteamID == lobbyID.m_SteamID)
            {
                //invoke OnLobbyData
                data = lobby.Data;
                InvokeLobbyDataUpdate(true, lobbyID, lobbyID);
                return true;
            }
        }
        InvokeLobbyDataUpdate(false, new CSteamID(0), new CSteamID(0));
        return false;
    }

    public string GetLobbyData(CSteamID lobbyID, string key)
    {
        if (isServer())
        {
            lock (m_server)
            {
                if (lobbyID.m_SteamID == m_server.m_lobby.m_id.m_SteamID)
                {
                    string value = m_server.m_lobby.GetLobbyData(key);
                    return value;
                }
            }
        }

        foreach (LobbyData lobby in lobbyList)
        {
            if (lobby.m_id.m_SteamID == lobbyID.m_SteamID)
            {
                return lobby.GetLobbyData(key);
            }
        }
        return "";
    }

    public bool SetLobbyData(CSteamID lobbyID, string key, string value)
    {
        if (isServer())
        {
            lock (m_server)
            {
                if (lobbyID.m_SteamID == m_server.m_lobby.m_id.m_SteamID)
                {
                    m_server.m_lobby.SetLobbyData(key, value);
                    return true;
                }
            }
        }
        //if we aren't the server. Then we don't own the lobby. So we can just fail
        return false;
    }

    public int GetNumLobbyMembers(CSteamID lobbyID)
    {
        if (isServer())
        {
            lock (m_server)
            {
                if (lobbyID.m_SteamID == m_server.m_lobby.m_id.m_SteamID)
                {
                    return m_server.m_lobby.m_members;
                }
            }
        }

        foreach (LobbyData lobby in lobbyList)
        {
            if (lobby.m_id.m_SteamID == lobbyID.m_SteamID)
            {
                return lobby.m_members;
            }
        }
        return -1;
    }

    public CSteamID GetLobbyOwner(CSteamID lobbyID)
    {
        if (isServer())
        {
            lock (m_server)
            {
                if (lobbyID.m_SteamID == m_server.m_lobby.m_id.m_SteamID)
                {
                    CSteamID ownerID = new CSteamID(m_server.m_lobby.m_ownerID.m_SteamID);
                    return ownerID;
                }
            }
        }

        foreach (LobbyData lobby in lobbyList)
        {
            if (lobby.m_id.m_SteamID == lobbyID.m_SteamID)
            {
                return lobby.m_ownerID;
            }
        }
        return new CSteamID(0);
    }

    public void CreateLobby(ELobbyType eLobbyType, int cMaxMembers)
    {
        if (isServer())
        {
            lock (m_server)
            {
                m_server.Stop();
            }
        }

        m_server = LobbyServer.CreateServer(m_IpAddress, m_PortRange);
        if (isServer())
        {
            m_server.StartLobby(eLobbyType, cMaxMembers);
            WLPPlugin.Logger.LogInfo($"localMatchmaking: Server started. IP: {m_IpAddress}, Port: {m_server.m_ports.tcp}");

            //Setup Tugboat
            WLPPlugin.Tugboat.SetClientAddress(m_IpAddress.ToString());
            WLPPlugin.Tugboat.SetPort(Convert.ToUInt16(m_server.m_ports.udp));

            //Dispatch lobby created event
            InvokeLobbyCreated(EResult.k_EResultOK, m_server.m_lobby.m_id);
            return;

        }
        InvokeLobbyCreated(EResult.k_EResultFail, new CSteamID(0));
        WLPPlugin.Logger.LogError("localMatchmaking: Server start failed.");
    }

    public void LeaveLobby(CSteamID steamIDLobby)
    {
        if (isServer())
        {
            lock (m_server)
            {
                m_server.Stop();
            }
            m_server = null;
            return;
        }
    }

    public bool isServer()
    {
        return m_server != null;
    }

    //Emits a LobbyCreated_t
    //Also Emit a LobbyEnter_t (As you enter the lobby you create)
    //Also Emit a LobbyDataUpdate
    //Also Emits a LobbyChatUpdate_t
    public void InvokeLobbyCreated(EResult result, CSteamID lobbyID)
    {
        LobbyCreated_t lobbyCreated = new LobbyCreated_t
        {
            m_eResult = result,
            m_ulSteamIDLobby = lobbyID.m_SteamID
        };
        LocalDispatcher.createCall(lobbyCreated, LobbyCreated_t.k_iCallback);

        //Delay LobbyEnter event until we are sure the Tugboat server is running (Race condition with MainMenuManager)
        WLPPlugin.Instance.EnterLobbyWhenServerOK(lobbyID);
    }



    //Emits a LobbyEnter_t
    public void InvokeLobbyEntered(EChatRoomEnterResponse result, CSteamID lobbyID)
    {
        LobbyEnter_t lobbyEnter = new LobbyEnter_t
        {
            m_ulSteamIDLobby = lobbyID.m_SteamID,
            m_rgfChatPermissions = 0,
            m_bLocked = false,
            m_EChatRoomEnterResponse = Convert.ToUInt32(result),
        };
        LocalDispatcher.createCall(lobbyEnter, LobbyEnter_t.k_iCallback);
    }

    //Emits a LobbyData_t
    public void InvokeLobbyDataUpdate(bool succes, CSteamID lobbyID, CSteamID lobbyOrMemberID)
    {
        LobbyDataUpdate_t lobbyDataUpdate = new LobbyDataUpdate_t
        {
            m_ulSteamIDLobby = lobbyID.m_SteamID,
            m_ulSteamIDMember = lobbyOrMemberID.m_SteamID,
            m_bSuccess = Convert.ToByte(succes)
        };
        LocalDispatcher.createCall(lobbyDataUpdate, LobbyDataUpdate_t.k_iCallback);
    }

    //Emits a LobbyChatUpdate_t
    public void InvokeLobbyChatUpdate(EChatMemberStateChange change, CSteamID lobbyID, CSteamID memberID, CSteamID EffectorMemberID)
    {
        LobbyChatUpdate_t LobbyChatUpdate = new LobbyChatUpdate_t
        {
            m_ulSteamIDLobby = lobbyID.m_SteamID,
            m_ulSteamIDUserChanged = memberID.m_SteamID,
            m_ulSteamIDMakingChange = EffectorMemberID.m_SteamID,
            m_rgfChatMemberStateChange = Convert.ToUInt32(change)
        };
        LocalDispatcher.createCall(LobbyChatUpdate, LobbyChatUpdate_t.k_iCallback);
    }
}
