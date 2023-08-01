using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : PhysicsItem
{
    [SerializeField] GameObject _explosion;
    [SerializeField] float _fuzeTime = 3f;
    [SyncVar] bool _fuzed = false;
    [SyncVar] CharacterThrowableHandler _handler;
    Transform _throwablePosition;
    public void Activate(CharacterThrowableHandler handler)
    {
        if (hasAuthority)
        {
            _handler = handler;
            CmdActivate();
        }
    }

    protected override void ChangePhysicsParent(GameObject owner)
    {
        base.ChangePhysicsParent(owner);
        if (ownerObject == null)
        {
            _throwablePosition = null;
        }
        else
        {
            _handler = ownerObject.GetComponent<CharacterThrowableHandler>();
            _throwablePosition = ownerObject.GetComponent<CharacterThrowableHandler>().GetWeaponHoldPosition();
        }
    }

    [Command]
    void CmdActivate()
    {
        _fuzed = true;
    }

    private void FixedUpdate()
    {
        FuzeTick();
    }

    void FuzeTick()
    {
        if (_fuzed) _fuzeTime -= Time.fixedDeltaTime;
        if (_fuzeTime <= 0)
        {
            _fuzed = false;
            Explode();
        }
    }

    void Explode()
    {
        if (isServer)
        {
            _handler.ThrowableDestroyed(this);
            NetworkedSpawner.Instance.CreateObject(_explosion, transform.position);
            Dispose();
            Destroy(gameObject);
        }
    }

    [ClientRpc]
    void Dispose()
    {
        Destroy(gameObject);
    }

    public void Throw(Vector2 force)
    {
        CmdThrow(force);
    }

    [Command]
    void CmdThrow(Vector2 force)
    {
        ChangePhysicsParent(null);
        _rigidBody.AddForce(force);
        ChangeClientAuthority(null);
    }
}
