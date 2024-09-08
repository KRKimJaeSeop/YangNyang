using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTest : MonoBehaviour
{
    public Rigidbody2D _rb2D;

    public bool isRgMove;

    private void OnEnable()
    {
        SetPosition(Vector2.down);

    }
    private void OnDisable()
    {
        SetPosition(Vector2.up*5);

    }
    protected void SetPosition(Vector2 position)
    {
        if (isRgMove)
        {
            _rb2D.MovePosition(position);
        }
        else
            transform.position = position;
    }

}
