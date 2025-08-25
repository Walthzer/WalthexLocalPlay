using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WatsonTcp;

namespace WalthexLocalPlay.Modules.LocalMatchmaking;

public class LobbyFetcher
{
    public static async Task<LobbyData> FetchLobbyData(IPAddress ip, int lobbyPort)
    {
        Dictionary<string, object> md = new Dictionary<string, object>
        {
            { MetaDataFields.EMessageTypeField, EMessageType.EMessageTypeGetData.ToString() }
        };
        string resultJson = await doFetch(ip, lobbyPort, md);
        if (resultJson == null || resultJson.Length <= 0)
        {
            WLPPlugin.Logger.LogError($"FetchLobbyData Error. call for port: {lobbyPort} Invalid resultJson {resultJson}");
            return null;
        }

        //try to deserialise the LobbyData
        try
        {
            LobbyData result = JsonSerializer.Deserialize<LobbyData>(resultJson);
            return result;
        }
        catch (Exception)
        {
            WLPPlugin.Logger.LogError($"FetchLobbyData Error. call for port: {lobbyPort} resultJson deserialize failed {resultJson}");
        }
        return null;
    }

    private static async Task<string> doFetch(IPAddress ip, int lobbyPort, Dictionary<string, object> metadata, object jsonObject=null)
    {
        //Before trying any networking, verify call parameters
        if (!metadata.ContainsKey(MetaDataFields.EMessageTypeField))
        {
            WLPPlugin.Logger.LogError($"doFetch Error. call for port: {lobbyPort} Invalid metadata parameter: {metadata}");
            return null;
        }

        //Content is empty or json
        string content = "";
        try
        {
            if (jsonObject != null)
            {
                content = JsonSerializer.Serialize(jsonObject);
            }
        }
        catch (Exception)
        {
            WLPPlugin.Logger.LogError($"doFetch Error. call for port: {lobbyPort} Invalid jsonObject parameter {jsonObject}");
            return null;
        }

        //Open a connection to the potential localMatchmaking server
        using (WatsonTcpClient fetcher = new WatsonTcpClient(ip.ToString(), lobbyPort))
        {
            fetcher.Events.MessageReceived += MessageReceived;

            try
            {
                fetcher.Connect();
            }
            catch (TimeoutException)
            {
                WLPPlugin.Logger.LogDebug($"doFetch failed. Could not connect on port {lobbyPort}");
            }

            //Connection failed
            if (!fetcher.Connected)
            {
                WLPPlugin.Logger.LogDebug($"doFetch failed. closed port: {lobbyPort}");
                return null;
            }

            // send and wait for a response
            try
            {
                // Ensure client steamID is included in metadata
                metadata.Add(MetaDataFields.SteamID, WLPPlugin.steamID.m_SteamID);

                
                await fetcher.SendAsync(Encoding.UTF8.GetBytes(content), metadata);
                return null;
                //Force use of UTF8 to encode message content
                SyncResponse resp = await fetcher.SendAndWaitAsync(500, Encoding.UTF8.GetBytes(content), metadata);
                
                //Response is actually from a localMatchmaking server
                if (!resp.Metadata.TryGetValue(MetaDataFields.EMessageTypeField, out object respTypeObj) || !resp.Metadata.TryGetValue(MetaDataFields.SteamID, out object serverIDObj))
                {
                    WLPPlugin.Logger.LogInfo($"doFetch failed. server on port: {lobbyPort} is not a localMatchmaking server.");
                    return null;
                }
                //Clunky casting
                EMessageType responseType = (EMessageType)Convert.ToInt32(respTypeObj);
                string serverID = Convert.ToString(serverIDObj);

                //Response is ok
                if (responseType != EMessageType.EMessageTypeOK)
                {
                    //It is not ok
                    WLPPlugin.Logger.LogError($"doFetch Error. server on port: {lobbyPort} with steamID: {serverID} responded with {responseType}");
                    return null;

                }
                WLPPlugin.Logger.LogInfo($"doFetch Succes. server on port: {lobbyPort} with steamID: {serverID}");
                return Encoding.UTF8.GetString(resp.Data, 0, resp.Data.Length);
            }
            catch (TimeoutException)
            {
                WLPPlugin.Logger.LogDebug($"doFetch failed. Timeout on port: {lobbyPort}");
            }
            return null;
        }

    }

    static void MessageReceived(object sender, MessageReceivedEventArgs args)
    {
        WLPPlugin.Logger.LogDebug(
            "Message from "
            + args.Client.ToString()
            + ": "
            + Encoding.UTF8.GetString(args.Data));
    }
}

