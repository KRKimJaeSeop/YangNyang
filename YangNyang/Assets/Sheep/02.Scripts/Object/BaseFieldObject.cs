using MoreMountains.Feedbacks;
using System;
using UnityEngine;

/// <summary>
/// 필드에 배치되는 물리를 가진 오브젝트다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BaseFieldObject : MonoBehaviour
{
 
    protected Transform _transform;
    protected Rigidbody2D _rb2D;
    protected Collider2D _collider2D;
    protected Action _cbDisable;

    [SerializeField]
    private int instanceID;
    public int InstanceID { get; private set; }


    protected virtual void Awake()
    {
        _transform = this.transform;
        _rb2D = this.GetComponent<Rigidbody2D>();
        _collider2D = this.GetComponent<Collider2D>();
        InstanceID = this.gameObject.GetInstanceID();
        instanceID = InstanceID;
    }

    protected void SetPosition(Vector2 position)
    {
        if (_rb2D != null)
        {
            if (gameObject.activeSelf == false)
            {
                _transform.position = position;
            }
            else
            {
                _rb2D.MovePosition(position);
            }
        }
        else
        {
            _transform.position = position;
        }
    }


    /// <summary>
    /// 게임오브젝트를 활성화하고 콜백이있다면 비활성화할때 실행시킨다.
    /// </summary>
    /// <param name="cbDisable"></param>
    public virtual void Spawn(Vector2 spawnPosition, Action cbDisable = null)
    {
        SetPosition(spawnPosition);
        this.gameObject.SetActive(true);
        this._cbDisable = cbDisable;
    }
    public virtual void Despawn()
    {
            _cbDisable?.Invoke();
            this.gameObject.SetActive(false);

    }

}
