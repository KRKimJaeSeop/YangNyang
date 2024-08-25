using UnityEngine;

public abstract class CharacterBase : MonoBehaviour, IMovable
{
    [SerializeField]
    private GameObject speechBubble;

    /// <summary>
    /// 필요한 상태들을 정의한다.
    /// </summary>
    protected abstract void InitializeStates();


    /// <summary>
    /// 지정된 위치로 이동시킨다. 
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="moveSpeed"></param>
    public void MoveToPosition(Vector2 targetPosition, float moveSpeed = 5.0f)
    {

    }

    public void SpeechBubble()
    {

    }

}
