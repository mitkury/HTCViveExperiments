using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SegueManager : EnterExitSensor {

	List<ObtainableItem> items = new List<ObtainableItem>();
	Segue segue;

	public override void UpdateContributor (Thing contributor) {
		base.UpdateContributor (contributor);

		//On(contributor, SensorType.OnSensorEnter, OnSensorEnter);
		//On(contributor, SensorType.OnSensorExit, OnSensorExit);
	}

	protected override void Start () {
		base.Start ();

		segue = receivers[0] as Segue;
	}

	protected override Thing ProcessSensorWith (Collider other, bool entering) {
		var thing = base.ProcessSensorWith (other, entering);

		if (thing == null)
			return null;

		if (entering) {
			var obtainableItem = thing as ObtainableItem;
			
			if (obtainableItem == null) 
				return null;
			
			// It's important that the item is obtained for adding to the list, so a segue
			// performs only when the player releases the item inside the sensor.
			if (obtainableItem.IsObtained) {
				items.Remove(obtainableItem);
				items.Add(obtainableItem);
			}
		} else {
			var obtainableItem = thing as ObtainableItem;
			
			if (obtainableItem == null) 
				return null;
			
			items.Remove(obtainableItem);		
		}

		return thing;
	}

	/*
	void OnSensorEnter(object sender) {
		var obtainableItem = sender as ObtainableItem;

		if (obtainableItem == null) 
			return;

		// It's important that the item is obtained for adding to the list, so a segue
		// performs only when the player releases the item inside the sensor.
		if (obtainableItem.IsObtained) {
			items.Remove(obtainableItem);
			items.Add(obtainableItem);
		}
	}

	void OnSensorExit(object sender) {
		var obtainableItem = sender as ObtainableItem;
		
		if (obtainableItem == null) 
			return;

		items.Remove(obtainableItem);
	}
	*/

	void Update() {
		if (items.Count == 0)
			return;

		for (int i = 0; i < items.Count; i++) {
			var item = items[i];

			// Perorm segues for items that were dropped inside the tirgger. NOT when they're still obtained.
			if (item.IsObtained)
				continue;
			
			if (segue != null) {
				segue.PerformSegue(item);
				items.Remove(item);
			}
		}
	}
}
