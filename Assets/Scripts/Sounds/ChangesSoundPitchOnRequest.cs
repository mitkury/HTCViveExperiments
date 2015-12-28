using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangesSoundPitchOnRequest : MonoBehaviour {

	static List<int> fadeTweenIDs;

	public AudioSource audioSourceReceiver;
	public float max = 1f;
	public float min = 0.5f;

	public void FadeIn(float time) {
		if (audioSourceReceiver == null)
			return;
		
		//LeanTween.cancel(audioSourceReceiver.gameObject);
		CancelFades();
		var tween = LeanTween.value(audioSourceReceiver.gameObject, UpdateAudioValue, audioSourceReceiver.pitch, max, time);
		fadeTweenIDs.Add(tween.uniqueId);
	}
	
	public void FadeOut(float time) {
		if (audioSourceReceiver == null)
			return;
		
		//LeanTween.cancel(audioSourceReceiver.gameObject);
		CancelFades();
		var tween = LeanTween.value(audioSourceReceiver.gameObject, UpdateAudioValue, audioSourceReceiver.pitch, min, time);
		fadeTweenIDs.Add(tween.uniqueId);
	}

	void Start() {
		if (fadeTweenIDs == null)
			fadeTweenIDs = new List<int>();
	}

    void UpdateAudioValue(float value){
		audioSourceReceiver.pitch = value;
	}

	void CancelFades() {
		foreach (int id in fadeTweenIDs) {
			LeanTween.cancel(audioSourceReceiver.gameObject, id);
		}

		fadeTweenIDs.Clear();
	}

}
