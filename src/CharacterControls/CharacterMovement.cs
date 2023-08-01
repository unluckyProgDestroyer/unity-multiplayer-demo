using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public enum VirtualInputState
{
    None,
    Tap,
    Hold,
    Release,
}

public class CharacterMovement : NetworkBehaviour
{
    const float EPSILON = 0.01f;
    [SerializeField]
    float _maxRunForce = 200;
    [SerializeField]
    float _maxRunSpeed = 5;
    [SerializeField]
    float _maxJumpSpeed = 5;
    [SerializeField]
    float _jumpForce = 5;
    [SerializeField]
    float _maxJumpTime = 0.5f;
    [SerializeField]
    float _jumpCooldown = 0.2f;
    float _jumpCooldownTimer = 0.5f;
    float _jumpTimer = 0;

    [SerializeField]
    float _maxDashSpeed = 30;
    [SerializeField]
    float _maxDashForce = 400;
    [SerializeField]
    float _maxDashTime = 0.5f;
    [SerializeField]
    float _dashCooldown = 0.5f;
    float _dashCooldownTimer = 0.5f;
    float _dashTimer = 0;

    [SerializeField]
    Rigidbody2D _rb;
    [SerializeField]
    BoxCollider2D _groundCheckTrigger;
    [SerializeField]
    bool _grounded;
    float _vertical = 0;
    float _horizontal = 0;
    bool _jump = false;
    bool _dash = false;
    bool _stompInput = false;
    bool _stomped = false;
    bool _jumpCoolingDown = false;

    VirtualInputState _jumpInputState = VirtualInputState.None;
    VirtualInputState _dashInputState = VirtualInputState.None;

    [SerializeField]
    float _slowDownTime = 0.2f;
    [SerializeField]
    int _stompForce = 500;

    private void Awake()
    {
        _jumpCooldownTimer = 0;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            RegisterInputs();
            InputsToServerCmd(_vertical, _horizontal, _jumpInputState);
        }

