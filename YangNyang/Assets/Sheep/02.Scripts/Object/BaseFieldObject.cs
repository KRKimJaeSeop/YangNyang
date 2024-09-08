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
    // 종료시 콜백.
    protected Action _cbDisable;

    //[SerializeField]
    //protected SpriteRenderer[] _spriteRenderers;

    public int InstanceID { get; private set; }


    protected virtual void Awake()
    {
        gameObject.SetActive(true);
        _transform = this.transform;
        _rb2D = this.GetComponent<Rigidbody2D>();
        _collider2D = this.GetComponent<Collider2D>();

        InstanceID = this.gameObject.GetInstanceID();
    }

    protected void SetPosition(Vector2 position)
    {
        // 이게 더 빠르다고 알고있는데, 오히려 더 지연이 있어서 일단 주석처리하고 아래로 통일한다.
        //if (_rb2D != null)
        //{
        //    _rb2D.position = position;
        //}
        //else
            _transform.position = position;
    }


    /// <summary>
    /// 게임오브젝트를 활성화하고 콜백이있다면 비활성화할때 실행시킨다.
    /// </summary>
    /// <param name="cbDisable"></param>
    public virtual void Spawn(Vector2 spawnPosition, Action cbDisable = null)
    {
        this.gameObject.SetActive(true);
        SetPosition(spawnPosition);
        this._cbDisable = cbDisable;
    }
    public virtual void Despawn()
    {
        _cbDisable?.Invoke();
        this.gameObject.SetActive(false);
    }

}
