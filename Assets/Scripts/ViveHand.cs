using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class ViveHand : MonoBehaviour
{
    public float grabThreshold = 0.4f;
    public ObtainableItem itemInHand;

    int index;

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

    public void SetDeviceIndex(int index)
    {
        this.index = index;

        SetControllerIndices();
    }

    void SetControllerIndices()
    {
        var vr = SteamVR.instance;
        if (vr.hmd.GetTrackedDeviceClass((uint)index) != TrackedDeviceClass.Controller)
            return;
    }

    void Update()
    {
        var device = SteamVR_Controller.Input(index);

        /*
        foreach (var buttonId in buttonIds)
        {
            if (SteamVR_Controller.Input(index).GetPressDown(buttonId))
            {

            }
            if (SteamVR_Controller.Input(index).GetPressUp(buttonId))
            {

            }
            if (SteamVR_Controller.Input(index).GetPress(buttonId))
            {

            }
        }
        */

        foreach (var buttonId in axisIds)
        {
            if (SteamVR_Controller.Input(index).GetTouchDown(buttonId))
            {
                //Debug.Log(buttonId + " touch down");
            }
            if (SteamVR_Controller.Input(index).GetTouchUp(buttonId))
            {
                if (buttonId == EVRButtonId.k_EButton_SteamVR_Trigger)
                {
                    DropItem(device);
                } 
            }
            if (SteamVR_Controller.Input(index).GetTouch(buttonId))
            {
                var axis = SteamVR_Controller.Input(index).GetAxis(buttonId);

                if (buttonId == EVRButtonId.k_EButton_SteamVR_Trigger)
                {
                    if (itemInHand == null && axis.x >= grabThreshold && GetComponentInChildren<GrabTrigger>().things.Count > 0)
                    {
                        var thing = GetComponentInChildren<GrabTrigger>().things[0];
                        var item = thing as ObtainableItem;

                        if (item != null)
                        {
                            itemInHand = item;
                            itemInHand.Dispatch(ObtainableItemEventType.OnPickUp, this);
                        }
                        else
                        {
                            Actium.Dispatch("ViveHandInteractsWith_" + thing.name, this);
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
            if (itemInHand.owner == gameObject)
            {
                itemInHand.Dispatch(ObtainableItemEventType.OnDropOff, this);
                itemInHand.GetComponent<Rigidbody>().velocity = device.velocity * 1.4f;
            }

            itemInHand = null;
        }
    }

}
