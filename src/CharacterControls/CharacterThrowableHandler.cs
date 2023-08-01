using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterThrowableHandler : NetworkBehaviour
{
    [SerializeField]
    Rigidbody2D _rigidBody;
    [SerializeField]
    Transform _throwableHoldPosition;
    [SyncVar]
    Throwable _throwable;
    [SerializeField]
    float _throwForce = 10f;
    [SyncVar]
    bool _activated = false;
    float rotation;

    void Update()
    {
        if (_throwable == null) return;
        _throwable.transform.position = _throwableHoldPosition.position;

        if (!hasAuthority) return;

        LookAtCursor();
        if (Input.GetKeyDown(KeyCode.Alpha1)) Activate();
        if (Input.GetKeyUp(KeyCode.Alpha1)) Throw();
    }



    public void AddThrowable(Throwable throwable)
    {
        CmdAddThrowable(throwable);
    }

    [Command]
    void CmdAddThrowable(Throwable throwable)
    {
        if (_activated)
        {
            return;
        }
        if (_throwable != null)
        {
            RemoveThrowable();
        }
        throwable.ChangeOwnerServer(gameObject, _throwableHoldPosition);
        throwable.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        _throwable = throwable;
    }

    void LookAtCursor()
    {
        if (!_throwable) return;
        if (!isLocalPlayer) return;
        if (!hasAuthority) return;

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        rotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        if (difference.x > 0)
        {
            _throwable.SetRotation(Quaternion.Euler(0f, 0f, rotation));
        }
        else
        {
            rotation = 180 - rotation;
            _throwable.SetRotation(Quaternion.Euler(0f, 180f, rotation));
        }
        
    }

    void Throw()
    {
        if (_throwable)
        {
            _throwable.Throw(_throwable.transform.right * _throwForce);
            CmdThrow();
        }
    }

    [Command]
    void CmdThrow()
    {
        _activated = false;
        _throwable = null;
    }

    public void RemoveThrowable()
    {
        if (hasAuthority)
        {
            CmdRemoveThrowable();
        }
    }

    [Command]
    void CmdRemoveThrowable()
    {
        if (_throwable)
        {
            _throwable.ChangeOwnerServer(null, null);
            _throwable = null;
        }
    }

    void Activate()
    {
        if (_throwable)
        {
            _activated = true;
            CmdActivate();
            _throwable.Activate(this);
        }
    }

    [Command]
    void CmdActivate()
    {
        _activated = true;
    }

    public void ThrowableDestroyed(Throwable destroyed)
    {
        if (destroyed == _throwable)
        {
            _activated = false;
            _throwable = null;
        }
    }

    internal Transform GetWeaponHoldPosition()
    {
        return _throwableHoldPosition;
    }
}
