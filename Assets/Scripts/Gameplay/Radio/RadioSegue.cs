using UnityEngine;
using System.Collections;

public class RadioSegue : Segue {

	public Radio radio;

	public override void PerformSegue (ObtainableItem item) {
		base.PerformSegue (item);

		item.GetComponent<Rigidbody>().isKinematic = true;
		item.GetComponent<Rigidbody>().detectCollisions = false;

		radio.OpenFlap();	

		item.transform.parent = radio.transform;

		var cassettePoint = radio.cassettePoint;
		var cassetterUpPosition = cassettePoint.transform.localPosition + (cassettePoint.up + -cassettePoint.forward) * 0.2f;

		LeanTween.rotateLocal(item.gameObject, cassettePoint.localRotation.eulerAngles, 1f).setEase(LeanTweenType.easeInSine);

		LeanTween.moveLocal(item.gameObject, cassetterUpPosition, 1f).setOnComplete(delegate() {
			LeanTween.moveLocal(item.gameObject, cassettePoint.localPosition, 1f).setOnComplete(delegate() {
				radio.Play(item);
			}).setEase(LeanTweenType.easeInSine);
		}).setEase(LeanTweenType.easeInSine);

		//radio.Play(item);

	}

}
