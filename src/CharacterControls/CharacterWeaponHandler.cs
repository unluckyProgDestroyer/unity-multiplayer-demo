using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterWeaponHandler : NetworkBehaviour
{
    [SerializeField]
    Rigidbody2D _rigidBody;
    [SerializeField]
    Transform _weaponHoldPosition;
    [SerializeField]
    bool _useWeapon = false;
    [SyncVar]
    Weapon _weapon;
    bool _dropWeapon = false;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _dropWeapon = true;
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            _dropWeapon = false;
        }

        if (_weapon == null) return;
        _useWeapon = Input.GetButton("Fire1");
        LookAtCursor();
        _weapon.transform.position = _weaponHoldPosition.position;
    }

    void LookAtCursor()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (!hasAuthority)
        {
            return;
        }
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _weapon.transform.position;
        float rotation = Mathf.Atan2(difference.y, difference.x)*Mathf.Rad2Deg;
        if (difference.x>0)
        {
            _weapon.SetRotation(Quaternion.Euler(0f, 0f, rotation));
        }
        else
        {
            rotation = 180-rotation;
            _weapon.SetRotation(Quaternion.Euler(0f, 180f, rotation));
        }
    }

    void FixedUpdate()
    {
        CheckInputs();
    }

    void CheckInputs()
    {
        if (!hasAuthority)
        {
            return;
        }
        if (_useWeapon) UseWeapon();
        if (_dropWeapon) RemoveWeapon();
    }
    void UseWeapon()
    {
        if (_weapon!=null) _weapon.Use();
    }
    public void AddWeapon(Weapon weapon)
    {
        CmdAddWeapon(weapon);
    }

    [Command]
    void CmdAddWeapon(Weapon weapon)
    {
        if (_weapon != null)
        {
            RemoveWeapon();
        }
        weapon.ChangeOwnerServer(gameObject, _weaponHoldPosition);
        _weapon = weapon;
    }

    public void RemoveWeapon()
    {
        CmdRemoveWeapon();
    }

    [Command]
    private void CmdRemoveWeapon()
    {
        if (_weapon != null)
        {
            _weapon.ChangeOwnerServer(null, null);
            _weapon = null;
        }
    }

    public Vector2 GetVelocity()
    {
        return _rigidBody.velocity;
    }

    internal Transform GetWeaponHoldPosition()
    {
        return _weaponHoldPosition;
    }
}
