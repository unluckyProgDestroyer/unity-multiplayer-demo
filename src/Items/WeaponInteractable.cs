using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInteractable : Interactable
{
    [SerializeField]
    Weapon _weapon;
    public override void Interact(CharacterInteractionHandler caller)
    {
        base.Interact(caller);
        characterInteractionHandler.EquipWeapon(_weapon);
    }
}
