using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Elevator : Thing
{

    public ElevatorFloor currentFloor;
    public List<ElevatorFloor> floors = new List<ElevatorFloor>();
    public MovingFloor movingFloor;

    public void Elevate(int floorIndex)
    {
        currentFloor = floors[floorIndex];

        LeanTween.cancel(movingFloor.gameObject);

        Vector3 targetPosition = new Vector3(movingFloor.transform.position.x, currentFloor.transform.position.y, movingFloor.transform.position.z);

        LeanTween.move(movingFloor.gameObject, targetPosition, 20f);
    }

    public void Elevate()
    {
        int index = 1;

        if (currentFloor != null)
        {
            index = floors.IndexOf(currentFloor) + 1;
        }

        Elevate(index);
    }

    public void Elevate(object sender)
    {
        int floor = (int)sender;

        Elevate(floor);
    }

    protected override void Start()
    {
        base.Start();

        currentFloor = floors[0];
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Actium.On("RequestMovementToFloor", Elevate);
    }

}
