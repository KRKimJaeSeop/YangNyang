using System;
using UnityEngine;

public class UserStorage : BaseStorage
{
    private const string PLAYERPREFS_USER = "User";

    [System.Serializable]
    public class StorageData : ICloneable
    {
        public int day;
        public int researchLevel;
        public long researchExp;

        public object Clone()
        {
            StorageData clone = new StorageData();

            clone.day = this.day;
            clone.researchLevel = this.researchLevel;
            clone.researchExp = this.researchExp;

            return clone;
        }

    }

    protected StorageData _data = new StorageData();

    public StorageData Data { get { return _data; } }
    public int Day { get { return _data.day; } }
    public int ResearchLevel { get { return _data.researchLevel; } }
    public long ResearchExp { get { return _data.researchExp; } }

    // ---- event
    public delegate void UpdateDayEvent(int day);
    public static event UpdateDayEvent OnUpdateDay; // 날짜(회차) 업데이트 이벤트 
    public delegate void UpdateLevelEvent(int level);
    public static event UpdateLevelEvent OnUpdateLevel; // 레벨 업데이트 이벤트 
    public delegate void UpdateExpEvent(long exp, long amount);
    public static event UpdateExpEvent OnUpdateExp; // 경험치 업데이트 이벤트 


    private long _cachedMaxExp;


    #region IStorage
    public override void Clear()
    {
        base.Clear();
        PlayerPrefs.DeleteKey(PLAYERPREFS_USER); // reset용.
        _data = new StorageData();
        // 플레이 중일 때에는 데이터를 삭제했기 때문에 다시 초기값을 세팅해야 하므로 Load() 호출.
        if (Application.isPlaying)
            Load();
    }

    public override bool Load()
    {
        if (IsLoaded == true)
            return false;

        // 데이터가 없다면.
        if (!PlayerPrefs.HasKey(PLAYERPREFS_USER))
        {
            //Debug.LogWarning($"{GetType()}::{nameof(Load)}: No PlayerPrefs");
            if (Application.isPlaying)
            {
                Overwrite(GameDataManager.Instance.Storages.StartData.User);

                //if (!GameDataManager.Instance.Settings.System.SampleBuild)
                //{
                //    // to do somethig on release build
                //    Overwrite(GameDataManager.Instance.Storages.StartupData.Currency);
                //}
                //else
                //{
                //    // to do somethig on sample build
                //    Overwrite(AppDataManager.Instance.Storages.StartupDataForSampleBuild.Currency);
                //}
            }
        }
        // 데이터가 있다면.
        else
        {
            string strData = PlayerPrefs.GetString(PLAYERPREFS_USER);
            Overwrite(strData);
        }

        return base.Load();
    }

    public override bool Save()
    {
        if (IsDirty)
        {
            string strData = JsonUtility.ToJson(_data);
            PlayerPrefs.SetString(PLAYERPREFS_USER, strData);
            //Debug.Log($"{GetType()}::{nameof(Save)} - {strData}");
            Debug.Log($"{GetType()}::{nameof(Save)}");
        }
        return base.Save();
    }

    public void Overwrite(StorageData data)
    {
        _data = data.Clone() as StorageData;
        SetDirty();
        //Save();
    }

    public override void Overwrite(string strJson)
    {
        //Debug.Log($"{GetType()}::{nameof(Overwrite)} - {strJson}");
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
        _cachedMaxExp = GameDataManager.Instance.Tables.Research.GetMaxExp(_data.researchLevel);
    }


    #region Day
    public long IncreaseDay(int amount)
    {
        _data.day += amount;
        SetDirty();
        OnUpdateDay?.Invoke(_data.day);
        return _data.day;
    }
    #endregion Day


    #region ResearchLevel
    public long IncreaseLevel(int amount)
    {
        if (amount == 0)
            return _data.researchLevel;

        _data.researchLevel += amount;
        SetDirty();
        _cachedMaxExp = GameDataManager.Instance.Tables.Research.GetMaxExp(_data.researchLevel);
        OnUpdateLevel?.Invoke(_data.researchLevel);
        return _data.researchLevel;
    }
    #endregion Level

    #region ResearchExp
    public long IncreaseExp(long amount)
    {
        if (amount == 0)
            return _data.researchExp;

        var result = _data.researchExp + amount;
        if (result < _cachedMaxExp)
        {
            UpdateExp(amount);
        }
        else
        {
            IncreaseLevel(1);
            UpdateExp(-_data.researchExp);
        }
        return _data.researchExp;
    }

    private void UpdateExp(long amount)
    {
        _data.researchExp += amount;
        SetDirty();
        OnUpdateExp?.Invoke(_data.researchExp, amount);
    }
    #endregion



}
