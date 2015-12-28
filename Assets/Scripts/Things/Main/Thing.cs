using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThingEventType {
	public static readonly ThingEventType OnTemperatureChange = new ThingEventType("ChangesTemperature_");

	public override string ToString() {
		return Value;
	}
	
	protected ThingEventType(string value) {
		this.Value = value;
	}
	
	public string Value { get; private set; }
}

[System.Serializable]
public struct ThingPhysicalProperties {
	public float temperature;
	public float areaTemperature;
	public float thermalConductivity;
	public float electroconductivity;

	public ThingPhysicalProperties(float temperature, float areaTemperature, float thermalConductivity, float electroconductivity) {
		this.temperature = temperature;
		this.areaTemperature = areaTemperature;
		this.thermalConductivity = thermalConductivity;
		this.electroconductivity = electroconductivity;
	}
}

[DisallowMultipleComponent]
public class Thing : MonoBehaviour {

	public List<Thing> receivers = new List<Thing>();
	public ThingPhysicalProperties properties = new ThingPhysicalProperties(25f, 25f, 1f, 0f);

	protected List<Thing> contributors = new List<Thing>();
	protected ThingPhysicalProperties prevProperties = new ThingPhysicalProperties();
	protected PlaysSoundOnRequest audioPlayer;

	List<LinkToHandlerInActiumEventData> linksToHandlers = new List<LinkToHandlerInActiumEventData>();

	/// <summary>
	/// Adds a thing to the list of contributors that may dispatch events that this thing can handle.
	/// </summary>
	/// <param name="thing">Thing.</param>
	public void AddContributor(Thing thing) {
		contributors.Add(thing);

		UpdateEventListenersForContributors();
	}

    public List<Thing> Contributors
    {
        get
        {
            return contributors;
        }
    }
	
	/// <summary>
	/// Gets the name for an Actium event with a unique GameObject ID.
	/// </summary>
	/// <returns>The Actium event name.</returns>
	/// <param name="eventType">Event type.</param>
	public string GetActiumEventNameWithID(ThingEventType eventType) {
		return GetActiumEventNameWithID(eventType.ToString());
	}
	
    // TODO: When a thing has a networkIdentity replace GetInstanceID() with NetId in order to send those events over the network.
	public string GetActiumEventNameWithID(string eventType) {
		return eventType + gameObject.name + "_ID" + GetInstanceID().ToString();
	}
	
	public virtual void OnInteractionStart(object sender) {}
	
	public virtual void OnInteractionStop(object sender) {}

	/// <summary>
	/// Set up an event listener in Actium relevant to ThingEventType, adds a GameObject ID to the event name.
	/// </summary>
	/// <param name="eventType">Event type.</param>
	/// <param name="handler">Event handler.</param>
	public void On(ThingEventType eventType, ActiumEventHandler handler) {
		var eventName = GetActiumEventNameWithID(eventType);

		On(eventName, handler);
	}

	/// <summary>
	/// Set up an event listener in Actium relevant to ThingEventType of the contributor thing, adds contributor's GameObject ED to the event name.
	/// </summary>
	/// <param name="contributor">Contributor.</param>
	/// <param name="eventType">Event type.</param>
	/// <param name="handler">Handler.</param>
	public void On(Thing contributor, ThingEventType eventType, ActiumEventHandler handler) {
		var eventName = contributor.GetActiumEventNameWithID(eventType);

		contributor.On(eventName, handler);
	}
	
	/// <summary>
	/// Set up an event listener in Actium.
	/// </summary>
	/// <param name="eventName">Event name.</param>
	/// <param name="handler">Event handler.</param>
	public void On(string eventName, ActiumEventHandler handler) {
		Actium.On(eventName, handler);
		
		linksToHandlers.Add(new LinkToHandlerInActiumEventData(eventName, handler));
	}
	
	/// <summary>
	/// Removes all event listeners for this thing from Actium.
	/// </summary>
	public void RemoveAllListeners() {
		foreach (var link in linksToHandlers) {
			Actium.RemoveOn(link.name, link.handler);
		}
		
		linksToHandlers.Clear();
	}
	
	public void Dispatch(ThingEventType eventType, object sender = null) {
		Dispatch(GetActiumEventNameWithID(eventType), sender);
	}
	
	public void Dispatch(string eventName, object sender = null) {
		Actium.Dispatch(eventName, sender);
	}

    /*
    // TODO: figure out how to dispatch them for every thing on a scene that is not NetworkBehaviour
    // TODO: what if Actium would search for NetworkInstanceID first and if it would find it it whould use it in GetActiumEventNameWithID instead of a regular InstanceID.
    public void DispaptchOverNetwork(ThingEventType eventType, object sender = null, bool waitForServer = false)
    {
        // GetActiumEventNameWithID wouldn't work because NetworkID is set locally and is different on each client. (look at NetworkInstanceId)

        Actium.DispatchOverNetwork(GetActiumEventNameWithID(eventType), sender, waitForServer);
    }

    public void DispaptchOverNetwork(string eventName, object sender = null, bool waitForServer = false)
    {
        Actium.DispatchOverNetwork(eventName, sender, waitForServer);
    }
    */

    /// <summary>
    /// Updates event listeners for the contributors.
    /// </summary>
    public virtual void UpdateEventListenersForContributors() {
		foreach (var thing in contributors) {
			UpdateContributor(thing);
		}
	}

	/// <summary>
	/// Calls for every contributor in the list of contributors.
	/// </summary>
	/// <param name="contributor">Contributor.</param>
	public virtual void UpdateContributor(Thing contributor) {}

	/// <summary>
	/// Updates the physical properties rougly once a second (1—1.5f).
	/// </summary>
	protected virtual IEnumerator UpdatePhysicalPropertiesCo() {
		float deltaTime = 0f;

		while (true) {
			if (prevProperties.temperature != properties.temperature) {
				Dispatch(ThingEventType.OnTemperatureChange, this);
			}


			// Temperature loss/gain from the area.
			var diff = properties.areaTemperature - properties.temperature;
			if (Mathf.Abs(diff) >= 0.01f) {
				var addTemperature = 1f * deltaTime;
				addTemperature = diff > 0 ? addTemperature : -addTemperature;
				properties.temperature += addTemperature;
			} else {
				properties.temperature = properties.areaTemperature;
			}

			prevProperties = properties;

			deltaTime = 1f + Random.Range(0, 0.5f);

			// Yied every 1-1.5f seconds in order to not pillow the thread with this coroutine.
			yield return new WaitForSeconds(deltaTime);
		}
	}

	protected virtual void Start() {
		foreach (var receiver in receivers) {
			receiver.AddContributor(this);
		}

		audioPlayer = GetComponent<PlaysSoundOnRequest>();
		if (audioPlayer == null) {
			audioPlayer = gameObject.AddComponent<PlaysSoundOnRequest>();
		}
	}
	
	protected virtual void OnEnable() {
		UpdateEventListenersForContributors();

		StartCoroutine(UpdatePhysicalPropertiesCo());
	}
	
	protected virtual void OnDisable() {
		RemoveAllListeners();
	}

}
