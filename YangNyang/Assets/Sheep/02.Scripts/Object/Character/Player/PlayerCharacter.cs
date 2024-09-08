using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public class PlayerCharacter : CharacterObject
{
    public enum PlayerState
    {
        None,
        Idle,
        Move,
        Work,
    }

    private StateMachine<PlayerState> _fsm;
    [SerializeField, Tooltip("조이스틱 조작시 이동속도")]
    private float _controllMoveSpeed = 10;
    private Vector2 _movementAmount;
    private Vector3 _originScale;
    private Vector3 _flipScale;
    [SerializeField, Tooltip("상호작용중인 IInteractable 게임오브젝트의 정보")]
    private InteractObjectInfo currentInteractObjectInfo;


    protected override void Awake()
    {
        base.Awake();
        _fsm = new StateMachine<PlayerState>();
        _fsm.Initialize(this);
        InitializeStates();
        _fsm.SetInitState(PlayerState.Idle);

        _originScale = _transform.localScale;
        _flipScale = new Vector3(-(_transform.localScale.x), _transform.localScale.y, _transform.localScale.z);
        //나중에 활성화
        // Addressables.LoadAssetAsync<Sprite>("고양이테스트").Completed += OnSpriteLoaded;

    }
    private void OnSpriteLoaded(AsyncOperationHandle<Sprite> handle)
    {
        //_spriteRenderer.sprite = handle.Result;
    }

    protected override void InitializeStates()
    {
        _fsm.AddState(PlayerState.Idle, Idle_Enter, Idle_Execute, Idle_Exit);
        _fsm.AddState(PlayerState.Move, Move_Enter, Move_Execute, Move_Exit);
        _fsm.AddState(PlayerState.Work, Work_Enter, Work_Execute, Work_Exit);
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
        this._movementAmount = movementAmount;
    }

    private void Update()
    {
        _fsm.Update();
    }

    #region 충돌 처리 메소드.

    /// <summary>
    /// 플레이어가 트리거 콜라이더에 들어가거나 머무르거나 나갈 때
    /// IInteractable 인터페이스를 구현한 객체와의 상호작용만을 처리한다.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {

        IInteractable interactableObject = collision.GetComponent<IInteractable>();
        if (interactableObject != null)
        {
            if (currentInteractObjectInfo.IsEmpty())
            {
                EnterSingleInteract(interactableObject.GetObjectInfo());
                interactableObject.EnterSingleInteraction();
            }
            else
            {
                interactableObject.EnterMultipleInteraction();
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IInteractable interactableObject = collision.GetComponent<IInteractable>();
        if (interactableObject != null &&
             currentInteractObjectInfo.IsSameIDObject(interactableObject.GetObjectInfo()))
        {
            StaySingleInteraction(interactableObject.GetObjectInfo());
            interactableObject.StaySingleInteraction();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactableObject = collision.GetComponent<IInteractable>();
        if (interactableObject != null &&
            currentInteractObjectInfo.IsSameIDObject(interactableObject.GetObjectInfo()))
        {
            ExitSingleInteraction(interactableObject.GetObjectInfo());
            interactableObject.ExitSingleInteraction();
        }
    }



    /// <summary>
    /// OnTrigger~에서 받은 InteractObjectInfo에 따라 플레이어의 상호작용을 결정한다.
    /// </summary>
    /// <param name="_info"></param>
    private void EnterSingleInteract(InteractObjectInfo _info)
    {
        currentInteractObjectInfo = _info;
        switch (_info.objectType)
        {
            case FieldObject.Type.WorkableSheep:
                _fsm.ChangeState(PlayerState.Work);
                break;
            case FieldObject.Type.UnWorkableSheep:
                break;
            default:
                break;
        }
    }
    public void StaySingleInteraction(InteractObjectInfo _info)
    {
        if (currentInteractObjectInfo.instanceID == _info.instanceID)
        {
            if (currentInteractObjectInfo.objectType != _info.objectType)
            {
                _fsm.ChangeState(PlayerState.Idle);
            }
        }
    }
    public void ExitSingleInteraction(InteractObjectInfo _info)
    {
        currentInteractObjectInfo = _info;
        switch (_info.objectType)
        {
            case FieldObject.Type.WorkableSheep:
                _fsm.ChangeState(PlayerState.Idle);
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
        base.SetAnim_Move(false);
        //Debug.Log("Entering Idle State");    
        //GetComponent<Animator>().SetTrigger("Idle");
    }

    private void Idle_Execute()
    {
        //Debug.Log("Executing Idle State");
        if (_movementAmount != Vector2.zero)
        {
            _fsm.ChangeState(PlayerState.Move);
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
        base.SetAnim_Move(true);
        //Debug.Log("Entering Move State");
        //GetComponent<Animator>().SetTrigger("Move");
    }

    private void Move_Execute()
    {
        // 이동 조작 없을 시 Idle로 전환한다.
        if (_movementAmount == Vector2.zero)
        {
            _rb2D.velocity = Vector2.zero;
            _fsm.ChangeState(PlayerState.Idle);
            return;
        }
        if (_movementAmount.x < 0)
        {
            _transform.localScale = _flipScale;
            // 이대로 애니메이션을 이미 만들어버려서..어쩔수없이 플립할때 말풍선도 같이 플립해주자.
            _speechBubble.Flip(true);
        }
        if (_movementAmount.x > 0)
        {
            _transform.localScale = _originScale;
            _speechBubble.Flip(false);
        }

        _rb2D.velocity = _movementAmount * _controllMoveSpeed;
    }

    private void Move_Exit()
    {
        //Debug.Log("Exiting Move State");
    }
    #endregion

    #region State.Work
    private void Work_Enter()
    {
        base.SetAnim_Work(true);
        //_spriteRenderer.color = Color.red;
    }

    private void Work_Execute()
    {
        _rb2D.velocity = _movementAmount * _controllMoveSpeed;
    }

    private void Work_Exit()
    {
        base.SetAnim_Work(false);
        //_spriteRenderer.color = Color.white;
    }


    #endregion

}
