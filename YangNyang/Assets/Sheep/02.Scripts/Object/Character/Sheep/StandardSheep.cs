using DG.Tweening;
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

    private StateMachine<SheepState> _fsm;
    // Idle���¸� �ѹ��̶� �ߴٸ�  true�� �ȴ�.
    private bool _hasBeenIdle = false;
    private Coroutine _IdleCoroutine;
    private Coroutine _moveCoroutine;
    private Tween _moveTween;    
    // �۾� �ڷ�ƾ
    private Coroutine _workCoroutine;
    private float _workTime = 3f;
    // �۾� �߰��� Ż�������ʰ�, ������ �۾��� �Ϸ��� ��쿡�� false�� �ȴ�.
    private bool _isWorkable;

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
    public void Spawn(Vector2 position, Action cbDisable = null)
    {
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
            FieldObjectType.Type.WorkableSheep : FieldObjectType.Type.UnWorkableSheep;

        return new InteractObjectInfo(currentType, InstanceID);
    }

    /// <summary>
    /// �÷��̾�� ���� �� Work ���·� ��ȯ�Ѵ�.
    /// </summary>
    public virtual void EnterInteraction()
    {
        if (_isWorkable)
        {
            _fsm.ChangeState(SheepState.Work);
        }
    }
    public void StayInteraction()
    {

    }

    public void ExitInteraction()
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
        _hasBeenIdle = true;
        // ���� �ð� �� Move ���·� ��ȯ�Ѵ�.
        _IdleCoroutine = StartCoroutine(IdleToMoveCoroutine());
    }

    private IEnumerator IdleToMoveCoroutine()
    {
        yield return new WaitForSeconds(Random.value * 2);
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
        // �������� �̵� ����
        _moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector2 targetPosition = FieldObjectManager.Instance.sheepArrivalPosition.position;

        // hasBeenIdle�� �ѹ��̶� Ȯ���� ���� �� �ڷ�ƾ�� Ż���ؼ� Idle���°� �ƾ��ٸ� true�� �Ǳ� ������,
        // MoveTween�� �ߺ������� ���´�.
        if (!_hasBeenIdle)
        {
            // rigidBody�� �������� ����� �����Ͱ���. ��Ȯ�� ������ �𸣰ڴ�.
            // SetPosition�� ����� �Ǳ� ���� �̺κ����Լ��� ���۵ż�, �� ��ġ�� ���õɶ����� ��ٷȴٰ� Move�� �����Ѵ�.
            Vector2 collectSpawnPosition = FieldObjectManager.Instance.sheepSpawnPosition.position;
            yield return new WaitUntil(() => _rb2D.position == collectSpawnPosition);

            // ��ǥ �������� �̵����Ѷ�.
            _moveTween = MoveToPosition(targetPosition, 5, () =>
            {
                _fsm.ChangeState(SheepState.None);
                ObjectPool.Instance.Push(gameObject.name, this.gameObject);
            });
        }


        // �̵� �� �ڷ�ƾ���� �ð����� ������ ���� �˻�.
        while (true)
        {
            // 0.5�ʸ��� �˻�
            yield return new WaitForSeconds(1f);
            // Ȯ�������� Idle ���·� ��ȯ
            if ((!_hasBeenIdle) && Random.value < 0.1f) // 10% Ȯ���� Idle ���·� ��ȯ
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
        yield return new WaitForSeconds(_workTime);
        //�۾� ���� �� Idle�� ��ȯ�Ѵ�.
        _isWorkable = false;
        _fsm.ChangeState(SheepState.Move);
    }

    private void Work_Execute()
    {
        // Work ���¿����� �ൿ
    }

    private void Work_Exit()
    {
        // Work ���� ���� �� �ൿ
        _spriteRenderer.color = Color.white;
        _moveTween.Play();

        // �ڷ�ƾ �׽�Ʈ
        StopCoroutine(_workCoroutine);
        _workCoroutine = null;
    }

    #endregion
}