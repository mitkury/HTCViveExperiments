using UnityEngine;
using System.Collections;

public class ElevatorButton : InteractiveThing
{
    public int targetFloor;

    protected override void OnEnable()
    {
        base.OnEnable();

        Actium.On("ViveHandInteractsWith_" + gameObject.name, OnInteractionStart);
    }

    public override void OnInteractionStart(object sender)
    {
        base.OnInteractionStart(sender);

        Actium.Dispatch("RequestMovementToFloor", targetFloor);
    }

}