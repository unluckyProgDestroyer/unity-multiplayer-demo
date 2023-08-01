using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : PhysicsItem
{
    Transform _weaponPosition;
    public abstract void Use();


    protected override void ChangePhysicsParent(GameObject owner)
    {
        base.ChangePhysicsParent(owner);
        if (ownerObject == null)
        {
            _weaponPosition = null;
        }
        else
        {
            _weaponPosition = ownerObject.GetComponent<CharacterWeaponHandler>().GetWeaponHoldPosition();
        }
    }
}
