using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class KnobActionType : InteractiveThingEventType {
	public static readonly KnobActionType OnTurnOn = new KnobActionType("TurnsOn_");
	public static readonly KnobActionType OnTurnOff = new KnobActionType("TurnsOff_");
	
	protected KnobActionType(string value) : base(value) {}
}

public class Knob : InteractiveThing {

	public Vector3 axis = new Vector3(0, 0, 1f);
	//public float min = 0f;
	public float max = 90f;
	public float animationTime = 0.5f;
	public AudioClipData switchAudio = new AudioClipData();

	float progress;
	Quaternion initLocalRotation;

	public float Progress {
		get {
			return progress;
		}
		set {
			progress = value;

			LeanTween.cancel(gameObject);
			float angleBetweenInitAndCurrentRotation = Quaternion.Angle(initLocalRotation, transform.localRotation);
			LeanTween.rotateAroundLocal(gameObject, axis, progress * max - angleBetweenInitAndCurrentRotation, animationTime).setEase(LeanTweenType.easeInSine);

			if (progress == 0) {
				Dispatch(KnobActionType.OnTurnOff);
			} else if (progress >= 1f) {
				Dispatch(KnobActionType.OnTurnOn);
			}
		}
	}

	public override void OnInteractionStart(object sender) {
		base.OnInteractionStart(sender);

		Switch();
	}

	protected override void Start () {
		base.Start ();

		initLocalRotation = transform.localRotation;
	}

	public void Switch() {
		Progress = Progress < 0.5f ? 1f : 0f;

		if (audioPlayer != null && switchAudio.audioClip != null) {
			audioPlayer.PlayOneShot(switchAudio);
		}
	}
}
