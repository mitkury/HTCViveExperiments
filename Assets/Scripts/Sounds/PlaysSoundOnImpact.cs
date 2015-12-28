using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class AudioClipDataOnImpact : AudioClipData {
	public float velocityToVolume = 0.2f;
	public float requiredMinVelocity = 1f;
	public float maxVolume = 10f;
}

public class PlaysSoundOnImpact : SoundInteraction {

	public List<AudioClipDataOnImpact> audioClipsData = new List<AudioClipDataOnImpact>();
	Rigidbody _rigidbody;

	float minTimeBetweenPlayingSound = 0.2f;
	float timeSinceLastSoundPlayed;

	void Start () {
		_rigidbody = GetComponent<Rigidbody>();

		if (audioClipsData == null) {
			audioClipsData = new List<AudioClipDataOnImpact>();
		}

		if (audioClipsData.Count == 0 && DefaultSoundOnImpact.Instance != null) {
			audioClipsData.Add(DefaultSoundOnImpact.DefaultAudioClipData);
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (audioClipsData == null || audioClipsData.Count == 0)
			return;

		if (Time.time - timeSinceLastSoundPlayed < minTimeBetweenPlayingSound)
			return;

		var magnitude = collision.relativeVelocity.magnitude;
		var result = audioClipsData.Where(d => magnitude >= d.requiredMinVelocity).ToArray();

		if (result.Length == 0)
			return;

		// Get an audio with a max 'requiredMinVelocity'.
		var audioClipData = result[result.Length - 1];
		var hitVolume = audioClipData.velocityToVolume * magnitude;

		hitVolume = hitVolume > audioClipData.maxVolume ? audioClipData.maxVolume : hitVolume;

		audioClipData.volume = hitVolume;
		PlayOneShot(audioClipData);

		timeSinceLastSoundPlayed = Time.time;
	}

}
