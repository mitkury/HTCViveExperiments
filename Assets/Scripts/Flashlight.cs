using UnityEngine;
using System.Collections;

public class Flashlight : ObtainableItem
{

    public override void OnPickUp(object sender)
    {
        base.OnPickUp(sender);

        Actium.Dispatch("FlashLightIsPickedUp", sender);
    }

}
