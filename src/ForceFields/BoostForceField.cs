using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostForceField : MonoBehaviour
{
    [SerializeField]
    float _pushForceMagnitude;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D collisionRigidBody = collision.attachedRigidbody;
        if (!collisionRigidBody) return;
        Vector2 direction = transform.up;

        collisionRigidBody.AddForce(_pushForceMagnitude * direction);
    }
}
