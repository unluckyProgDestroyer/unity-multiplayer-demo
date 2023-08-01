using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularPullForceField : MonoBehaviour
{
    [SerializeField]
    float _maxPullForce = 50;
    [SerializeField]
    float _counterForceMultiplier = 1;
    [SerializeField]
    CircleCollider2D _circleCollider;
    LagrangeCurve strengthCurve = new LagrangeCurve();
    private void Awake()
    {
        strengthCurve.AddControlPoint(new Vector2(0f, 1f));
        strengthCurve.AddControlPoint(new Vector2(0.1f, 0.5f));
        strengthCurve.AddControlPoint(new Vector2(0.33f, 0.30f));
        strengthCurve.AddControlPoint(new Vector2(1f, 0.2f));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D collisionRigidBody = collision.attachedRigidbody;
        if (!collisionRigidBody) return;

        float pullRange = _circleCollider.radius * transform.localScale.x;
        float maxForceRadius = pullRange * 0.1f;
        float distanceDependentForceRange = pullRange - maxForceRadius;

        Vector2 collisionPosition = collision.transform.position;
        Vector2 selfPosition = transform.position;

        Vector2 positionDifference = selfPosition - collisionPosition;
        float positionDifferenceMagnitude = positionDifference.magnitude;

        if (positionDifferenceMagnitude > pullRange)
        {
            return;
        }

        float pullMultiplier;

        if (positionDifferenceMagnitude < maxForceRadius)
        {
            pullMultiplier = 1f;
        }
        else
        {
            float distanceProportion = (positionDifferenceMagnitude - maxForceRadius) / distanceDependentForceRange;
            pullMultiplier = strengthCurve.Point(distanceProportion).y;
        }
        Vector2 resultant;

        Vector2 pullInForce = CalculateMaxForceToPullInDeltaTime(positionDifference, collisionRigidBody.mass, Time.fixedDeltaTime);
        Vector2 counterForce = CalculateCounterForce(collisionRigidBody) * _counterForceMultiplier;

        resultant = pullInForce+counterForce;
        if (_maxPullForce<resultant.magnitude)
        {
            resultant = resultant.normalized*_maxPullForce;
        }

        collisionRigidBody.AddForce(resultant * pullMultiplier);
    }

    Vector2 CalculateMaxForceToPullInDeltaTime(Vector2 distance, float mass, float deltaTime)
    {
        return distance / (deltaTime * deltaTime) * mass;
    }

    Vector2 CalculateCounterForce(Rigidbody2D collisionRigidBody)
    {
        Vector2 stopForce = -collisionRigidBody.velocity * collisionRigidBody.mass / Time.fixedDeltaTime;
        return stopForce;
    }

}
 
 public static class Vector2Extension
{
    public static float AngleFromAxisX(this Vector2 v)
    {
        return Mathf.Acos(v.x / v.magnitude)*Mathf.Rad2Deg;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(-degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(-degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        
        return v;
    }
}
