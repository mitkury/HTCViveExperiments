using UnityEngine;
using System.Collections;

public class FloorQuestFlashlight : Thing
{
    public ElevatorFloor floor;

    protected override void OnEnable()
    {
        base.OnEnable();

        Actium.On("FlashLightIsPickedUp", OnSolve);
    }

    void OnSolve(object sender)
    {
        floor.OpenDoors(2f);
    }

}
