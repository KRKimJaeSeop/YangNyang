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
    /// 지정된 위치에 스폰된다.
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
        // Idle 상태에서의 행동
    }

    private void None_Exit()
    {
    }
    #endregion

    #region State.Idle
    private void Idle_Enter()
    {
        hasBeenIdle = true;
        // 일정 시간 후 Move 상태로 전환
        StartCoroutine(IdleToMoveCoroutine());
    }

    private IEnumerator IdleToMoveCoroutine()
    {
        yield return new WaitForSeconds(Random.value * 2);
        fsm.ChangeState(SheepState.Move);
    }

    private void Idle_Execute()
    {
        // Idle 상태에서의 행동
    }

    private void Idle_Exit()
    {
        StopCoroutine(IdleToMoveCoroutine());
        Debug.Log("다시 가");
        moveTween.Play();
    }
    #endregion

    #region State.Move
    private void Move_Enter()
    {
        // 목적지로 이동 시작
        moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector2 targetPosition = FieldObjectManager.Instance.sheepArrivalPosition.position;

        if (!hasBeenIdle)
        {
            // 목표 지점까지 이동시켜라.
            moveTween = MoveToPosition(targetPosition, 1, () =>
            {
                Debug.Log("도착!");
                //moveTween.Kill();
                ObjectPool.Instance.Push(gameObject.name, this.gameObject);
            });
        }


        // 이동 중 코루틴으로 시간마다 멈출지 말지 검사.
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            // 0.5초마다 검사
            yield return new WaitForSeconds(1f);
            // 확률적으로 Idle 상태로 전환
            if ((!hasBeenIdle) && Random.value < 0.2f) // 20% 확률로 Idle 상태로 전환
            {
                Debug.Log("잠깐 멈춰");
                moveTween.Pause();
                fsm.ChangeState(SheepState.Idle);
                yield break;
            }
        }
    }

    private void Move_Execute()
    {
        // Move 상태에서의 행동
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
        // Work 상태에서의 초기화
    }

    private void Work_Execute()
    {
        // Work 상태에서의 행동
    }

    private void Work_Exit()
    {
        // Work 상태 종료 시 행동
    }
    #endregion
}