        if (isServer)
        {
            CheckJump();
            CheckDash();
        }
    }

    [Command] void InputsToServerCmd(float vertical, float horizontal, VirtualInputState jumpInputState)
    {
        _vertical = vertical;
        _horizontal = horizontal;
        _jumpInputState = jumpInputState;
    }

    void RegisterInputs()
    {
        if (!isLocalPlayer) return;
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");


        RegisterJumpInput();
        _stompInput = _vertical < -0.4f;
        RegisterDashInput();
    }

    void RegisterDashInput()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _dashInputState = VirtualInputState.Tap;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            _dashInputState = VirtualInputState.Hold;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _dashInputState = VirtualInputState.Release;
        }
        else
        {
            _dashInputState = VirtualInputState.None;
        }
    }

    void RegisterJumpInput()
    {
        bool currentJumpInput = _vertical > 0.4f;
        if (currentJumpInput)
        {
            if (_jumpInputState == VirtualInputState.None || _jumpInputState == VirtualInputState.Release)
            {
                _jumpInputState = VirtualInputState.Tap;
            }
            else if (_jumpInputState == VirtualInputState.Tap)
            {
                _jumpInputState = VirtualInputState.Hold;
            }
        }
        else
        {
            if (_jumpInputState == VirtualInputState.Tap || _jumpInputState == VirtualInputState.Hold)
            {
                _jumpInputState = VirtualInputState.Release;
            }
            else if (_jumpInputState == VirtualInputState.Release)
            {
                _jumpInputState = VirtualInputState.None;
            }
        }
    }

    void CheckJump()
    {
        if (_jumpInputState == VirtualInputState.None)
        {
            return;
        }

        if (_jumpInputState == VirtualInputState.Release)
        {
            CancelJump();
            return;
        }

        if (_jumpInputState == VirtualInputState.Tap && _jumpCooldownTimer <= 0)
        {
            _jumpCoolingDown = false;
            _jump = true;
            _jumpTimer = 0;
        }
    }

    void CheckDash()
    {
        if (_dashInputState == VirtualInputState.Tap && _dashCooldownTimer <= 0)
        {
            _dash = true;
            _dashTimer = 0;
        }
        else if (_dashInputState == VirtualInputState.Release)
        {
            CancelDash();
        }
    }

    void Jump()
    {
        _jumpTimer += Time.fixedDeltaTime;
        _rb.AddForce(calculateJumpForce(_jumpForce, Time.fixedDeltaTime));
        if (_jumpTimer > _maxJumpTime)
        {
            CancelJump();
        }
    }

    void CancelJump()
    {
        if (!_jump) return;

        _jump = false;
        _jumpCooldownTimer = _jumpCooldown;
    }

    void FixedUpdate()
    {
        if (!isServer)
        {
            return;
        }
        if (_jumpCoolingDown) _jumpCooldownTimer -= Time.fixedDeltaTime;

        if (_grounded)
        {
            _stomped = false;
            _jumpCoolingDown = _jumpCooldown > 0 && !_jump;
            _jumpTimer = 0;
        }

        if (_stompInput)
        {
            Stomp();
            _stompInput = false;
        }

        if (_dash && !_stomped)
        {
            CancelJump();
            Dash();
            return;
        }
        else
        {
            _dashCooldownTimer -= Time.fixedDeltaTime;
        }

        if (_jump) Jump();

        Run();
    }
    void CancelDash()
    {
        if (!_dash) return;

        _dash = false;
        _dashCooldownTimer = _dashCooldown;
    }


    void Dash()
    {
        _dashTimer += Time.fixedDeltaTime;
        if (Math.Abs(_horizontal) > EPSILON)
        {
            _rb.AddForce(calculateHorizontalForce(_maxDashSpeed, _maxDashForce, Time.fixedDeltaTime));
        }
        if (_dashTimer > _maxDashTime)
        {
            CancelDash();
        }
    }

    void Run()
    {
        if (Math.Abs(_horizontal) > EPSILON)
        {
            _rb.AddForce(calculateHorizontalForce(_maxRunSpeed, _maxRunForce, Time.fixedDeltaTime));
        }
    }

    void Stomp()
    {
        if (!_grounded && !_stomped)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            _rb.AddForce(new Vector2(0, -_stompForce));
            _stomped = true;
        }
    }


    public void changeGrounded(bool g)
    {
        _grounded = g;
    }

    Vector2 calculateHorizontalForce(float maxSpeed, float maxForce, float deltaTime)
    {
        if (_rb.velocity.x * _horizontal > maxSpeed)
        {
            return Vector2.zero;
        }

        Vector2 _force = new Vector2(_horizontal, 0) * maxForce;
        float velocityToMaxSpeed = Math.Abs(_horizontal * maxSpeed - _rb.velocity.x);
        float deltaVelocity = maxForce * deltaTime / _rb.mass;

        if (deltaVelocity > velocityToMaxSpeed)
        {
            _force = new Vector2(_horizontal, 0) * velocityToMaxSpeed * _rb.mass / deltaTime;
        }

        return _force;
    }


    Vector2 calculateJumpForce(float maxForce, float deltaTime)
    {
        Vector2 gravityOffset = -Physics2D.gravity * _rb.gravityScale;
        Vector2 fallCounterForce = new Vector2(0, 0);
        if (_rb.velocity.y < -1)
        {
            fallCounterForce = new Vector2(0, -_rb.velocity.y / deltaTime);
        }
        float velocityToMaxSpeed = _maxJumpSpeed - _rb.velocity.y;

        if (velocityToMaxSpeed < 0)
        {
            return Vector2.zero;
        }

        float deltaVelocity = maxForce * deltaTime / _rb.mass;
        if (deltaVelocity > velocityToMaxSpeed)
        {
            maxForce = velocityToMaxSpeed * _rb.mass / deltaTime;
        }

        return new Vector2(0, _vertical) * maxForce + gravityOffset + fallCounterForce * 0.5f;
    }

}