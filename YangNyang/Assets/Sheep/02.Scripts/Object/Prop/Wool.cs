using DG.Tweening;
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
        callback = () =>
        {
            _collider2D.isTrigger = true;

        };

        //dotween ���� �� ��������
        return transform.DOJump(targetPosition, moveSpeed, 1, 1).OnComplete(() => { _collider2D.isTrigger = true; });
    }

    public void EnterInteraction()
    {
        _collider2D.isTrigger = false;
        ObjectPool.Instance.Push(gameObject.name, this.gameObject);
        // �ֿ���
    }
    public void StayInteraction()
    {

    }


    public void ExitInteraction()
    {

    }

    public InteractObjectInfo GetObjectInfo()
    {
        return new InteractObjectInfo(FieldObjectType.Type.Wool, InstanceID);

    }
}
