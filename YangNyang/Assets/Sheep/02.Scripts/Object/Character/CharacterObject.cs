using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// 말풍선,상태머신,애니메이션을 쓰는 캐릭터의 오브젝트의 부모다.
/// </summary>
public abstract class CharacterObject : BaseFieldObject, IMovable
{
    #region Animation Parameters
    public readonly string ANIM_IS_WORK = "IsWork";
    public readonly string ANIM_IS_MOVE = "IsMove";
    #endregion

    [SerializeField]
    protected SpeechBubble _speechBubble;

    [SerializeField]
    protected Animator _animator;
    //public AnimationStateController AnimStateController { get { return animStateController; } }


    /// <summary>
    /// 필요한 상태와 각 상태마다 실행할 함수를 초기화한다.
    /// </summary>
    protected abstract void InitializeStates();

    protected virtual void SetAnim_Work(bool isWork)
    {
        _animator.SetBool(ANIM_IS_WORK, isWork);
    }
    protected virtual void SetAnim_Move(bool isMove)
    {
        _animator.SetBool(ANIM_IS_MOVE, isMove);
    }
    /// <summary>
    /// 지정된 위치로 지정된 시간동안 걸어가듯 이동시키고 이동 후 콜백이 있다면 실행한다.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="time"></param>
    public Tween MoveToPosition(Vector2 targetPosition, float time, Action callback = null)
    {
        SetAnim_Move(true);
        return _rb2D.DOMove(targetPosition, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetAnim_Move(false);
            callback?.Invoke();
        });
    }
    public Tween MoveToPosition(Place.Type placeType, float time, Action callback = null)
    {
        SetAnim_Move(true);
        var position = FieldObjectManager.Instance.Places.GetPlacePosition(placeType);
        return _rb2D.DOMove(position, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetAnim_Move(false);
            callback?.Invoke();
        });
    }

    public void ShowSpeechBubble(string speechText, float showTime = 2f, bool isTypingAnim = false,Action callback = null)
    {
        _speechBubble.Show(speechText, showTime, isTypingAnim, callback);
    }

}
