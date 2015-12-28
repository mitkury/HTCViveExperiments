using UnityEngine;
using System.Collections;

public class ElevatorDoor : InteractiveThing
{
    public Vector3 openPosition;

    Vector3 initPosition;

    public void Open()
    {
        LeanTween.move(gameObject, openPosition, 1f);
    }

    public void OpenAfterSec(float seconds)
    {
        StartCoroutine(CoOpenAfterSec(seconds));
    }

    protected override void Start()
    {
        base.Start();

        initPosition = transform.position;
    }

    IEnumerator CoOpenAfterSec(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Open();
    }

}
