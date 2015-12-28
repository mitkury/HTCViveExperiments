using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveThingEventType : ThingEventType {
	public static readonly InteractiveThingEventType OnInteractionStart = new InteractiveThingEventType("Visitor_StartsInteractionWith_");
	public static readonly InteractiveThingEventType OnInteractionStop = new InteractiveThingEventType("Visitor_StopsInteractionWith_");
	
	protected InteractiveThingEventType(string value) : base(value) {}
}

public class InteractiveThing : Thing {

	protected override void OnEnable() {
		base.OnEnable();

		On(InteractiveThingEventType.OnInteractionStart, OnInteractionStart);
		On(InteractiveThingEventType.OnInteractionStop, OnInteractionStop);
	}

}
