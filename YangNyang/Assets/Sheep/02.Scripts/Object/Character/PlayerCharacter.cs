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
    #region �浹ó���� �Ѵ�. IInteractable�� ����Ͽ� ��ȣ�ۿ������Ʈ�� ������ �þ �̺κ��� ������ �ʿ� ����.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactableObject = collision.GetComponent<IInteractable>();
        if (interactableObject != null && currentInteractObjectInfo.IsEmpty())
        {
            Interact(interactableObject.GetObjectInfo());
            interactableObject.EnterInteraction();            
        }
      
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactableObject = collision.GetComponent<IInteractable>();
        if (interactableObject != null && 
            currentInteractObjectInfo.IsSameObject(interactableObject.GetObjectInfo()))
        {
            currentInteractObjectInfo.SetEmpty();
            interactableObject.ExitInteraction();
        }
       
    }
    #endregion

    /// <summary>
    /// ���� InteractObjectInfo�� ���� �÷��̾��� ��ȣ�ۿ��� �����Ѵ�.
    /// </summary>
    /// <param name="_info"></param>
    private void Interact(InteractObjectInfo _info)
    {
        currentInteractObjectInfo = _info;
        // currentInteractObjectInfo �� ���� �ٸ� �ൿ�� �ϵ��� �Ѵ�.
    }



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
        // �̵� ���� �߰�
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
