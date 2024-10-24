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
    [SerializeField,Tooltip("말풍선 부모 트랜스폼.없다면 미세팅")]
    protected Transform _speechBubbleParent;
    [SerializeField]
    protected SpriteRenderer _dropShadow;

    [SerializeField]
    protected Animator _animator;
    protected Vector3 _originScale;
    protected Vector3 _flipScale;


    protected override void Awake()
    {
        base.Awake();
        _originScale = _transform.localScale;
        _flipScale = new Vector3(-(_transform.localScale.x), _transform.localScale.y, _transform.localScale.z);

        if (_speechBubbleParent == null)
        {
            _speechBubble = Instantiate(
               AddressableManager.Instance.GetAsset<GameObject>(AddressableManager.RemoteAssetCode.SpeechBubble), this.transform).
               GetComponent<SpeechBubble>();
        }
        else
        {
            _speechBubble = Instantiate(
              AddressableManager.Instance.GetAsset<GameObject>(AddressableManager.RemoteAssetCode.SpeechBubble), _speechBubbleParent).
              GetComponent<SpeechBubble>();
        }
        _speechBubble.gameObject.SetActive(false);
        _dropShadow.sprite =
            AddressableManager.Instance.GetAsset<Sprite>(AddressableManager.RemoteAssetCode.DropShadow);
        //_dropShadow.transform.localScale = Vector3.one;
        //_dropShadow.transform.localPosition= Vector3.zero;
    }


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
    protected abstract void Flip(bool isFlip);
    /// <summary>
    /// 지정된 위치로 지정된 시간동안 걸어가듯 이동시키고 이동 후 콜백이 있다면 실행한다.
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
