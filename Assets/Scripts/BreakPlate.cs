using UnityEngine;
using System.Collections;

public class BreakPlate : ObtainableItem
{
	public float minVelocity = 0.3f;
	public GameObject brokenPlate;
	private bool broken = false;

	void OnCollisionEnter(Collision collision) 
	{
		if(broken) return;
		var magnitude = collision.relativeVelocity.magnitude;
		if(magnitude < minVelocity)
			return;

		Smash ();
		return;
	}

	private void Smash ()
	{
		broken = true; // Prevent collision from calling more than once

		// Spawn Broken Plate
		GameObject p = Instantiate(brokenPlate) as GameObject;
		p.transform.parent = transform.parent;
		p.transform.localRotation = transform.localRotation;
		p.transform.localPosition = transform.localPosition;

		Rigidbody[] bodies = p.GetComponentsInChildren<Rigidbody>();

		foreach (Rigidbody b in bodies)
		{
			b.velocity = gameObject.GetComponent<Rigidbody>().velocity;

			b.AddExplosionForce (15.0f,gameObject.transform.position,2.0f,0.05f);
		}

		// Destroy plate
		Destroy(gameObject);
	}
}
