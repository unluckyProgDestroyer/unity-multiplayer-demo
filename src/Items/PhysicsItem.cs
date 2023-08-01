using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsItem : NetworkBehaviour
{
    [SyncVar(hook = nameof(rotationChanged))]
    Quaternion _rotation;

    protected Rigidbody2D _parentRigidBody;
    [SyncVar(hook =nameof(ownerObjectChanged))]
    protected GameObject ownerObject;
    [SerializeField]
    protected Rigidbody2D _rigidBody;
    [SerializeField]
    Collider2D _collider;
    [SerializeField]
    GameObject _interactable;
    
    [ServerCallback]
    public void ChangeOwnerServer(GameObject owner, Transform weaponPosition)
    {
        ownerObject = owner;
        ChangePhysicsParent(owner);
        ChangeClientAuthority(owner);
    }



    [Command]
    protected void CmdChangePhysicsParent(GameObject owner)
    {
        ChangePhysicsParent(owner);
    }

    [Command]
    protected void CmdChangeClientAuthority(GameObject owner)
    {
        ChangeClientAuthority(owner);
    }

    protected virtual void ChangePhysicsParent(GameObject owner)
    {
        if (owner == null)
        {
            EnablePhysics();
            _interactable.SetActive(true);
        }
        else
        {
            _parentRigidBody = owner.GetComponent<Rigidbody2D>();
            DisablePhysics();
            _interactable.SetActive(false);
        }
    }

    protected void ChangeClientAuthority(GameObject owner)
    {
        ownerObject = owner;
        NetworkIdentity networkIdentity = gameObject.GetComponent<NetworkIdentity>();
        if (owner == null)
        {
            networkIdentity.RemoveClientAuthority();
        }
        else
        {
            networkIdentity.RemoveClientAuthority();
            NetworkIdentity ownersNetworkIdentity = owner.GetComponent<NetworkIdentity>();
            networkIdentity.AssignClientAuthority(ownersNetworkIdentity.connectionToClient);
        }
    }

    void DisablePhysics()
    {
        _rigidBody.bodyType = RigidbodyType2D.Kinematic;
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.angularVelocity = 0f;
        GetComponent<NetworkTransform>().enabled = false;
        _collider.enabled = false;
    }
    void EnablePhysics()
    {
        _rigidBody.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<NetworkTransform>().enabled = true;
        _collider.enabled = true;
    }

    void ownerObjectChanged(GameObject oldOwner, GameObject newOwner)
    {
        ChangePhysicsParent(newOwner);
    }

    [Command]
    public void SetRotation(Quaternion rotation)
    {
        _rotation = rotation;
        transform.rotation = rotation;
    }

    private void rotationChanged(Quaternion oldRotation, Quaternion newRotation)
    {
        transform.rotation = newRotation;
    }
}
