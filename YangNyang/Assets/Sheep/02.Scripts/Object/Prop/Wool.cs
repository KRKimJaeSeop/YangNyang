using DG.Tweening;
using System;
using UnityEngine;

public class Wool : FieldObject, IMovable, IInteractable
{
    /// <summary>
    /// 지정된 속도로 지정된 위치로 던져지듯 연출하며 이동된다.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="moveSpeed"></param>
    public Tween MoveToPosition(Vector2 targetPosition, float moveSpeed, Action callback = null)
    {
        callback = () =>
        {
            _collider2D.isTrigger = true;

        };

        //dotween 실행 후 떨어지기
        return transform.DOJump(targetPosition, moveSpeed, 1, 1).OnComplete(() => { _collider2D.isTrigger = true; });
    }

    public void EnterInteraction()
    {
        _collider2D.isTrigger = false;
        ObjectPool.Instance.Push(gameObject.name, this.gameObject);
        // 주워짐
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
