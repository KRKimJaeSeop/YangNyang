using System;
using UnityEngine;

/// <summary>
/// �ʵ忡 ��ġ�Ǵ� ������ ���� ������Ʈ��.
/// </summary>
 [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BaseFieldObject : MonoBehaviour
{
    protected Transform _transform;
    protected Rigidbody2D _rb2D;
    protected Collider2D _collider2D;
    // ����� �ݹ�.
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
        if (_rb2D != null)
        {
            _rb2D.MovePosition(position);
        }
        else
            _transform.position = position;
    }

 
    /// <summary>
    /// ���ӿ�����Ʈ�� Ȱ��ȭ�ϰ� �ݹ����ִٸ� ��Ȱ��ȭ�Ҷ� �����Ų��.
    /// </summary>
    /// <param name="cbDisable"></param>
    public void EnableGameObject(Action cbDisable = null)
    {
        this.gameObject.SetActive(true);
        this._cbDisable = cbDisable;
    }
    public void DisableGameObject()
    {
        _cbDisable?.Invoke();
        this.gameObject.SetActive(false);
    }

}
