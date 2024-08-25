using UnityEngine;

public abstract class CharacterBase : MonoBehaviour, IMovable
{
    [SerializeField]
    private GameObject speechBubble;

    /// <summary>
    /// �ʿ��� ���µ��� �����Ѵ�.
    /// </summary>
    protected abstract void InitializeStates();


    /// <summary>
    /// ������ ��ġ�� �̵���Ų��. 
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
