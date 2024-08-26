using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class StandardSheep : CharacterObject
{
    public enum SheepState
    {
        None,
        Idle,
        Move,
        Work,
    }

    private StateMachine<SheepState> fsm;
    private Coroutine moveCoroutine;
    private bool hasBeenIdle = false;
    private Tween moveTween;

    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<SheepState>();
        fsm.Initialize(this);
        InitializeStates();
        fsm.SetInitState(SheepState.None);

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
        fsm.ChangeState(SheepState.Move);
        hasBeenIdle = false;
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
        // ���� �ð� �� Move ���·� ��ȯ
        StartCoroutine(IdleToMoveCoroutine());
    }

    private IEnumerator IdleToMoveCoroutine()
    {
        yield return new WaitForSeconds(Random.value * 2);
        fsm.ChangeState(SheepState.Move);
    }

    private void Idle_Execute()
    {
        // Idle ���¿����� �ൿ
    }

    private void Idle_Exit()
    {
        StopCoroutine(IdleToMoveCoroutine());
        Debug.Log("�ٽ� ��");
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

        if (!hasBeenIdle)
        {
            // ��ǥ �������� �̵����Ѷ�.
            moveTween = MoveToPosition(targetPosition, 1, () =>
            {
                Debug.Log("����!");
                //moveTween.Kill();
                ObjectPool.Instance.Push(gameObject.name, this.gameObject);
            });
        }


        // �̵� �� �ڷ�ƾ���� �ð����� ������ ���� �˻�.
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
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
        }
    }
    #endregion

    #region State.Work
    private void Work_Enter()
    {
        // Work ���¿����� �ʱ�ȭ
    }

    private void Work_Execute()
    {
        // Work ���¿����� �ൿ
    }

    private void Work_Exit()
    {
        // Work ���� ���� �� �ൿ
    }
    #endregion
}