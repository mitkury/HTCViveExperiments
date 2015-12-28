using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrabTrigger : EnterExitSensor 
{
	public List<Thing> things;

	protected override Thing ProcessSensorWith (Collider other, System.Boolean entering)
	{
		var thing = base.ProcessSensorWith (other, entering);

		if (thing == null)
			return null;

		things.RemoveAll(t => thing);

		if (entering)
		{
			things.Add(thing);
		}

		return thing;
	}

	

}
