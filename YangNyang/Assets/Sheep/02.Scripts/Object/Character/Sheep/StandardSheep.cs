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
        None,// Spawn�Ǳ��� �ʱ�ȭ�� ������, �������� �����ؼ� Pull�� ���� �����̴�.
        Idle,// Ȯ�������� �� �ѹ� Idle���°� �� �� �ִ�. �����ð����� �̵��� ���߰�, �ٽ� Move���·� ��ȯ�Ѵ�.
        Move,// Spawn�ǰ����� �⺻ ���´�. �����������ؼ� ��� �̵��ϰ�, �� �� �� Ȯ�������� Idle���°� �� �� �ִ�. �������� �����ϸ� None���� ��ȯ�ȴ�.
        Work,// �÷��̾�� �浹�ϸ� ��ȯ�ȴ�. �÷��̾ �̵��ؼ� �浹���� ���������ų�, ������ Work�ð��� ������ Move�� ��ȯ�ȴ�.
    }

    [SerializeField]
    protected SpriteResolver _bodySpriteResolver;
    [SerializeField]
    protected SpriteResolver _headSpriteResolver;
    protected StateMachine<SheepState> _fsm;
    // Idle���¸� �ѹ��̶� �ߴٸ�  true�� �ȴ�.
    private bool _hasBeenIdle = false;
    private Coroutine _IdleCoroutine;
    private Coroutine _moveCoroutine;
    private WaitForSeconds _idleCheckInterval = new WaitForSeconds(1f);
    private Tween _moveTween;
    private float _jumpPower = 0;
    // �۾� �ڷ�ƾ
    private Coroutine _workCoroutine;
    // �۾� �߰��� Ż�������ʰ�, ������ �۾��� �Ϸ��� ��쿡�� false�� �ȴ�.
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
    /// ������ ��ġ�� �����ȴ�.
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
            UIManager.Instance.OpenNotificationPanel("�ӽ� ������� ����");
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
    /// �÷��̾�� ���� �� Work ���·� ��ȯ�Ѵ�.
    /// </summary>
    public virtual void EnterSingleInteraction()
    {
        if (_isWorkable)
        {
            _fsm.ChangeState(SheepState.Work);
        }
    }
    /// <summary>
    /// �÷��̾�� ������� �ٸ� ��� �۾����� ��.
    /// </summary>
    public void EnterMultipleInteraction()
    {
        // �������ϴ�~
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
        // Idle ���¿����� �ൿ
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
        // ���� �ð� �� Move ���·� ��ȯ�Ѵ�.
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

        // �������� �̵� ����
        _moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector2 spawnPosition =
         FieldObjectManager.Instance.Places.GetPlacePosition(Place.Type.SheepSpawn);
        Vector2 targetPosition =
            FieldObjectManager.Instance.Places.GetPlacePosition(Place.Type.SheepExit);

        // hasBeenIdle�� �ѹ��̶� Ȯ���� ���� �� �ڷ�ƾ�� Ż���ؼ� Idle���°� �ƾ��ٸ� true�� �Ǳ� ������,
        // MoveTween�� �ߺ������� ���´�.
        if (!_hasBeenIdle)
        {
            //// ������ �̵��� ���� ��ġ �̵��� ������ Ȯ�� �� �� move��Ų��.
            yield return new WaitUntil(() => _rb2D.position == spawnPosition);

            _moveTween = _rb2D.DOJump(targetPosition, _jumpPower, 1, _tbUnit.MoveSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                Despawn();
                _fsm.ChangeState(SheepState.None);
                ObjectPool.Instance.Push(gameObject.name, this.gameObject);
            });

        }



        // �̵� �� �ڷ�ƾ���� �ð����� ������ ���� �˻�.
        while (true)
        {
            // 0.5�ʸ��� �˻�
            yield return _idleCheckInterval;
           
            // Ȯ�������� Idle ���·� ��ȯ
            if ((!_hasBeenIdle) && Random.value < _tbUnit.IdleStateRate) // IdleStateRate Ȯ���� Idle ���·� ��ȯ
            {
                _moveTween.Pause();
                _fsm.ChangeState(SheepState.Idle);
                yield break;
            }
        }
    }

    private void Move_Execute()
    {
        // Move ���¿����� �ൿ
     

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
        //�۾� ���� �� Idle�� ��ȯ�Ѵ�.
        WorkComplete();
    }
    protected virtual void WorkComplete()
    {
        // ���� ������ �̴´�.
        FieldObjectManager.Instance.SpawnWools
            (this.transform.position, Random.Range(_tbUnit.MinWoolAmount, _tbUnit.MaxWoolAmount + 1));
        // ���� ���� �̹����� ��ȯ�Ѵ�.
        SetSpriteResolver(0);
        // ���丮���� ����� �ȵƴٸ� �ر�.
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
        // Work ���¿����� �ൿ

    }

    private  void Work_Exit()
    {
        SetAnim_Work(false);

        // Work ���� ���� �� �ൿ
        //_spriteRenderer.color = Color.white;
        _moveTween.Play();
        _feedback_workEnter.StopFeedbacks();

        // �ڷ�ƾ �׽�Ʈ
        StopCoroutine(_workCoroutine);
        _workCoroutine = null;
        //Debug.Log("�� ����!");

    }

    #endregion
}