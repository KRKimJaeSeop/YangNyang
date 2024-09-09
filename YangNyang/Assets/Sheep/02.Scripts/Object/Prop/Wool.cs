using DG.Tweening;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wool : BaseFieldObject, IMovable, IInteractable
{
    private bool _isInteractable;
    private Vector2 dropPoint;

    public override void Spawn(Vector2 startPosition , Action cbDisable = null)
    {
        base.Spawn(startPosition, cbDisable);

        float randomX = Random.Range(FieldObjectManager.Instance.Places.GetPlacePosition
        (Place.Type.WoolDropZone_BottomLeftCorner).x,
        FieldObjectManager.Instance.Places.GetPlacePosition(Place.Type.WoolDropZone_TopRightCorner).x);

        float randomY = Random.Range(FieldObjectManager.Instance.Places.GetPlacePosition
         (Place.Type.WoolDropZone_BottomLeftCorner).y,
         FieldObjectManager.Instance.Places.GetPlacePosition(Place.Type.WoolDropZone_TopRightCorner).y);
        
        dropPoint.Set(randomX, randomY);
        MoveToPosition(dropPoint, 6);
    }
 

    /// <summary>
    /// 지정된 속도로 지정된 위치로 던져지듯 연출하며 이동된다.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="moveSpeed"></param>
    public Tween MoveToPosition(Vector2 targetPosition, float moveSpeed, Action callback = null)
    {
        return _transform.DOJump(targetPosition, moveSpeed, 1, 1).SetEase(Ease.OutQuad).OnComplete(() => { _isInteractable = true; });
    }

    public void EnterSingleInteraction()
    {
        PickUpWool();
    }
    public void EnterMultipleInteraction()
    {
        PickUpWool();
    }
    private void PickUpWool()
    {
        if (_isInteractable)
        {
            _isInteractable = false;
            _rb2D.DOMove(new Vector2(-5.61f, 6.57f), 1).SetEase(Ease.InBack).OnComplete(() =>
            {
                ObjectPool.Instance.Push(gameObject.name, this.gameObject);
                GameDataManager.Instance.Storages.Currency.Increase(Currency.Type.Wool, 10);
                Despawn();
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
