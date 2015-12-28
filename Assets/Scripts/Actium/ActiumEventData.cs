using System;

public delegate void ActiumEventHandler(object sender);
//public delegate void ActiumEventHandler(ActiumEvent event);

/// <summary>
/// Keeps a name of the event and event handlers.
/// </summary>
public class ActiumEventData {
	
	public event ActiumEventHandler eventsDispatched;

	public string Name { get; private set; }

	public ActiumEventData(string eventName) {
		Name = eventName;
	}

	public void Invoke(object sender) {
		if (eventsDispatched != null) {
			eventsDispatched(sender);
		}
	}
}

public struct LinkToHandlerInActiumEventData {
	public string name;
	public ActiumEventHandler handler;
	
	public LinkToHandlerInActiumEventData(string name, ActiumEventHandler handler) {
		this.name = name;
		this.handler = handler;
	}
}