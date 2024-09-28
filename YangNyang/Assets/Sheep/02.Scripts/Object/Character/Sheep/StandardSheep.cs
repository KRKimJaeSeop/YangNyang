using DG.Tweening;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
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

    [SerializeField]
    protected SpriteResolver _bodySpriteResolver;
    [SerializeField]
    protected SpriteResolver _headSpriteResolver;
    protected StateMachine<SheepState> _fsm;
    // Idle상태를 한번이라도 했다면  true가 된다.
    private bool _hasBeenIdle = false;
    private Coroutine _IdleCoroutine;
    private Coroutine _moveCoroutine;
    private WaitForSeconds _idleCheckInterval = new WaitForSeconds(1f);
    private Tween _moveTween;
    private float _jumpPower = 0;
    // 작업 코루틴
    private Coroutine _workCoroutine;
    // 작업 중간에 탈출하지않고, 완전히 작업을 완료한 경우에만 false가 된다.
    protected bool _isWorkable;
    protected SheepTableUnit _tbUnit;
    [Header("Feel")]
    [SerializeField]
    protected MMF_Player _feedback_workEnter;
    [SerializeField]
    protected MMF_Player _feedback_workExit;
    [SerializeField]
    protected MMF_Player _feedback_cry;
    [SerializeField]
    protected MMF_Player _feedback_lockSheep;
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
    public void Spawn(int id, Vector2 position, SheepState initState, Action cbDisable = null)
    {
        base.Spawn(position, cbDisable);
        _tbUnit = GameDataManager.Instance.Tables.Sheep.GetUnit(id);
        _hasBeenIdle = false;
        _isWorkable = true;
        _animator.SetFloat("BlinkSpeed", Random.Range(0.1f, 3f));
        SetSpriteResolver(_tbUnit.id);
        _jumpPower = Random.Range(-1.0f, 1.0f);
        if (!GameDataManager.Instance.Storages.UnlockSheep.IsUnlockSheepID(id))
        {
            _feedback_lockSheep.PlayFeedbacks();
            _speechBubble.Show("!!",5f);
            UIManager.Instance.OpenNotificationPanel("임시 레전드양 등장");
        }
        //_feedback_workEnter.StopFeedbacks();
        _fsm.ChangeState(initState);
    }

    protected void SetSpriteResolver(int Label)
    {
        _headSpriteResolver.SetCategoryAndLabel("EquipWool_Head", $"{Label}");
        _bodySpriteResolver.SetCategoryAndLabel("EquipWool_Body", $"{Label}");

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
        _feedback_cry.PlayFeedbacks();
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
        SetAnim_Move(true);

        // 목적지로 이동 시작
        _moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector2 spawnPosition =
         FieldObjectManager.Instance.Places.GetPlacePosition(Place.Type.SheepSpawn);
        Vector2 targetPosition =
            FieldObjectManager.Instance.Places.GetPlacePosition(Place.Type.SheepExit);

        // hasBeenIdle는 한번이라도 확률에 의해 이 코루틴을 탈출해서 Idle상태가 됐었다면 true가 되기 때문에,
        // MoveTween의 중복실행을 막는다.
        if (!_hasBeenIdle)
        {
            //// 안전한 이동을 위해 위치 이동이 완전히 확인 된 후 move시킨다.
            yield return new WaitUntil(() => _rb2D.position == spawnPosition);

            _moveTween = _rb2D.DOJump(targetPosition, _jumpPower, 1, _tbUnit.MoveSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                Despawn();
                _fsm.ChangeState(SheepState.None);
                ObjectPool.Instance.Push(gameObject.name, this.gameObject);
            });

        }



        // 이동 중 코루틴으로 시간마다 멈출지 말지 검사.
        while (true)
        {
            // 0.5초마다 검사
            yield return _idleCheckInterval;
           
            // 확률적으로 Idle 상태로 전환
            if ((!_hasBeenIdle) && Random.value < _tbUnit.IdleStateRate) // IdleStateRate 확률로 Idle 상태로 전환
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
    protected override void Flip(bool isFlip)
    {
        if (isFlip)
        {
            _transform.localScale = _originScale;
            _speechBubble.Flip(true);
        }
        else
        {
            _transform.localScale = _flipScale;
            _speechBubble.Flip(false);
        }
    }
 
    private void Move_Exit()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        SetAnim_Move(false);

    }
    #endregion

    #region State.Work
    private void Work_Enter()
    {
        _moveTween.Pause();
        _feedback_workEnter.PlayFeedbacks();
        SetAnim_Work(true);
        _workCoroutine = StartCoroutine(WorkProcess());
    }
    private IEnumerator WorkProcess()
    {
        var day = GameDataManager.Instance.Storages.User.Day;
        var speed = GameDataManager.Instance.Tables.DatyStatus.GetWorkTime(day);
        yield return new WaitForSeconds(speed);
        //작업 끝날 시 Idle로 전환한다.
        WorkComplete();
    }
    protected virtual void WorkComplete()
    {
        // 양털 아이템 뽑는다.
        FieldObjectManager.Instance.SpawnWools
            (this.transform.position, Random.Range(_tbUnit.MinWoolAmount, _tbUnit.MaxWoolAmount + 1));
        // 양털 벗은 이미지로 변환한다.
        SetSpriteResolver(0);
        // 스토리지에 등록이 안됐다면 해금.
        if (!GameDataManager.Instance.Storages.UnlockSheep.IsUnlockSheepID(_tbUnit.id))
        {
            GameDataManager.Instance.Storages.UnlockSheep.UnlockSheep(_tbUnit.id);
        }
        _isWorkable = false;
        _feedback_workExit.PlayFeedbacks();
        _fsm.ChangeState(SheepState.Move);
    }
    private void Work_Execute()
    {
        // Work 상태에서의 행동

    }

    private  void Work_Exit()
    {
        SetAnim_Work(false);

        // Work 상태 종료 시 행동
        //_spriteRenderer.color = Color.white;
        _moveTween.Play();
        _feedback_workEnter.StopFeedbacks();

        // 코루틴 테스트
        StopCoroutine(_workCoroutine);
        _workCoroutine = null;
        //Debug.Log("일 종료!");

    }

    #endregion
}