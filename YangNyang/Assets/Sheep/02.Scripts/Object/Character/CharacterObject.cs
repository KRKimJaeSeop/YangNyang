using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// ��ǳ��,���¸ӽ�,�ִϸ��̼��� ���� ĳ������ ������Ʈ�� �θ��.
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
    /// �ʿ��� ���µ��� �����Ѵ�.
    /// </summary>
    protected abstract void InitializeStates();


    /// <summary>
    /// ������ ��ġ�� ������ �ð����� �ɾ�� �̵���Ű�� �̵� �� �ݹ��� �ִٸ� �����Ѵ�.
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
