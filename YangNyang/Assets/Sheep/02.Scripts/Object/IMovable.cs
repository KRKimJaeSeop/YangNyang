using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ��ġ�� �ڵ� �̵���ų �� �ִ� ������Ʈ�� ���� �������̽�
/// </summary>
public interface IMovable
{
    void MoveToPosition(Vector2 targetPosition, float moveSpeed);
}
