using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableInteractable : Interactable
{
    [SerializeField]
    Throwable _throwable;
    public override void Interact(CharacterInteractionHandler caller)
    {
        base.Interact(caller);
        characterInteractionHandler.PickUpThrowable(_throwable);
    }
}
