using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularPushForceField : MonoBehaviour
{
    [SerializeField]
    float _pushForceMagnitude;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D collisionRigidBody = collision.attachedRigidbody;
        if (!collisionRigidBody) return;

        Vector2 collisionPosition = collision.transform.position;
        Vector2 selfPosition = transform.position;
        Vector2 direction = collisionPosition - selfPosition;

        collisionRigidBody.AddForce(_pushForceMagnitude*direction);
    }
}
