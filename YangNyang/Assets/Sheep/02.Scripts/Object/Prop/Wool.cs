using DG.Tweening;
using FieldObjectType;
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
        //dotween 실행 후 떨어지기
        return null;
    }

    public void EnterInteraction()
    {
        ObjectPool.Instance.Push(gameObject.name, this.gameObject);
        // 주워짐
    }

    public void ExitInteraction()
    {

    }

    public InteractObjectInfo GetObjectInfo()
    {
        return new InteractObjectInfo(FieldObjectType.Type.Wool, InstanceID);

    }
}
