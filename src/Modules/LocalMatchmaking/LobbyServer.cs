using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using WatsonTcp;
using System.Net.Mail;

namespace WalthexLocalPlay.Modules.LocalMatchmaking;

public class LobbyServer
{
    private WatsonTcpServer m_serverTCP;
    public PortPair m_ports { get; private set; }
    public LobbyData m_lobby { get; private set; }
    public static LobbyServer CreateServer(IPAddress ip, PortRange portRange)
    {
        if (ip == null)
        {
            WLPPlugin.Logger.LogError("Unable to create local lobby: invalid IP.");
            return null;
        }
        PortPair ports = FindOpenPortPair(portRange);
        if (ports == null)
        {
            WLPPlugin.Logger.LogError("Unable to create local lobby: No port pair available.");
            return null;
        }
        LobbyServer Server = new LobbyServer();
        Server.m_ports = ports;
        Server.m_serverTCP = new WatsonTcpServer(ip.ToString(), Server.m_ports.tcp);
        Server.m_serverTCP.Events.ClientConnected += ClientConnected;
        Server.m_serverTCP.Events.ClientDisconnected += ClientDisconnected;
        Server.m_serverTCP.Events.MessageReceived += MessageReceived;
        Server.m_serverTCP.Callbacks.SyncRequestReceivedAsync = SyncRequestReceived;

        //Debug
        Server.m_serverTCP.Settings.Logger = logMessage;
        Server.m_serverTCP.Settings.DebugMessages = true;

        return Server;
    }

    public static void logMessage(Severity sev, string message)
    {
        Debug.Log(message);
    }

    ~LobbyServer()
    {
        m_serverTCP.Stop();
        WLPPlugin.Logger.LogInfo($"LobbyServer on Port: {this.m_ports.tcp} stopped");
    }

    public void Stop()
    {
        m_serverTCP.Stop();
        WLPPlugin.Logger.LogInfo($"LobbyServer on Port: {this.m_ports.tcp} stopped");
    }

    public void StartLobby(ELobbyType type, int maxMembers)
    {
        m_lobby = new LobbyData(m_ports, type, maxMembers);
        m_serverTCP.Start();
        WLPPlugin.Logger.LogInfo($"LobbyServer on Port: {m_ports.tcp} Started");
    }

    static PortPair FindOpenPortPair(PortRange rangeLobbyTCP)
    {
        IPEndPoint[] tcpConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
        IPEndPoint[] udpConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();

        int GamePortUDPOffset = rangeLobbyTCP.Size() + 1;
        foreach (int portTCP in rangeLobbyTCP.Ports())
        {
            if (tcpConnections.Any(p => p.Port == portTCP))
            {
                continue;
            }
            if (udpConnections.Any(p => p.Port == (portTCP + GamePortUDPOffset)))
            {
                continue;
            }
            //Saniy check
            if (portTCP <= 0)
            {
                continue;
            }
            //Both ports are free
            return new PortPair(portTCP, portTCP + GamePortUDPOffset);
        }
        return null;

    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    static async Task<SyncResponse> SyncRequestReceived(SyncRequest req)
    {
        WLPPlugin.Logger.LogInfo($"SyncRequestReceived begin. Request recieved {req}");
        //Is this a valid request
        if (!req.Metadata.TryGetValue(MetaDataFields.EMessageTypeField, out object reqTypeObj) || !req.Metadata.TryGetValue(MetaDataFields.SteamID, out object clientIDObj))
        {
            WLPPlugin.Logger.LogInfo($"SyncRequestReceived failed. Invalid request recieved");
            return null;
        }
        //Clunky casting
        EMessageType requestType = (EMessageType)Convert.ToInt32(reqTypeObj);
        string clientID = Convert.ToString(clientIDObj);


        //Sloppy fix to get a reference from static function
        LobbyServer instance = WLPPlugin.LocalMatchmaking.m_server;
        //For now just lock on every request
        lock (instance)
        {
            switch (requestType)
            {
                case EMessageType.EMessageTypeGetData:
                    return instance.GetData(req);

                //case EMessageType.EMessageTypeSetData:
                default:
                    return instance.CreateResponse(req, EMessageType.EMessageTypeFail, "");
            }
        }
    }

    private SyncResponse CreateResponse(SyncRequest req, EMessageType type, string content)
    {
        Dictionary<string, object> responseMetadata = new Dictionary<string, object>();
        responseMetadata.Add(MetaDataFields.SteamID, m_lobby.m_id.m_SteamID.ToString());
        responseMetadata.Add(MetaDataFields.EMessageTypeField, type.ToString());
        return new SyncResponse(req, responseMetadata, Encoding.UTF8.GetBytes(content));
    }

    private SyncResponse GetData(SyncRequest req)
    {
        try
        {
            string content = JsonSerializer.Serialize(WLPPlugin.LocalMatchmaking.m_server.m_lobby);
            return CreateResponse(req, EMessageType.EMessageTypeOK, content);
        }
        catch (Exception)
        {
            WLPPlugin.Logger.LogError($"SyncRequestReceived Error. on request: EMessageTypeGetData. Failed to serialize lobby");
        }
        return CreateResponse(req, EMessageType.EMessageTypeFail, "");
    }

    //WatsonTCP server callbacks
    static void ClientConnected(object sender, ConnectionEventArgs args)
    {
        Debug.Log("Client connected: " + args.Client.ToString());
    }

    static void ClientDisconnected(object sender, DisconnectionEventArgs args)
    {
        Debug.Log(
            "Client disconnected: "
            + args.Client.ToString()
            + ": "
            + args.Reason.ToString());
    }

    static void MessageReceived(object sender, MessageReceivedEventArgs args)
    {
        Debug.Log(
            "Message from "
            + args.Client.ToString()
            + ": "
            + Encoding.UTF8.GetString(args.Data));
    }
}
