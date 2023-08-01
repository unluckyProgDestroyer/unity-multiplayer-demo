using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    Rigidbody2D _rigidbody;
    [SerializeField]
    PoolableObject _poolableObject;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer==LayerMask.NameToLayer("Default") || collision.gameObject.layer == LayerMask.NameToLayer("Character") || collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            if (NetworkServer.active) {
                if (!_poolableObject)
                {
                    Destroy(gameObject);
                }
                _poolableObject.ReturnSelf();
            }
        }
    }

    [Server]
    public void Initialize(float mass, float speed, float damage, Vector3 position, Quaternion rotation)
    {
        this.transform.position = position;
        this.transform.rotation = rotation;
        _rigidbody.mass = mass;
        _rigidbody.velocity = transform.right * speed;
        InitializeRpc(mass, speed, damage, position, rotation);
        gameObject.SetActive(true);
    }

    [ClientRpc]
    public void InitializeRpc(float mass, float speed, float damage, Vector3 position, Quaternion rotation)
    {
        this.transform.position = position;
        this.transform.rotation = rotation;
        _rigidbody.mass = mass;
        _rigidbody.velocity = transform.right * speed;
        gameObject.SetActive(true);
    }
}
