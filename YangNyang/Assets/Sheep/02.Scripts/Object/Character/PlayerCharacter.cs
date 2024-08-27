using UnityEngine;

public class PlayerCharacter : CharacterObject 
{
    public enum PlayerState
    {
        None,
        Idle,
        Move,
        Work,
    }

    public bool IsWorkable { get => fsm.GetCurrentState() != PlayerState.Work; }

    private Vector2 movementAmount;
    [SerializeField]
    private float controllMoveSpeed = 10;
    private StateMachine<PlayerState> fsm;

    public InteractObjectInfo currentInteractObjectInfo;

    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<PlayerState>();
        fsm.Initialize(this);
        InitializeStates();
        fsm.SetInitState(PlayerState.Idle);
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
        fsm.Update();
    }

    #region 충돌 처리 메소드. 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentInteractObjectInfo.IsEmpty())
        {
            IInteractable interactableObject = collision.GetComponent<IInteractable>();
            if (interactableObject != null)
            {
                EnterInteract(interactableObject.GetObjectInfo());
                interactableObject.EnterInteraction();
            }
        }

    }
  
    private void OnTriggerStay2D(Collider2D collision)
    {
        IInteractable interactableObject = collision.GetComponent<IInteractable>();
        if (interactableObject != null &&
             currentInteractObjectInfo.IsSameObject(interactableObject.GetObjectInfo()))
        {
            StayInteraction(interactableObject.GetObjectInfo());
            interactableObject.StayInteraction();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactableObject = collision.GetComponent<IInteractable>();
        if (interactableObject != null &&
            currentInteractObjectInfo.IsSameObject(interactableObject.GetObjectInfo()))
        {
            ExitInteraction(interactableObject.GetObjectInfo());
            interactableObject.ExitInteraction();
        }
    }

  

    /// <summary>
    /// 받은 InteractObjectInfo에 따라 플레이어의 상호작용을 결정한다.
    /// </summary>
    /// <param name="_info"></param>
    private void EnterInteract(InteractObjectInfo _info)
    {
        currentInteractObjectInfo = _info;
        // currentInteractObjectInfo 에 따라 다른 행동을 하도록 한다.
    }
    public void StayInteraction(InteractObjectInfo _info)
    {
        Debug.Log($"플레이어 작업 중");

    }
    public void ExitInteraction(InteractObjectInfo _info)
    {
        currentInteractObjectInfo = _info;
        currentInteractObjectInfo.SetEmpty();

    }
    #endregion



    protected override void InitializeStates()
    {
        fsm.AddState(PlayerState.Idle, Idle_Enter, Idle_Execute, Idle_Exit);
        fsm.AddState(PlayerState.Move, Move_Enter, Move_Execute, Move_Exit);
        fsm.AddState(PlayerState.Work, Work_Enter, Work_Execute, Work_Exit);
    }



    void OnJoystickMove(Vector2 movementAmount)
    {
        this.movementAmount = movementAmount;
    }


    #region State.Idle
    private void Idle_Enter()
    {
        //Debug.Log("Entering Idle State");    
        //GetComponent<Animator>().SetTrigger("Idle");
    }

    private void Idle_Execute()
    {
        //Debug.Log("Executing Idle State");
        if (movementAmount != Vector2.zero)
        {
            fsm.ChangeState(PlayerState.Move);
        }
    }

    private void Idle_Exit()
    {
        //Debug.Log("Exiting Idle State");
    }
    #endregion

    #region State.Move
    private void Move_Enter()
    {
        //Debug.Log("Entering Move State");
        //GetComponent<Animator>().SetTrigger("Move");
    }

    private void Move_Execute()
    {
        //Debug.Log("Executing Move State");
        // 이동 로직 추가
        if (movementAmount == Vector2.zero)
        {
            _rb2D.velocity = Vector2.zero;
            fsm.ChangeState(PlayerState.Idle);
            return;
        }

        _rb2D.velocity = movementAmount * controllMoveSpeed;
    }

    private void Move_Exit()
    {
        //Debug.Log("Exiting Move State");
    }
    #endregion

    #region State.Work
    private void Work_Enter()
    {
        //Debug.Log("Entering Work State");
        //GetComponent<Animator>().SetTrigger("Idle");
    }

    private void Work_Execute()
    {
        //Debug.Log("Executing Work State");
        _rb2D.velocity = movementAmount * controllMoveSpeed;
    }

    private void Work_Exit()
    {
        //Debug.Log("Exiting Work State");
    }


    #endregion

}
