using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    public enum State
    {
        None,
        Idle,
        Move,
        Work,

    }

    private Vector2 _movementAmount;
    [SerializeField]
    private Rigidbody2D _rb2D;
    [SerializeField]
    private float controllMoveSpeed = 10;

    private StateMachine<State> stateMachine;

    private void Start()
    {
        stateMachine = new StateMachine<State>();
        stateMachine.Initialize(this);
        // 상태 추가 이부분 상위클래스에 abstract로 만들기.
        stateMachine.AddState(State.Idle, Idle_Enter, Idle_Execute, Idle_Exit);
        stateMachine.AddState(State.Move, Move_Enter, Move_Execute, Move_Exit);
        stateMachine.AddState(State.Work, Work_Enter, Work_Execute, Work_Exit);
        stateMachine.SetInitState(State.Idle);
    }

    private void OnEnable()
    {
        FloatingJoystick.OnUpdateMovement += OnJoystickMove;
    }
    private void OnDisable()
    {
        FloatingJoystick.OnUpdateMovement -= OnJoystickMove;
    }

    private void Update()
    {
        stateMachine.Update();
    }

    void OnJoystickMove(Vector2 movementAmount)
    {
        _movementAmount = movementAmount;
    }


    #region State.Idle
    private void Idle_Enter()
    {
        Debug.Log("Entering Idle State");
    
        //GetComponent<Animator>().SetTrigger("Idle");
    }

    private void Idle_Execute()
    {
        Debug.Log("Executing Idle State");
        if (_movementAmount != Vector2.zero)
        {
            stateMachine.ChangeState(State.Move);
        }
    }

    private void Idle_Exit()
    {
        Debug.Log("Exiting Idle State");
    }
    #endregion

    #region State.Move
    private void Move_Enter()
    {
        Debug.Log("Entering Move State");
        //GetComponent<Animator>().SetTrigger("Move");
    }

    private void Move_Execute()
    {
        Debug.Log("Executing Move State");
        // 이동 로직 추가
        if (_movementAmount == Vector2.zero)
        {
            _rb2D.velocity = Vector2.zero;
            stateMachine.ChangeState(State.Idle);
            return;
        }

        _rb2D.velocity = _movementAmount * controllMoveSpeed;
    }

    private void Move_Exit()
    {
        Debug.Log("Exiting Move State");
    }
    #endregion

    #region State.Work
    private void Work_Enter()
    {
        Debug.Log("Entering Work State");
        //GetComponent<Animator>().SetTrigger("Idle");
    }

    private void Work_Execute()
    {
        Debug.Log("Executing Work State");      
    }

    private void Work_Exit()
    {
        Debug.Log("Exiting Work State");
    }

    #endregion

}
