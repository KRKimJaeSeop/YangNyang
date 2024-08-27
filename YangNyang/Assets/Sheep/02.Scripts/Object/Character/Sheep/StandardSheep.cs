using DG.Tweening;
using FieldObjectType;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class StandardSheep : CharacterObject, IInteractable
{
    public enum SheepState
    {
        // Spawn�Ǳ��� �ʱ�ȭ�� ������, �������� �����ؼ� Pull�� ���� �����̴�.
        None,
        // Ȯ�������� �� �ѹ� Idle���°� �� �� �ִ�. �����ð����� �̵��� ���߰�, �ٽ� Move���·� ��ȯ�Ѵ�.
        Idle,
        // Spawn�ǰ����� �⺻ ���´�. �����������ؼ� ��� �̵��ϰ�, �� �� �� Ȯ�������� Idle���°� �� �� �ִ�. �������� �����ϸ� None���� ��ȯ�ȴ�.
        Move,
        // �÷��̾�� �浹�ϸ� ��ȯ�ȴ�. �÷��̾ �̵��ؼ� �浹���� ���������ų�, ������ Work�ð��� ������ Move�� ��ȯ�ȴ�.
        Work,
    }

    private StateMachine<SheepState> fsm;
    private Coroutine IdleCoroutine;
    private Coroutine moveCoroutine;
    // Idle���¸� �ѹ��̶� �ߴٸ�  true�� �ȴ�.
    private bool hasBeenIdle = false;
    private Tween moveTween;


    public int instanceId;
    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<SheepState>();
        fsm.Initialize(this);
        InitializeStates();
        fsm.SetInitState(SheepState.None);
        instanceId = InstanceID;

    }
    private void Update()
    {
        fsm.Update();
    }
    protected override void InitializeStates()
    {
        fsm.AddState(SheepState.None, None_Enter, None_Execute, None_Exit);
        fsm.AddState(SheepState.Idle, Idle_Enter, Idle_Execute, Idle_Exit);
        fsm.AddState(SheepState.Move, Move_Enter, Move_Execute, Move_Exit);
        fsm.AddState(SheepState.Work, Work_Enter, Work_Execute, Work_Exit);
    }


    /// <summary>
    /// ������ ��ġ�� �����ȴ�.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="cbDisable"></param>
    public void Spawn(Vector2 position, Action cbDisable = null)
    {
        EnableGameObject(cbDisable);
        SetPosition(position);
        hasBeenIdle = false;
        fsm.ChangeState(SheepState.Move);
    }

    public InteractObjectInfo GetObjectInfo()
    {
        return new InteractObjectInfo(FieldObjectType.Type.Sheep, InstanceID);
    }

    /// <summary>
    /// �÷��̾�� ���� �� Work ���·� ��ȯ�Ѵ�.
    /// </summary>
    public virtual void EnterInteraction()
    {
        Debug.Log("�� �۾� ����");
        fsm.ChangeState(SheepState.Work);
    }

    public void ExitInteraction()
    {
        Debug.Log("�� �۾� Ż��");
        fsm.ChangeState(SheepState.Move);
    }


    #region State.None 
    private void None_Enter()
    {
    }

    private void None_Execute()
    {
        // Idle ���¿����� �ൿ
    }

    private void None_Exit()
    {
    }
    #endregion

    #region State.Idle
    private void Idle_Enter()
    {
        hasBeenIdle = true;
        // ���� �ð� �� Move ���·� ��ȯ�Ѵ�.
        IdleCoroutine = StartCoroutine(IdleToMoveCoroutine());
    }

    private IEnumerator IdleToMoveCoroutine()
    {
        yield return new WaitForSeconds(Random.value * 2);
        fsm.ChangeState(SheepState.Move);
    }

    private void Idle_Execute()
    {

    }

    private void Idle_Exit()
    {
        StopCoroutine(IdleToMoveCoroutine());
        IdleCoroutine = null;
        moveTween.Play();
    }
    #endregion

    #region State.Move
    private void Move_Enter()
    {
        // �������� �̵� ����
        moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector2 targetPosition = FieldObjectManager.Instance.sheepArrivalPosition.position;

        // hasBeenIdle�� �ѹ��̶� Ȯ���� ���� �� �ڷ�ƾ�� Ż���ؼ� Idle���°� �ƾ��ٸ� true�� �Ǳ� ������,
        // MoveTween�� �ߺ������� ���´�.
        if (!hasBeenIdle)
        {
            // rigidBody�� �������� ����� �����Ͱ���. ��Ȯ�� ������ �𸣰ڴ�.
            // SetPosition�� ����� �Ǳ� ���� �̺κ����Լ��� ���۵ż�, �� ��ġ�� ���õɶ����� ��ٷȴٰ� Move�� �����Ѵ�.
            Vector2 collectSpawnPosition = FieldObjectManager.Instance.sheepSpawnPosition.position;
            yield return new WaitUntil(() => _rb2D.position == collectSpawnPosition);

            // ��ǥ �������� �̵����Ѷ�.
            moveTween = MoveToPosition(targetPosition, 5, () =>
            {
                fsm.ChangeState(SheepState.None);
                ObjectPool.Instance.Push(gameObject.name, this.gameObject);
            });
        }


        // �̵� �� �ڷ�ƾ���� �ð����� ������ ���� �˻�.
        while (true)
        {
            // 0.5�ʸ��� �˻�
            yield return new WaitForSeconds(1f);
            // Ȯ�������� Idle ���·� ��ȯ
            if ((!hasBeenIdle) && Random.value < 0.2f) // 20% Ȯ���� Idle ���·� ��ȯ
            {
                Debug.Log("��� ����");
                moveTween.Pause();
                fsm.ChangeState(SheepState.Idle);
                yield break;
            }
        }
    }

    private void Move_Execute()
    {
        // Move ���¿����� �ൿ
    }

    private void Move_Exit()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }
    #endregion

    #region State.Work
    private void Work_Enter()
    {
        Debug.Log("���ϴ���~");
        moveTween.Pause();
        GetComponent<SpriteRenderer>().color = Color.red;
        //StartCoroutine(WorkToMoveCoroutine());
    }

    private IEnumerator WorkToMoveCoroutine()
    {
        yield return new WaitForSeconds(5);
        //fsm.ChangeState(SheepState.Move);
    }

    private void Work_Execute()
    {
        // Work ���¿����� �ൿ
    }

    private void Work_Exit()
    {
        // Work ���� ���� �� �ൿ
        GetComponent<SpriteRenderer>().color = Color.white;
        moveTween.Play();
    }




    #endregion
}