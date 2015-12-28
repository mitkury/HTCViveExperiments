using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages events. Subscribes event listeners and dispatches events.
/// </summary>
public class Actium : MonoBehaviour {

	static Actium _instance;
	static bool isShuttingDown;

	List<ActiumEventData> eventListeners = new List<ActiumEventData>();

	static Actium Instance {
		get {
			if (_instance != null) {
				return _instance;
			} else {
				_instance = GameObject.FindObjectOfType(typeof(Actium)) as Actium;
				
				if (_instance == null) {
					_instance = new GameObject("~Actium", new System.Type[] { typeof(Actium) }).GetComponent<Actium>();
				}
				
				return _instance;
			}
		}
	}

    // TODO: Implement storing and calling event names as hashes.
    /// <summary>
    /// Knuth hash
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns>A persistent Knuth hash</returns>
    // http://stackoverflow.com/questions/9545619/a-fast-hash-function-for-string-in-c-sharp
    public static UInt64 CalculateHash(string eventName)
    {
        UInt64 hashedValue = 3074457345618258791ul;
        for (int i = 0; i < eventName.Length; i++)
        {
            hashedValue += eventName[i];
            hashedValue *= 3074457345618258799ul;
        }
        return hashedValue;
    }

    /// <summary>
    /// Set up an event listener.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    /// <param name="method">Method.</param>
    public static void On(string eventName, ActiumEventHandler method) {
		if (isShuttingDown)
			return;

		var actiumEvent = Instance.eventListeners.Find(e => e.Name == eventName);

		if (actiumEvent == null) {
			actiumEvent = new ActiumEventData(eventName);
			Instance.eventListeners.Add(actiumEvent);
		}

		//Debug.Log("Setting up an event handler for "+eventName);

		actiumEvent.eventsDispatched -= method;
		actiumEvent.eventsDispatched += method;
	}

	/// <summary>
	/// Dispatches an event throughout all the event listeners.
	/// </summary>
	/// <param name="eventName">Event name.</param>
	/// <param name="sender">Sender.</param>
	public static void Dispatch(string eventName, object sender = null) {
		if (isShuttingDown)
			return;

		var actiumEvent = Instance.eventListeners.Find(e => e.Name == eventName);

		if (actiumEvent == null) {
			//Debug.LogWarning("Couldn't find an event with a name "+eventName);
			return;
		}

        //Debug.Log("Event: "+eventName);

		actiumEvent.Invoke(sender);
	}

    // TODO: includePlayers[], excludePlayers[]
    // TODO: dispatchLocally
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="sender"></param>
    /// <param name="dispatchLocally">Dispatches an event on a client that sent the event.</param>
    public static void DispatchOverNetwork(string eventName, object sender = null, bool dispatchLocally = false)
    {
        if (NetworkActium.Client != null)
        {
            NetworkActium.Client.RequestDispatch(eventName, sender, dispatchLocally);
        }
        else
        {
            Debug.LogWarning("Couldn't find an instance of NetworkClientActium attached to a player's gameobject. The event cannot be dispatched over the network without NetworkActium present.");
        }
    }

    public static void DispatchOnServer(string eventName, object sender = null)
    {
        if (NetworkActium.Client != null)
        {
            NetworkActium.Client.RequestDispatchOnServer(eventName, sender);
        }
        else
        {
            Debug.LogWarning("Couldn't find an instance of NetworkClientActium attached to a player's gameobject. The event cannot be dispatched over the network without NetworkActium present.");
        }
    }

    /// <summary>
    /// Removes all the event listener with a particular handle.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    /// <param name="method">Handler.</param>
    public static void RemoveOn(string eventName, ActiumEventHandler handler) {
		if (isShuttingDown)
			return;

		var actiumEvent = Instance.eventListeners.Find(e => e.Name == eventName);

		if (actiumEvent == null) {
			//Debug.LogWarning("Couldn't remove an event with a name "+eventName);
			return;
		}

		actiumEvent.eventsDispatched -= handler;
	}

	void OnApplicationQuit() {
		isShuttingDown = true;
	}

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
