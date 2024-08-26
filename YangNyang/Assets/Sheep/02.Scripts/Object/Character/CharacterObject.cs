using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// 말풍선,상태머신,애니메이션을 쓰는 캐릭터의 오브젝트의 부모다.
/// </summary>
public abstract class CharacterObject : FieldObject, IMovable
{
    #region Animation States
    public readonly string ANIM_IDLE = "Idle";
    #endregion

    [SerializeField]
    private GameObject speechBubble;
    //public AnimationStateController AnimStateController { get { return animStateController; } }


    /// <summary>
    /// 필요한 상태들을 정의한다.
    /// </summary>
    protected abstract void InitializeStates();


    /// <summary>
    /// 지정된 위치로 지정된 시간동안 걸어가듯 이동시키고 이동 후 콜백이 있다면 실행한다.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="time"></param>
    public Tween MoveToPosition(Vector2 targetPosition, float time, Action callback = null)
    {
        return transform.DOMove(targetPosition, time).SetEase(Ease.Linear).OnComplete(() => { callback(); });
    }

    public void ShowSpeechBubble(string speechText, float showTime = 1f)
    {

    }

}
