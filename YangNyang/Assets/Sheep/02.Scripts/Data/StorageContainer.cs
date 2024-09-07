using System.Collections.Generic;
using UnityEngine;

public class StorageContainer : MonoBehaviour, IStorage
{
    [System.Flags]
    public enum Type : long // (참고:MonoBehaviour 상속클래스는 long, uint 등에 대한 enum 상속값을 사용하지 못한다.)
    {
        // ---- None
        None = 0,

        Prefereence = 1L << 1,
        User = 1L << 2,
        Currency = 1L << 3,
        UnlockSheep = 1L << 4,

        All = long.MaxValue
    }

    public bool IsLoaded { get; protected set; }

    protected List<BaseStorage> _storages = new List<BaseStorage>();

    protected LongBitFlags _savedTypes = new LongBitFlags();
    [Header("[Settings]")]
    [SerializeField, Tooltip("시작 데이터")]
    private StartData startData;
    [SerializeField, Tooltip("개발용 시작 데이터")]
    private StartData devStartData;
    public StartData StartData { get { return startData; } }
    public StartData DevStartData { get { return devStartData; } }

    //===========
    private PreferenceStorage _preference = new PreferenceStorage();
    private UserStorage _user = new UserStorage();
    private CurrencyStorage _currency = new CurrencyStorage();
    private UnlockSheepStorage _unlockSheep = new UnlockSheepStorage();

    public PreferenceStorage Preference { get { return _preference; } }
    public UserStorage User { get { return _user; } }
    public CurrencyStorage Currency { get { return _currency; } }
    public UnlockSheepStorage UnlockSheep { get { return _unlockSheep; } }




    #region IStorage
    public void Clear()
    {
#if UNITY_EDITOR
        IsLoaded = false;
        foreach (var storage in _storages)
            storage.Clear();
#endif
    }
    public bool Load()
    {
        if (!IsLoaded)
        {
            foreach (var storage in _storages)
                storage.Load();

            IsLoaded = true;
        }
        return IsLoaded;
    }
    public bool Save()
    {
        var savedType = SaveAndGetSaved();
        if (savedType.HasValue((long)Type.None))
            return false;

        return true;
    }
    public void Overwrite(string strJson)
    {
    }
    public string ToJson()
    {
        return null;
    }
    #endregion
    /// <summary>
    /// 저장소 리스트(_storages) 세팅.
    /// </summary>
    public void RegisterStorages()
    {
        _storages.Clear();
        RegisterStorage(_preference);
        RegisterStorage(_user);
        RegisterStorage(_currency);
        RegisterStorage(_unlockSheep);
    }

    public virtual void RegisterStorage(BaseStorage storage)
    {
        _storages.Add(storage);
    }

    /// <summary>
    /// 저장소 로딩/초기화.
    /// </summary>
    /// <returns></returns>
    public virtual bool Initialize()
    {
        // 저장소 등록
        RegisterStorages();

        Load();

        // 저장소 데이터 초기화
        foreach (var storage in _storages)
            storage.Initialize();

        return true;
    }

    /// <summary>
    /// 저장소 데이터 중에 저장 가능한 데이터가 있는지 여부 반환.
    /// </summary>
    /// <returns></returns>
    public bool IsStorageable()
    {
        return (_preference.IsDirty
            || _user.IsDirty
            || _currency.IsDirty
            || _unlockSheep.IsDirty);
    }

    /// <summary>
    /// 저장 후, 저장 타입 반환.
    /// 방문용 저장소는 저장하지 않는다.
    /// </summary>
    /// <returns></returns>
    public LongBitFlags SaveAndGetSaved()
    {
        _savedTypes.Clear();
        if (IsLoaded == false)
            return _savedTypes;

        if (_preference.Save())
            _savedTypes.Add((long)Type.Prefereence);
        if (_user.Save())
            _savedTypes.Add((long)Type.User);
        if (_currency.Save())
            _savedTypes.Add((long)Type.Currency);
        if (_unlockSheep.Save())
            _savedTypes.Add((long)Type.UnlockSheep);

        return _savedTypes;
    }


}