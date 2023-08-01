using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firearm : Weapon
{
    [SerializeField]
    ProjectileFactory projectileFactory;
    [SerializeField]
    NetworkedProjectileFactory networkedProjectileFactory;

    override public void Use()
    {
        if (!hasAuthority)
        {
            return;
        }

        networkedProjectileFactory.Trigger();
    }

}
