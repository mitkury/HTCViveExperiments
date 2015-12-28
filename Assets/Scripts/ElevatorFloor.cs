using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorFloor : MonoBehaviour
{
    public List<ElevatorDoor> doors;

    public void OpenDoors(float afterSec = 0f)
    {
        foreach (var door in doors)
        {
            door.OpenAfterSec(afterSec);
        }
    }

}
