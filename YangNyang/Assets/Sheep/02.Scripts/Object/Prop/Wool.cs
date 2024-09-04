using DG.Tweening;
using System;
using UnityEngine;

public class Wool : BaseFieldObject, IMovable, IInteractable
{
    private bool _isInteractable;

    /// <summary>
    /// 지정된 속도로 지정된 위치로 던져지듯 연출하며 이동된다.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="moveSpeed"></param>
    public Tween MoveToPosition(Vector2 targetPosition, float moveSpeed, Action callback = null)
    {
        //dotween 실행 후 떨어지기
        return transform.DOJump(targetPosition, moveSpeed, 1, 1).SetEase(Ease.OutQuad).OnComplete(() => { _isInteractable = true; });
    }

    public void EnterSingleInteraction()
    {
        GetWool();
    }
    public void EnterMultipleInteraction()
    {
        GetWool();
    }
    private void GetWool()
    {
        if (_isInteractable)
        {
            _isInteractable = false;
            transform.DOMove(new Vector2(-5.61f, 6.57f), 1).SetEase(Ease.InBack).OnComplete(() =>
            {
                ObjectPool.Instance.Push(gameObject.name, this.gameObject);
                DisableGameObject();
                GameDataManager.Instance.Storages.Currency.Increase(Currency.Type.Wool, 10);
                GameDataManager.Instance.Storages.Currency.Increase(Currency.Type.Gold, 1000);
            });
        }
    }

    public void StaySingleInteraction()
    {
      
    }


    public void ExitSingleInteraction()
    {

    }

    public InteractObjectInfo GetObjectInfo()
    {
        return new InteractObjectInfo(FieldObject.Type.Wool, InstanceID);

    }
}
