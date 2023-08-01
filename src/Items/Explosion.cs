using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    float _explosionForce = 500;
    float _startSize = 0.01f;
    [SerializeField]
    float _endSize = 0.3f;
    [SerializeField]
    float _duration = 0.1f;
    Vector3 _sizeIncremention;
    int _stepsLeft;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        Vector2 collisionPosition = collision.transform.position;
        Vector2 selfPosition = transform.position;
        Vector2 direction = (collisionPosition - selfPosition).normalized;
        if (rb)
        {
            rb.AddForce(direction*_explosionForce);
        }
    }

    private void Awake()
    {
        int _resolution = Mathf.RoundToInt(_duration / Time.fixedDeltaTime);
        float _sizeStepAmount = (_endSize - _startSize) / _resolution;
        _stepsLeft = _resolution;
        transform.localScale = Vector3.one*_startSize;
        _sizeIncremention = Vector3.one*_sizeStepAmount;
    }

    private void FixedUpdate()
    {
        _stepsLeft--;
        transform.localScale = transform.localScale + _sizeIncremention;
        if (_stepsLeft == 0)
        {
            Destroy(gameObject);
        }
    }
}
