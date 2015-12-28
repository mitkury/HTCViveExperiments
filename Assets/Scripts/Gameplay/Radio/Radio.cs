using UnityEngine;
using System.Collections;

public class Radio : ObtainableItem {

	public Transform cassettePoint;
	public Lever flap;
	public AudioSource speakers;

	bool isBroken;

	public bool FlapIsOpen {
		get {
			return flap.IsOn;
		}
	}

	public void Play(ObtainableItem item) {
		CloseFlap();

		item.transform.position = cassettePoint.position;
		item.transform.parent = transform;
		item.GetComponent<Rigidbody>().isKinematic = true;
		item.GetComponent<Rigidbody>().detectCollisions = false;

		var cassette = item.GetComponent<Cassette>();

		if (cassette == null)
			return;

		speakers.clip = cassette.audioClip;

		Invoke("Play", 1.5f);
	}

	public void Play() {
		speakers.Play();
	}

	public void OpenFlap() {
		if (!flap.IsOn) {
			flap.Switch();
		}
	}

	public void CloseFlap() {
		if (flap.IsOn) {
			flap.Switch();
		}
	}

	public void ChangePitchTo(float pitch, float time) {
		LeanTween.value(gameObject, delegate(float value) { 
			speakers.pitch = value;
		}, speakers.pitch, pitch, time);
	}

	protected override void Start() {
		base.Start();

		On(flap, KnobActionType.OnTurnOn, OnFlapOpen);
		On(flap, KnobActionType.OnTurnOff, OnFlapClose);

		//On(WaterEventType.OnPutInWater, OnPutInWater);
		//On(WaterEventType.OnPutOutFromWater, OnPutOutFromWater);
	}

	IEnumerator ChangePitchInSecCo(float pitch, float time, float delay) {
		yield return new WaitForSeconds(delay);
		ChangePitchTo(pitch, time);
	}

	void OnFlapOpen(object sender) {

	}

	void OnFlapClose(object sender) {

	}

	void OnPutInWater(object sender) {
		if (isBroken)
			return;

		StartCoroutine(ChangePitchInSecCo(0f, 3f, 1f));

		isBroken = true;
	}

	void OnPutOutFromWater(object sender) {
		if (isBroken)
			return;

		Debug.Log("Radio out of the water... but who cares now.");
	}

}
