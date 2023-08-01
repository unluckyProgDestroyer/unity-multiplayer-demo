using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected CharacterInteractionHandler characterInteractionHandler;
    public virtual void Interact(CharacterInteractionHandler caller)
    {
        characterInteractionHandler = caller;
    }
}
