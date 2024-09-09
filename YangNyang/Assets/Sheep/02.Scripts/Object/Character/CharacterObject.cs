using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// ��ǳ��,���¸ӽ�,�ִϸ��̼��� ���� ĳ������ ������Ʈ�� �θ��.
/// </summary>
public abstract class CharacterObject : BaseFieldObject, IMovable
{
    #region Animation States
    public readonly string ANIM_IDLE = "Idle";
    #endregion

    [SerializeField]
    protected SpeechBubble _speechBubble;

    [SerializeField]
    protected Animator _animator;
    //public AnimationStateController AnimStateController { get { return animStateController; } }


    /// <summary>
    /// �ʿ��� ���¿� �� ���¸��� ������ �Լ��� �ʱ�ȭ�Ѵ�.
    /// </summary>
    protected abstract void InitializeStates();

    protected virtual void SetAnim_Work(bool isWork)
    {
        _animator.SetBool("IsWork", isWork);
    }
    protected virtual void SetAnim_Move(bool isMove)
    {
        _animator.SetBool("IsMove", isMove);
    }
    /// <summary>
    /// ������ ��ġ�� ������ �ð����� �ɾ�� �̵���Ű�� �̵� �� �ݹ��� �ִٸ� �����Ѵ�.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="time"></param>
    public Tween MoveToPosition(Vector2 targetPosition, float time, Action callback = null)
    {
        SetAnim_Move(true);
        return transform.DOMove(targetPosition, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetAnim_Move(false);
            callback?.Invoke();
        });
    }
    public Tween MoveToPosition(Place.Type placeType, float time, Action callback = null)
    {
        SetAnim_Move(true);
        var position = FieldObjectManager.Instance.Places.GetPlacePosition(placeType);
        return transform.DOMove(position, time).SetEase(Ease.Linear).OnComplete(() =>
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
