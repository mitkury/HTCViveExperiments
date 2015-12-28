using UnityEngine;
using System.Collections;

public class BasketballHoop : Thing
{
	public AudioClipData scoreAudio = new AudioClipData();

	public override void UpdateContributor (Thing contributor)
    {
		base.UpdateContributor (contributor);

		On(contributor, SensorType.OnSensorExit, OnExitHoop);
	}

	void OnExitHoop(object sender)
    {
		CancelInvoke("Score");
		Invoke("Score", 0.1f);
	}

	void Score()
    {
		audioPlayer.PlayOneShot(scoreAudio);
	}

}
