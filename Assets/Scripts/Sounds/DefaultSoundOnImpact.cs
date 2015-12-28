using UnityEngine;
using System.Collections;

public class DefaultSoundOnImpact : SingletonComponent<DefaultSoundOnImpact> {

	public AudioClipDataOnImpact audioClipData = new AudioClipDataOnImpact();

	public static AudioClipDataOnImpact DefaultAudioClipData {
		get {
			if (Instance != null) 
				return Instance.audioClipData;
			else 
				return new AudioClipDataOnImpact();
		}
	}

}
