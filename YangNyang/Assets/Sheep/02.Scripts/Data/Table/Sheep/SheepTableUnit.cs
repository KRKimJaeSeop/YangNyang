using UnityEngine;

public class SheepTableUnit : BaseElementTable
{
    public const string ASSET_PATH = "Assets/Sheep/99.Data/Tables/Sheeps/";

    [Header("[Sheep]")]
    [SerializeField]
    private Sheep.Type _type;
    [SerializeField]
    private float _idleStateRate = 0.5f;
    [SerializeField]
    private float _idleTime = 1f;
    [SerializeField]
    private float _moveSpeed = 0.5f;
    [SerializeField]
    private int _minWoolAmount = 1;
    [SerializeField]
    private int _maxWoolAmount = 3;

    public Sheep.Type Type { get { return _type; } set { _type = value; } }
    public float IdleStateRate { get { return _idleStateRate; } }
    public float IdleTime { get { return _idleTime; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public int MinWoolAmount { get { return _minWoolAmount; } }
    public int MaxWoolAmount { get { return _maxWoolAmount; } }
}
