using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class ViveHandController : MonoBehaviour
{
	public float grabThreshold = 0.4f;

    public SteamVR_TrackedObject.EIndex index;
    public Transform origin; // if not set, relative to parent
    public bool isValid = false;

    List<int> controllerIndices = new List<int>();
	ObtainableItem itemInHand;
	
	private void OnDeviceConnected(params object[] args)
	{
        var i = (int)args[0];
        if (i != (int)this.index)
            return;

        var index = (int)args[0];
		
		var vr = SteamVR.instance;
		if (vr.hmd.GetTrackedDeviceClass((uint)index) != TrackedDeviceClass.Controller)
			return;
		
		var connected = (bool)args[1];
		if (connected)
		{
			Debug.Log(string.Format("Controller {0} connected.", index));
			PrintControllerStatus(index);
			controllerIndices.Add(index);
		}
		else
		{
			Debug.Log(string.Format("Controller {0} disconnected.", index));
			PrintControllerStatus(index);
			controllerIndices.Remove(index);
		}
	}

    private void OnNewPoses(params object[] args)
    {
        if (index == SteamVR_TrackedObject.EIndex.None)
            return;

        var i = (int)index;

        isValid = false;
        var poses = (Valve.VR.TrackedDevicePose_t[])args[0];
        if (poses.Length <= i)
            return;

        if (!poses[i].bDeviceIsConnected)
            return;

        if (!poses[i].bPoseIsValid)
            return;

        isValid = true;

        var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);

        if (origin != null)
        {
            pose = new SteamVR_Utils.RigidTransform(origin) * pose;
            pose.pos.x *= origin.localScale.x;
            pose.pos.y *= origin.localScale.y;
            pose.pos.z *= origin.localScale.z;
            transform.position = pose.pos;
            transform.rotation = pose.rot;
        }
        else
        {
            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;
        }
    }

    void OnEnable()
	{
		SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
        SteamVR_Utils.Event.Listen("new_poses", OnNewPoses);
    }
	
	void OnDisable()
	{
		SteamVR_Utils.Event.Remove("device_connected", OnDeviceConnected);
        SteamVR_Utils.Event.Remove("new_poses", OnNewPoses);
    }
	
	void PrintControllerStatus(int index)
	{
		var device = SteamVR_Controller.Input(index);
		Debug.Log("index: " + device.index);
		Debug.Log("connected: " + device.connected);
		Debug.Log("hasTracking: " + device.hasTracking);
		Debug.Log("outOfRange: " + device.outOfRange);
		Debug.Log("calibrating: " + device.calibrating);
		Debug.Log("uninitialized: " + device.uninitialized);
		Debug.Log("pos: " + device.transform.pos);
		Debug.Log("rot: " + device.transform.rot.eulerAngles);
		Debug.Log("velocity: " + device.velocity);
		Debug.Log("angularVelocity: " + device.angularVelocity);
		
		var l = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
		var r = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
		Debug.Log((l == r) ? "first" : (l == index) ? "left" : "right");
	}
	
	EVRButtonId[] buttonIds = new EVRButtonId[] {
		EVRButtonId.k_EButton_ApplicationMenu,
		EVRButtonId.k_EButton_Grip,
		EVRButtonId.k_EButton_SteamVR_Touchpad,
		EVRButtonId.k_EButton_SteamVR_Trigger
	};
	
	EVRButtonId[] axisIds = new EVRButtonId[] {
		EVRButtonId.k_EButton_SteamVR_Touchpad,
		EVRButtonId.k_EButton_SteamVR_Trigger
	};
	
	void Update()
	{
		foreach (var index in controllerIndices)
		{
			//var controllerState = SteamVR_Controller.Input((int)index).GetState();
			//var t = transform;
			//var baseTransform = new SteamVR_Utils.RigidTransform(t);

			var device = SteamVR_Controller.Input(index);

            // TODO: why it does return the position and rotation with a quite noticeable delay to a real position?
            /*
			transform.position = device.transform.pos;
			transform.rotation = device.transform.rot;
            */

			foreach (var buttonId in buttonIds)
			{
				if (SteamVR_Controller.Input(index).GetPressDown(buttonId))
				{
					if (buttonId == EVRButtonId.k_EButton_SteamVR_Trigger)
					{
						//Debug.Log("GRAB");
					}
				}
				if (SteamVR_Controller.Input(index).GetPressUp(buttonId))
				{
					/*
					Debug.Log(buttonId + " press up");
					if (buttonId == EVRButtonId.k_EButton_SteamVR_Trigger)
					{
						SteamVR_Controller.Input(index).TriggerHapticPulse();
						PrintControllerStatus(index);
					}
					*/
				}
				if (SteamVR_Controller.Input(index).GetPress(buttonId))
				{
					//Debug.Log(buttonId);
				}
			}
			
			foreach (var buttonId in axisIds)
			{
				if (SteamVR_Controller.Input(index).GetTouchDown(buttonId))
				{
					//Debug.Log(buttonId + " touch down");
				}
				if (SteamVR_Controller.Input(index).GetTouchUp(buttonId))
				{
					DropItem(device);
				}
				if (SteamVR_Controller.Input(index).GetTouch(buttonId))
				{
					var axis = SteamVR_Controller.Input(index).GetAxis(buttonId);

					if (buttonId == EVRButtonId.k_EButton_SteamVR_Trigger)
					{
						if (axis.x >= grabThreshold && GetComponentInChildren<GrabTrigger>().things.Count > 0)
						{
							var thing = GetComponentInChildren<GrabTrigger>().things[0];

							if (thing != null && thing.GetComponent<ObtainableItem>() != null && thing.GetComponent<ObtainableItem>().IsObtainable)
							{
								itemInHand = thing.GetComponent<ObtainableItem>();
								itemInHand.Dispatch(ObtainableItemEventType.OnPickUp, this);
							}

						}
						else
						{
							DropItem(device);
						}
					}
				}
			}
		}
	}

	void DropItem(SteamVR_Controller.Device device)
	{
		if (itemInHand != null)
		{
			itemInHand.Dispatch(ObtainableItemEventType.OnDropOff, this);
			itemInHand.owner = null;
			
			itemInHand.GetComponent<Rigidbody>().velocity = device.velocity * 1.4f;

			itemInHand = null;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;

		foreach (int i in controllerIndices)
		{
			var device = SteamVR_Controller.Input(i);

			Gizmos.DrawSphere(device.transform.pos, 0.05f);
			Gizmos.DrawLine(device.transform.pos, device.transform.pos + device.velocity * 1f);

		}
	}
}
