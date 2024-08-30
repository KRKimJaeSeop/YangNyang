using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class StandardSheep : CharacterObject, IInteractable
{
    public enum SheepState
    {
        None,// Spawn되기전 초기화만 됐을때, 목적지에 도착해서 Pull될 때의 상태이다.
        Idle,// 확률적으로 단 한번 Idle상태가 될 수 있다. 일정시간동안 이동을 멈추고, 다시 Move상태로 전환한다.
        Move,// Spawn되고나서의 기본 상태다. 목적지를향해서 계속 이동하고, 단 한 번 확률적으로 Idle상태가 될 수 있다. 목적지에 도달하면 None으로 전환된다.
        Work,// 플레이어에게 충돌하면 전환된다. 플레이어가 이동해서 충돌에서 빠져나오거나, 정해진 Work시간이 지나면 Move로 전환된다.
    }

    private StateMachine<SheepState> _fsm;
    // Idle상태를 한번이라도 했다면  true가 된다.
    private bool _hasBeenIdle = false;
    private Coroutine _IdleCoroutine;
    private Coroutine _moveCoroutine;
    private Tween _moveTween;
    // 작업 코루틴
    private Coroutine _workCoroutine;
    private float _workTime = 0.5f;
    // 작업 중간에 탈출하지않고, 완전히 작업을 완료한 경우에만 false가 된다.
    private bool _isWorkable;

    private SheepTableUnit _tbUnit;

    protected override void Awake()
    {
        base.Awake();
        _fsm = new StateMachine<SheepState>();
        _fsm.Initialize(this);
        InitializeStates();
        _fsm.SetInitState(SheepState.None);

    }
    protected override void InitializeStates()
    {
        _fsm.AddState(SheepState.None, None_Enter, None_Execute, None_Exit);
        _fsm.AddState(SheepState.Idle, Idle_Enter, Idle_Execute, Idle_Exit);
        _fsm.AddState(SheepState.Move, Move_Enter, Move_Execute, Move_Exit);
        _fsm.AddState(SheepState.Work, Work_Enter, Work_Execute, Work_Exit);
    }

    private void Update()
    {
        _fsm.Update();
    }

    /// <summary>
    /// 지정된 위치에 스폰된다.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="cbDisable"></param>
    public void Spawn(int id, Vector2 position, Action cbDisable = null)
    {
        _tbUnit = GameDataManager.Instance.Tables.Sheep.GetUnit(id);
        EnableGameObject(cbDisable);
        SetPosition(position);
        _hasBeenIdle = false;
        _isWorkable = true;
        _fsm.ChangeState(SheepState.Move);
    }

    #region IInteractable
    public InteractObjectInfo GetObjectInfo()
    {
        var currentType = _isWorkable ?
            FieldObject.Type.WorkableSheep : FieldObject.Type.UnWorkableSheep;

        return new InteractObjectInfo(currentType, InstanceID);
    }

    /// <summary>
    /// 플레이어와 닿을 시 Work 상태로 전환한다.
    /// </summary>
    public virtual void EnterSingleInteraction()
    {
        if (_isWorkable)
        {
            _fsm.ChangeState(SheepState.Work);
        }
    }
    /// <summary>
    /// 플레이어와 닿았지만 다른 양과 작업중일 시.
    /// </summary>
    public void EnterMultipleInteraction()
    {
        // 지나갑니다~
    }
    public void StaySingleInteraction()
    {

    }

    public void ExitSingleInteraction()
    {
        _fsm.ChangeState(SheepState.Move);
    }
    #endregion

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
        _hasBeenIdle = true;
        // 일정 시간 후 Move 상태로 전환한다.
        _IdleCoroutine = StartCoroutine(IdleToMoveCoroutine());
    }

    private IEnumerator IdleToMoveCoroutine()
    {
        yield return new WaitForSeconds(_tbUnit.IdleTime);
        _fsm.ChangeState(SheepState.Move);
    }

    private void Idle_Execute()
    {

    }

    private void Idle_Exit()
    {
        StopCoroutine(_IdleCoroutine);
        _IdleCoroutine = null;
        _moveTween.Play();
    }
    #endregion

    #region State.Move
    private void Move_Enter()
    {
        // 목적지로 이동 시작
        _moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector2 spawnPosition =
         FieldObjectManager.Instance.Places.GetPlacePosition(PlaceData.Type.SheepSpawn);
        Vector2 targetPosition =
            FieldObjectManager.Instance.Places.GetPlacePosition(PlaceData.Type.SheepExit);

        // hasBeenIdle는 한번이라도 확률에 의해 이 코루틴을 탈출해서 Idle상태가 됐었다면 true가 되기 때문에,
        // MoveTween의 중복실행을 막는다.
        if (!_hasBeenIdle)
        {
            // rigidBody의 포지션의 계산이 느린것같다. 정확한 원인은 모르겠다.
            // SetPosition이 제대로 되기 전에 이부분의함수가 시작돼서, 제 위치로 세팅될때까지 기다렸다가 Move를 시작한다.
            yield return new WaitUntil(() => _rb2D.position == spawnPosition);

            // 목표 지점까지 이동시켜라.
            _moveTween = MoveToPosition(targetPosition, _tbUnit.MoveSpeed, () =>
            {
                _fsm.ChangeState(SheepState.None);
                ObjectPool.Instance.Push(gameObject.name, this.gameObject);
            });
        }


        // 이동 중 코루틴으로 시간마다 멈출지 말지 검사.
        while (true)
        {
            // 0.5초마다 검사
            yield return new WaitForSeconds(1f);
            // 확률적으로 Idle 상태로 전환
            if ((!_hasBeenIdle) && Random.value < _tbUnit.IdleStateRate) // 10% 확률로 Idle 상태로 전환
            {
                _moveTween.Pause();
                _fsm.ChangeState(SheepState.Idle);
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
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }
    #endregion

    #region State.Work
    private void Work_Enter()
    {
        _moveTween.Pause();
        _spriteRenderer.color = Color.red;
        _workCoroutine = StartCoroutine(WorkProcess());
    }
    private IEnumerator WorkProcess()
    {
        var day = GameDataManager.Instance.Storages.User.Day;
        var speed = GameDataManager.Instance.Tables.DatyStatus.GetWorkTime(day);
        Debug.Log($"day: {day} / speed : {speed}");
        yield return new WaitForSeconds(speed);
        //작업 끝날 시 Idle로 전환한다.
        WorkCompletet();
    }
    private void WorkCompletet()
    {
        // 양털 뽑기
        FieldObjectManager.Instance.SpawnWool
            (this.transform.position, Random.Range(_tbUnit.MinWoolAmount, _tbUnit.MaxWoolAmount + 1));

        // 양털 벗은 이미지로 변환

        //상태 전환
        _isWorkable = false;
        _fsm.ChangeState(SheepState.Move);
    }
    private void Work_Execute()
    {
        // Work 상태에서의 행동

    }

    private void Work_Exit()
    {
        // Work 상태 종료 시 행동
        _spriteRenderer.color = Color.white;
        _moveTween.Play();

        // 코루틴 테스트
        StopCoroutine(_workCoroutine);
        _workCoroutine = null;
    }

    #endregion
}