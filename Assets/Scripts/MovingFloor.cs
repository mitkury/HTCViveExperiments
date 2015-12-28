using UnityEngine;
using System.Collections;

public class MovingFloor : MonoBehaviour
{
    public Transform target;

    Vector3 diffBetweenTarget;

    void Start()
    {
        diffBetweenTarget = transform.position - target.position;
    }

    void Update()
    {
        //target.position = transform.position - diffBetweenTarget;

        target.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }

}
