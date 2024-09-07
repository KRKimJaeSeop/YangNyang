using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockSheepStorage : BaseStorage
{
    private const string PLAYERPREFS_UNLOCK_SHEEP = "UnlockSheep";

    [System.Serializable]
    public class StorageData : ICloneable
    {
        [Tooltip("�ر� �� ����Ʈ")]
        public List<int> unlockSheeps = new List<int>();

        public object Clone()
        {
            StorageData clone = new StorageData();
            clone.unlockSheeps = new List<int>(this.unlockSheeps);
            return clone;
        }
    }

    private StorageData _data = new StorageData();

    public StorageData Data { get { return _data; } }

    // ---- event
    public delegate void UnlockSheepEvent(int id);
    public static event UnlockSheepEvent OnUnlockSheep;

    #region IStorage
    public override void Clear()
    {
        base.Clear();
        PlayerPrefs.DeleteKey(PLAYERPREFS_UNLOCK_SHEEP); // reset��.
        _data = new StorageData();
        // �÷��� ���� ������ �����͸� �����߱� ������ �ٽ� �ʱⰪ�� �����ؾ� �ϹǷ� Load() ȣ��.
        if (Application.isPlaying)
            Load();
    }

    public override bool Load()
    {
        if (IsLoaded == true)
            return false;

        // �����Ͱ� ���ٸ�.
        if (!PlayerPrefs.HasKey(PLAYERPREFS_UNLOCK_SHEEP))
        {
            if (Application.isPlaying)
            {
                Overwrite(GameDataManager.Instance.Storages.StartData.UnlockSheep);
            }
        }
        // �����Ͱ� �ִٸ�.
        else
        {
            string strData = PlayerPrefs.GetString(PLAYERPREFS_UNLOCK_SHEEP);
            Overwrite(strData);
        }

        return base.Load();
    }

    public override bool Save()
    {
        if (IsDirty)
        {
            string strData = JsonUtility.ToJson(_data);
            PlayerPrefs.SetString(PLAYERPREFS_UNLOCK_SHEEP, strData);
            Debug.Log($"{GetType()}::{nameof(Save)}");
        }
        return base.Save();
    }

    public void Overwrite(StorageData data)
    {
        _data = data.Clone() as StorageData;
        SetDirty();
    }

    public override void Overwrite(string strJson)
    {
        if (string.IsNullOrEmpty(strJson))
            return;

        _data = JsonUtility.FromJson<StorageData>(strJson);
    }

    public override string ToJson()
    {
        return JsonUtility.ToJson(_data);
    }
    #endregion // IStorage

    public override void Initialize()
    {

    }
    
    public bool IsUnlockAllSheep()
    {
        var sheeps = GameDataManager.Instance.Tables.Sheep.GetList();
        foreach (var sheep in sheeps)
        {
            if (!_data.unlockSheeps.Contains(sheep.id))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// �ش� id�� ���� �رݿ��θ� ��ȯ�Ѵ�.
    /// </summary>
    public bool IsUnlockSheepID(int id)
    {
        return _data.unlockSheeps.Contains(id);
    }

    /// <summary>
    /// �ش� id�� ���� �ر��Ѵ�.
    /// </summary>
    public void UnlockSheep(int id)
    {
        if (IsUnlockSheepID(id))
            return;
        _data.unlockSheeps.Add(id);
        OnUnlockSheep?.Invoke(id);
        SetDirty();
        Save();
    }

}
