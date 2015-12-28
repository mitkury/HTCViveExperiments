using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lever : InteractiveThing {
	
	public Vector3 axis = new Vector3(1f, 0, 0)	;
	public float openAngle = -45f;
	public float animationTime = 0.5f;
	public AudioClipData switchAudio = new AudioClipData();
	public AudioClipData switchFirstAudio = new AudioClipData();
	
	bool _isOn;
	Quaternion initLocalRotation;
	Quaternion openLocalRotation;

	public bool IsOn {
		get {
			return _isOn;
		}
	}

	public override void OnInteractionStart (object sender) {
		base.OnInteractionStart(sender);

		Switch();
	}
	
	public void Switch(object sender = null) {
		var targetAngle = _isOn ? Quaternion.Angle(transform.localRotation, initLocalRotation) : -Quaternion.Angle(transform.localRotation, openLocalRotation);
		LeanTween.cancel(gameObject);

		if (audioPlayer != null && switchFirstAudio != null && !IsOn) {
			audioPlayer.PlayOneShot(switchFirstAudio);
		}

		var tween = LeanTween.rotateAroundLocal(gameObject, axis, targetAngle, animationTime).setOnComplete(delegate() {
			if (audioPlayer != null && switchFirstAudio != null && !IsOn) {
				audioPlayer.PlayOneShot(switchFirstAudio);
			}
		});

		tween.setEase(LeanTweenType.easeInSine);

		_isOn = !_isOn;
		
		if (_isOn) {
			Dispatch(KnobActionType.OnTurnOn, sender);
		} else {
			Dispatch(KnobActionType.OnTurnOff, sender);
		}

		if (audioPlayer != null && switchAudio.audioClip != null) {
			audioPlayer.PlayOneShot(switchAudio);
		}
	}
	
	protected override void Start () {
		base.Start ();
		
		initLocalRotation = transform.localRotation;
		openLocalRotation = transform.localRotation * Quaternion.Euler(axis * openAngle);
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.blue;

		var openLocalRotation = transform.localRotation * Quaternion.Euler(axis * openAngle);
		Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(openLocalRotation.eulerAngles).normalized * -1);
	}
	
}