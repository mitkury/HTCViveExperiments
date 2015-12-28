using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkClientActium : NetworkBehaviour
{

    void Start()
    {
        if (isLocalPlayer)
        {
            NetworkActium.Client = this;
        }
    }

    // TODO: includeNetId[], excludeNetId[]
    public void RequestDispatch(string eventName, object sender = null, bool dispatchLocally = false)
    {
        // Find a NetworkComponent that has 

        // TODO: Get a client's ID in order for a client to see whether an event was requested on the same client.

        NetworkInstanceId[] excludeNetIds = null;

        // If an event isn't intended to be called on a side that sent the event--add the current netId to a list of excludingNetIds.
        if (!dispatchLocally)
        {
            excludeNetIds = new NetworkInstanceId[1];
            excludeNetIds[0] = netId;
        }

        //Debug.Log("Local: Request dispatching " + eventName + " event.");
        CmdDispatch(eventName, sender, excludeNetIds);
    }

    public void RequestDispatchOnServer(string eventName, object sender = null)
    {
        CmdDispatchOnServer(eventName, sender);
    }

    [Command]
    void CmdDispatch(string eventName, object sender, NetworkInstanceId[] excludeNetIds)
    {
        //Debug.Log("Server: Dispatch " + eventName + " for every client.");
        // TODO: decide on the server to whom send messages. (look at NetworkServer.SendToClient);
        NetworkActium.Singleton.RpcDispatch(eventName, sender, excludeNetIds);
    }

    [Command]
    void CmdDispatchOnServer(string eventName, object sender)
    {
        Actium.Dispatch(eventName, sender);
    }
}
