using UnityEngine;
using System.Collections;

public class MouseCameraControl : MonoBehaviour {

	public MouseLookSimple mouseLook;
	public Camera targetCamera;

	void Start () {
		mouseLook.Init(transform, targetCamera.transform);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update () {
		#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.LeftAlt))
			return;

		if (Input.GetMouseButtonDown(0)) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		#endif

		mouseLook.LookRotation(transform, targetCamera.transform);
	}
	
}
