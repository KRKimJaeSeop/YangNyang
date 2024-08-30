using System;
using UnityEngine;

public class UserStorage : BaseStorage
{
    private const string PLAYERPREFS_USER = "User";

    [System.Serializable]
    public class StorageData : ICloneable
    {
        public int day;
        public long researchLevel;
        public ulong researchExp;

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
    public long ResearchLevel { get { return _data.researchLevel; } }
    public ulong ResearchExp { get { return _data.researchExp; } }

    // ---- event
    public delegate void UpdateDayEvent();
    public static event UpdateDayEvent OnUpdateDay; // 날짜(회차) 업데이트 이벤트 
    public delegate void UpdateLevelEvent();
    public static event UpdateLevelEvent OnUpdateLevel; // 레벨 업데이트 이벤트 
    public delegate void UpdateExpEvent(ulong exp, ulong amount);
    public static event UpdateExpEvent OnUpdateExp; // 경험치 업데이트 이벤트 



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

    }


    #region Day
    public long IncreaseDay()
    {
        _data.day++;
        SetDirty();
        OnUpdateDay?.Invoke();
        return _data.day;
    }
    #endregion Day


    #region ResearchLevel
    public long IncreaseLevel()
    {
        _data.researchLevel++;
        SetDirty();
        OnUpdateLevel?.Invoke();
        return _data.researchLevel;
    }
    #endregion Level

    #region ResearchExp
    public ulong IncreaseExp(ulong amount)
    {
        if (amount == 0)
            return _data.researchExp;

        ulong result = _data.researchExp;
        try
        {
            result = checked(result + amount);
        }
        catch (OverflowException e)
        {
            result = long.MaxValue;
            Debug.LogWarning($"{GetType()}::{nameof(IncreaseExp)} : {e}, amount={amount}");
        }

        _data.researchExp = result;
        SetDirty();
        OnUpdateExp?.Invoke(result, amount);
        return result;
    }
    //public (bool success, ulong value) DecreaseExp(ulong amount)
    //{
    //    if (_data.researchExp < amount)
    //        return (false, _data.researchExp);

    //    _data.researchExp -= amount;
    //    if (_data.researchExp < 0)
    //        _data.researchExp = 0;

    //    SetDirty();
    //    OnUpdateExp?.Invoke(_data.researchExp, amount * -1);

    //    return (true, _data.researchExp);
    //}
    #endregion



}
