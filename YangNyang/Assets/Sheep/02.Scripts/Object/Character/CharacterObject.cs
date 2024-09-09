using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// ��ǳ��,���¸ӽ�,�ִϸ��̼��� ���� ĳ������ ������Ʈ�� �θ��.
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
    private Vector3 _originScale;
    private Vector3 _flipScale;
    protected override void Awake()
    {
        base.Awake();
        _originScale = _transform.localScale;
        _flipScale = new Vector3(-(_transform.localScale.x), _transform.localScale.y, _transform.localScale.z);

    }


    /// <summary>
    /// �ʿ��� ���¿� �� ���¸��� ������ �Լ��� �ʱ�ȭ�Ѵ�.
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
    protected void Flip(bool isFlip)
    {
        if (isFlip)
        {
            _transform.localScale = _flipScale;
            _speechBubble.Flip(true);
        }
        else
        {
            _transform.localScale = _originScale;
            _speechBubble.Flip(false);
        }

    }
    /// <summary>
    /// ������ ��ġ�� ������ �ð����� �ɾ�� �̵���Ű�� �̵� �� �ݹ��� �ִٸ� �����Ѵ�.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="time"></param>
    public Tween MoveToPosition(Vector2 targetPosition, float time, Action callback = null)
    {
        SetAnim_Move(true);
        Flip(_transform.position.x < targetPosition.x);
        return _rb2D.DOMove(targetPosition, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetAnim_Move(false);
            callback?.Invoke();
        });
    }
    public Tween MoveToPosition(Place.Type placeType, float time, Action callback = null)
    {
        SetAnim_Move(true);
        var targetPosition = FieldObjectManager.Instance.Places.GetPlacePosition(placeType);
        Flip(_transform.position.x >= targetPosition.x);
        return _rb2D.DOMove(targetPosition, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetAnim_Move(false);
            callback?.Invoke();
        });
    }
    public void ShowSpeechBubble(string speechText, float showTime = 2f, bool isTypingAnim = false, Action callback = null)
    {
        _speechBubble.Show(speechText, showTime, isTypingAnim, callback);
    }

}
