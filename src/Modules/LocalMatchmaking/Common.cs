using System;
using System.Collections.Generic;
using System.Text;

namespace WalthexLocalPlay.Modules.LocalMatchmaking;
public enum EMessageType
{
    EMessageTypeOK,
    EMessageTypeFail,
    EMessageTypeGetData,
    EMessageTypeSetData,
};

//MetaData dictionary keys, all lowercase 
public static class MetaDataFields
{
    public const string EMessageTypeField = "messagetype";
    public const string SteamID = "steamid";
}