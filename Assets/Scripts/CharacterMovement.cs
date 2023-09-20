using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private InputActions _inputActions;

    private const int MAX_JUMPS = 2;
    private const float GRAVITY = 9f;

    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpStrength;
    private int _numJumps;

    private float _gravity;
    private Vector3 _movementVelocity;
    private Vector3 _currentVelocity;
    private float _rotationDirection;

    private bool _isFloored;
    public Transform _floorRaycast;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.KeyboardMouse.Enable();
    }

    private void FixedUpdate()
    {
        HandleControls();
        HandleGravity();

        Vector3 appliedVelocity = Vector3.Lerp(_currentVelocity, _movementVelocity, Time.deltaTime );
        appliedVelocity.y = -_gravity;
        _currentVelocity = appliedVelocity;
        transform.position += _currentVelocity;

        if (_currentVelocity.sqrMagnitude > 0f)
        {
            _rotationDirection = Mathf.Atan2(_currentVelocity.z, _currentVelocity.x);
        }

        Quaternion newRotation = transform.rotation;
        newRotation.y = Mathf.LerpAngle(transform.rotation.y, _rotationDirection, Time.deltaTime);
        transform.rotation = newRotation;

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime);

        if (IsOnFloor() && _gravity > 2f && !_isFloored)
        {
            transform.localScale = new Vector3(1.25f, 0.75f, 1.25f);
        }

        _isFloored = IsOnFloor();
    }

    private void HandleControls()
    {
        Vector2 inputValue = _inputActions.KeyboardMouse.Movement.ReadValue<Vector2>();
        Debug.Log(inputValue);
        Vector3 input = Vector3.zero;
        input.x = inputValue.x;
        input.z = inputValue.y;

        _movementVelocity = input * _speed * Time.deltaTime;

        if (_inputActions.KeyboardMouse.Jump.WasPerformedThisFrame())
        {
            if (_numJumps > 0)
            {
                Jump();
                //AUdio
            }
        }
    }

    private void HandleGravity()
    {

        _gravity += GRAVITY * Time.deltaTime;
        if (_gravity > GRAVITY)
        {
            _gravity = GRAVITY;
        }

        if (_gravity > 0f && IsOnFloor())
        {
            _numJumps = MAX_JUMPS;
            _gravity = 0f;

        }
    }

    private void Jump()
    {
        _gravity = -_jumpStrength;

        _numJumps--;

        //EditorApplication.isPaused = true;
    }

    private bool IsOnFloor()
    {
        Ray ray = new Ray(transform.position,Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,1f))
        {
            return true;
        }

        Debug.DrawRay(transform.position, Vector3.down);
        return false;
    }
}
