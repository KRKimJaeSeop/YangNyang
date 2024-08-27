using System.Collections;
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

    private StateMachine<PlayerState> fsm;
    [SerializeField, Tooltip("���̽�ƽ ���۽� �̵��ӵ�")]
    private float controllMoveSpeed = 10;
    private Vector2 movementAmount;
    // ��ȣ�ۿ����� IInteractable ���ӿ�����Ʈ�� ����
    public InteractObjectInfo currentInteractObjectInfo;



    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<PlayerState>();
        fsm.Initialize(this);
        InitializeStates();
        fsm.SetInitState(PlayerState.Idle);
    }
    protected override void InitializeStates()
    {
        fsm.AddState(PlayerState.Idle, Idle_Enter, Idle_Execute, Idle_Exit);
        fsm.AddState(PlayerState.Move, Move_Enter, Move_Execute, Move_Exit);
        fsm.AddState(PlayerState.Work, Work_Enter, Work_Execute, Work_Exit);
    }

    private void OnEnable()
    {
        FloatingJoystick.OnUpdateMovement += OnJoystickMove;
    }

    private void OnDisable()
    {
        FloatingJoystick.OnUpdateMovement -= OnJoystickMove;
    }

    void OnJoystickMove(Vector2 movementAmount)
    {
        this.movementAmount = movementAmount;
    }

    private void Update()
    {
        fsm.Update();
    }

    #region �浹 ó�� �޼ҵ�.

    /// <summary>
    /// �÷��̾ Ʈ���� �ݶ��̴��� ���ų� �ӹ����ų� ���� ��
    /// IInteractable �������̽��� ������ ��ü���� ��ȣ�ۿ븸�� ó���Ѵ�.
    /// </summary>
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
             currentInteractObjectInfo.IsSameIDObject(interactableObject.GetObjectInfo()))
        {
            StayInteraction(interactableObject.GetObjectInfo());
            interactableObject.StayInteraction();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactableObject = collision.GetComponent<IInteractable>();
        if (interactableObject != null &&
            currentInteractObjectInfo.IsSameIDObject(interactableObject.GetObjectInfo()))
        {
            ExitInteraction(interactableObject.GetObjectInfo());
            interactableObject.ExitInteraction();
        }
    }



    /// <summary>
    /// OnTrigger~���� ���� InteractObjectInfo�� ���� �÷��̾��� ��ȣ�ۿ��� �����Ѵ�.
    /// </summary>
    /// <param name="_info"></param>
    private void EnterInteract(InteractObjectInfo _info)
    {
        currentInteractObjectInfo = _info;
        switch (_info.objectType)
        {
            case FieldObjectType.Type.WorkableSheep:
                fsm.ChangeState(PlayerState.Work);
                break;
            case FieldObjectType.Type.UnWorkableSheep:
                break;
            default:
                break;
        }
    }
    public void StayInteraction(InteractObjectInfo _info)
    {
        if(currentInteractObjectInfo.instanceID == _info.instanceID)
        {
            if (currentInteractObjectInfo.objectType != _info.objectType)
            {
                fsm.ChangeState(PlayerState.Idle);
            }
        }
    }
    public void ExitInteraction(InteractObjectInfo _info)
    {
        currentInteractObjectInfo = _info;
        switch (_info.objectType)
        {
            case FieldObjectType.Type.WorkableSheep:
                fsm.ChangeState(PlayerState.Idle);
                break;
            default:
                break;
        }

        currentInteractObjectInfo.SetEmpty();
    }
    #endregion


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
        // �̵� ���� ���� �� Idle�� ��ȯ�Ѵ�.
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
        _spriteRenderer.color = Color.red;
    }

    private void Work_Execute()
    {
        _rb2D.velocity = movementAmount * controllMoveSpeed;
    }

    private void Work_Exit()
    {
        _spriteRenderer.color = Color.white;
    }


    #endregion

}