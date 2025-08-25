using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace WalthexLocalPlay.Modules;

//Calls Steamworks callbacks while steamworks itself is disabled.
//Still uses unmanaged memory to pass callback parameters
//locking is done in the dispatcher, as calls can come in from threads

public class LocalDispatcher
{
    private Queue<CallbackMsg_t> m_calls = new Queue<CallbackMsg_t>();
    public object m_sync = new object();

    public static void createCall<T>(T callType, int iCallback)
    {
        try
        {
            CallbackMsg_t call = new CallbackMsg_t()
            {
                m_iCallback = iCallback,
                m_pubParam = Marshal.AllocHGlobal(Marshal.SizeOf(callType))
            };
            Marshal.StructureToPtr(callType, call.m_pubParam, false);
            WLPPlugin.LocalDispatcher.pushCall(call);
        }
        catch (Exception e)
        {
            WLPPlugin.Logger.LogError(e);
        }
    }

    public void pushCall(CallbackMsg_t call)
    {
        lock (m_sync)
        {
            m_calls.Enqueue(call);
        }
    }

    public void RunFrame()
    {
        FieldInfo field = typeof(CallbackDispatcher).GetField("m_registeredCallbacks", BindingFlags.NonPublic | BindingFlags.Static);
        Dictionary<int, List<Callback>> m_registeredCallbacks = (Dictionary<int, List<Callback>>)field.GetValue(null);
        if (m_registeredCallbacks == null)
        {
            return;
        }
        lock (m_sync)
        {
            while (m_calls.TryDequeue(out CallbackMsg_t callbackMsg_t))
            {
                try
                {
                    List<Callback> list = null;
                    List<Callback> value2 = null;
                    if (m_registeredCallbacks.TryGetValue(callbackMsg_t.m_iCallback, out value2))
                    {
                        list = new List<Callback>(value2);
                    }

                    if (list == null)
                    {
                        continue;
                    }

                    foreach (Callback item2 in list)
                    {
                        MethodInfo methodInfo = typeof(Callback).GetMethod("OnRunCallback", BindingFlags.NonPublic | BindingFlags.Instance);
                        methodInfo.Invoke(item2, new object[] { callbackMsg_t.m_pubParam });
                    }
                }
                catch (Exception e)
                {
                    WLPPlugin.Logger.LogError(e);
                }
                finally
                {
                    Marshal.FreeHGlobal(callbackMsg_t.m_pubParam);
                }
            }
        }
    }
}

