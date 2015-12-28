using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class NetworkActium : NetworkBehaviour
{
    protected static NetworkActium _instance;
    protected static NetworkClientActium _netClientActium;

    [SyncVar]
    protected GameObject syncInstance;

    public static NetworkActium Singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkActium>();
                //_instance = new GameObject("~NetworkActium", new Type[] { typeof(NetworkActium) }).GetComponent<NetworkActium>();
            }

            return _instance;
        }
    }

    public static NetworkClientActium Client
    {
        get
        {
            return _netClientActium;
        }
        set
        {
            _netClientActium = value;
        }
    }

    [ClientRpc]
    public void RpcDispatch(string eventName, object sender, NetworkInstanceId[] excludeNetId)
    {
        // Don't dispatch events that are not intendet for certain NetworkClientActium.netIds
        /*
        if (Array.Find<NetworkInstanceId>(excludeNetId, enetId => enetId == Client.netId) != null)
        {
            return;
        }
        */

        if (excludeNetId != null && excludeNetId[0] == Client.netId)
            return;

        //Debug.Log(eventName + " BY NetId" + excludeNetId[0]);

        Actium.Dispatch(eventName, sender);
    }

}
