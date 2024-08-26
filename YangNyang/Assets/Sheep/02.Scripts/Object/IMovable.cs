using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ��ġ�� �ڵ� �̵���ų �� �ִ� ������Ʈ�� ���� �������̽�
/// </summary>
public interface IMovable
{
    Tween MoveToPosition(Vector2 targetPosition, float moveSpeed, Action callback = null);
}
