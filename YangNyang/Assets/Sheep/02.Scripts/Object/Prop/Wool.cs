using DG.Tweening;
using FieldObjectType;
using System;
using UnityEngine;

public class Wool : FieldObject, IMovable, IInteractable
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

    public void EnterInteraction()
    {
        ObjectPool.Instance.Push(gameObject.name, this.gameObject);
        // �ֿ���
    }

    public void ExitInteraction()
    {

    }

    public InteractObjectInfo GetObjectInfo()
    {
        return new InteractObjectInfo(FieldObjectType.Type.Wool, InstanceID);

    }
}
