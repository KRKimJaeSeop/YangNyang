using System.Collections.Generic;
using UnityEngine;

public class StorageContainer : MonoBehaviour, IStorage
{
    [System.Flags]
    public enum Type : long // (����:MonoBehaviour ���Ŭ������ long, uint � ���� enum ��Ӱ��� ������� ���Ѵ�.)
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
    [SerializeField, Tooltip("���� ������")]
    private StartData startData;
    [SerializeField, Tooltip("���߿� ���� ������")]
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
    /// ����� ����Ʈ(_storages) ����.
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
    /// ����� �ε�/�ʱ�ȭ.
    /// </summary>
    /// <returns></returns>
    public virtual bool Initialize()
    {
        // ����� ���
        RegisterStorages();

        Load();

        // ����� ������ �ʱ�ȭ
        foreach (var storage in _storages)
            storage.Initialize();

        return true;
    }

    /// <summary>
    /// ����� ������ �߿� ���� ������ �����Ͱ� �ִ��� ���� ��ȯ.
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
    /// ���� ��, ���� Ÿ�� ��ȯ.
    /// �湮�� ����Ҵ� �������� �ʴ´�.
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