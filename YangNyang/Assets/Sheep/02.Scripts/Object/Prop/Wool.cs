using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wool : FieldObject, IMovable
{
    /// <summary>
    /// ������ �ӵ��� ������ ��ġ�� �������� �����ϸ� �̵��ȴ�.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="moveSpeed"></param>
    public Tween MoveToPosition(Vector2 targetPosition, float moveSpeed, Action callback = null)
    {
        //dotween ���� �� ��������
        return null;
    }

}
