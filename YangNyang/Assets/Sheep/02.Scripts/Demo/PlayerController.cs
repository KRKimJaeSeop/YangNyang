using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 _movementAmount;
    [SerializeField]
    private Rigidbody2D _rb2D;
    [SerializeField]
    private float _moveSpeed = 10;


    private void OnEnable()
    {
        FloatingJoystick.OnUpdateMovement += OnJoystickMove;
    }
    private void OnDisable()
    {
        FloatingJoystick.OnUpdateMovement -= OnJoystickMove;
    }
    void OnJoystickMove(Vector2 movementAmount)
    {
        _movementAmount = movementAmount;
        if (_movementAmount == Vector2.zero)
        {
            _rb2D.velocity = Vector2.zero;
            return;
        }

        _rb2D.velocity = _movementAmount * _moveSpeed;
        Debug.Log(movementAmount);
    }

    
}
