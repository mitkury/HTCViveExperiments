using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class SensorType : InteractiveThingEventType
{
    public static readonly SensorType OnSensorEnter = new SensorType("Thing_EntersSensor_");
    public static readonly SensorType OnSensorExit = new SensorType("Thing_ExitSensor_");

    private SensorType(string value) : base(value) { }
}

[RequireComponent(typeof(Collider))]
public class EnterExitSensor : Thing
{

    public bool registerAll;
    public List<string> registerNamesTagsTypes = new List<string>();
    public bool triggersEnter = true;
    public bool triggersExit = true;
    public bool dispatchProcessing;

    List<System.Type> registedTypes = new List<System.Type>();

    public bool CheckAgainstRegistred(MonoBehaviour target)
    {
        return registerNamesTagsTypes.Find(s => s == target.tag || s == target.name || IsSameOrSubclass(target, s)) != null;
    }

    public bool IsSameOrSubclass(object target, string potentialTypeName)
    {
        var targetType = target.GetType();
        var potentialType = System.Type.GetType(potentialTypeName);

        if (potentialType == null)
            return false;

        return potentialType.IsAssignableFrom(targetType) || targetType == potentialType;
    }

    protected override void Start()
    {
        base.Start();

        GetComponent<Collider>().isTrigger = true;

        // TODO: optimize type storing and comparing.
        /*
		foreach (var potentialTypeName in registerNamesTagsTypes) {
			var type = System.Type.GetType(potentialTypeName);
			if (type != null) {
				registedTypes.Add (type);
			}
		}
		*/
    }

    void OnTriggerEnter(Collider other)
    {
        ProcessSensorWith(other, true);
    }

    void OnTriggerExit(Collider other)
    {
        ProcessSensorWith(other, false);
    }

    protected virtual Thing ProcessSensorWith(Collider other, bool entering)
    {
        if (!triggersEnter)
            return null;

        if (other.attachedRigidbody == null)
            return null;

        // Get presumably a general Thing, not some of its children.
        var target = other.attachedRigidbody.GetComponent<Thing>();

        if (target == null)
            return null;

        if (registerAll || CheckAgainstRegistred(target))
        {
            var sensorType = entering ? SensorType.OnSensorEnter : SensorType.OnSensorExit;

            if (dispatchProcessing)
            {
                Dispatch(sensorType, target);
            }

            return target;
        }
        else
            return null;

        //return target;
    }
}
