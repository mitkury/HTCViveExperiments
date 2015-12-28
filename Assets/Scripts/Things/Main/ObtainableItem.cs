using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class ObtainableItemEventType : InteractiveThingEventType {
	public static readonly ObtainableItemEventType OnPickUp = new ObtainableItemEventType("Visitor_PicksUp_");
	public static readonly ObtainableItemEventType OnDropOff = new ObtainableItemEventType("Visitor_DropsOff_");
	
	private ObtainableItemEventType(string value) : base(value) {}
}

[RequireComponent(typeof(Rigidbody))]
public class ObtainableItem : InteractiveThing {

	public List<AudioClipDataOnImpact> audioOnImpact;
	[HideInInspector]
	public bool defaultGravity;
	public AudioClipData onPickUpAudio = new AudioClipData();
	public AudioClipData onDropOffAudio = new AudioClipData();
    public GameObject owner;
    public GameObject lastOwner;

	public bool IsObtained
    {
        get
        {
            return owner != null;
        }
    }

	public bool IsObtainable { get; set; }

	public virtual void OnPickUp(object sender) {
		if (!IsObtainable)
			return;

        //GetComponent<Rigidbody>().isKinematic = true;

        /*
		GetComponent<Rigidbody>().isKinematic = false;
        
		if (sender is MonoBehaviour) {
			var senderMonoBehaviour = sender as MonoBehaviour;
			transform.parent = senderMonoBehaviour.transform.parent;
		}
        */

        LeanTween.cancel(gameObject);

		audioPlayer.PlayOneShot(onPickUpAudio);

        //Debug.Log(sender + " tries to pick up the " + gameObject.name);

		/*
        var visitor = (sender as GameObject).GetComponent<Visitor>();

        owner = visitor.gameObject;
        lastOwner = owner;

        visitor.PickUp(gameObject);
        */


		GetComponent<Rigidbody>().isKinematic = true;

        var component = (MonoBehaviour)sender;

        owner = component.gameObject;

        transform.parent = owner.transform;
    }

	public virtual void OnDropOff(object sender) {
        audioPlayer.PlayOneShot(onDropOffAudio);

		GetComponent<Rigidbody>().isKinematic = false;

        var component = (MonoBehaviour)sender;
        transform.parent = component.transform.parent;
    }

	protected override void Start () {
		base.Start();

		defaultGravity = GetComponent<Rigidbody>().useGravity;

		var soundOnImpact = gameObject.AddComponent<PlaysSoundOnImpact>();
		soundOnImpact.audioClipsData = audioOnImpact;

		IsObtainable = true;
	}
	
	protected override void OnEnable () {
		base.OnEnable();

		On(ObtainableItemEventType.OnPickUp, OnPickUp);
		On(ObtainableItemEventType.OnDropOff, OnDropOff);
	}

	void Update()
	{
		if (owner == null)
			return;

        /*
		transform.position = owner.transform.position;
        transform.rotation = owner.transform.rotation;
        */
	}

}
