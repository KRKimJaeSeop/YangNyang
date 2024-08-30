using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyStorage : BaseStorage
{
    public const string PLAYERPREFS_CURRENCY = "Currency";
    public const int MAX_COLLECTABLE_AMOUNT_FROM_DROP = 500000;

    [System.Serializable]
    public class CurrencyData : ICloneable
    {
        public Currency.Type code;
        public long amount;
        public CurrencyData(Currency.Type code, long amount)
        {
            this.code = code;
            this.amount = amount;
        }
        public object Clone()
        {
            return new CurrencyData(this.code, this.amount);
        }
    }

    [System.Serializable]
    public class StorageData : ICloneable
    {
        //[Header("[StorageData]")]
        [Tooltip("재화 리스트")]
        public List<CurrencyData> currencies = new List<CurrencyData>();

        public object Clone()
        {
            StorageData clone = new StorageData();
            //clone.equippedID = this.equippedID;
            clone.currencies = this.currencies.ConvertAll(item => (item.Clone() as CurrencyData));
            return clone;
        }
       
    }

    private StorageData _data = new StorageData();

    public StorageData Data { get { return _data; } }

    // ---- event
    public delegate void UpdateCurrencyEvent(Currency.Type code, long total, long amount);
    public static event UpdateCurrencyEvent OnUpdateCurrency;
    public delegate void CollectCurrencyEvent(Currency.Type code, long total, long amount);
    public static event CollectCurrencyEvent OnCollectCurrency;


    #region IStorage
    public override void Clear()
    {
        base.Clear();
        PlayerPrefs.DeleteKey(PLAYERPREFS_CURRENCY); // reset용.
        _data = new StorageData();
        // 플레이 중일 때에는 데이터를 삭제했기 때문에 다시 초기값을 세팅해야 하므로 Load() 호출.
        if (Application.isPlaying)
            Load();
    }

    public override bool Load()
    {
        //Debug.LogWarning($"{GetType()}::{nameof(Load)}: called");
        if (IsLoaded == true)
            return false;

        // 데이터가 없다면.
        if (!PlayerPrefs.HasKey(PLAYERPREFS_CURRENCY))
        {
            //Debug.LogWarning($"{GetType()}::{nameof(Load)}: No PlayerPrefs");
            if (Application.isPlaying)
            {
                Overwrite(GameDataManager.Instance.Storages.StartData.Currency);

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
            //Debug.LogWarning($"{GetType()}::{nameof(Load)}: Has PlayerPrefs");
            string strData = PlayerPrefs.GetString(PLAYERPREFS_CURRENCY);
            Overwrite(strData);
        }

        return base.Load();
    }

    public override bool Save()
    {
        if (IsDirty)
        {
            string strData = JsonUtility.ToJson(_data);
            PlayerPrefs.SetString(PLAYERPREFS_CURRENCY, strData);
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


    #region Management
    public CurrencyData GetItem(Currency.Type code)
    {
        return _data.currencies.Find(item => (item.code == code));
    }

    private bool AddItem(Currency.Type code, long amount)
    {
        var item = GetItem(code);
        if (item == null)
        {
            var list = new List<int>();
            _data.currencies.Add(new CurrencyData(code, amount));
            SetDirty();
            OnUpdateCurrency?.Invoke(code, amount, amount);
            return true;

        }
        else
        {
            Debug.LogWarning($"{GetType()}::{nameof(AddItem)}: 이미 존재하는 아이템입니다. code({code})");
            return false;
        }
    }

    private bool RemoveItem(Currency.Type code)
    {
        var item = GetItem(code);
        if (item != null)
        {
            _data.currencies.Remove(item);
            SetDirty();
            return true;
        }

        return false;
    }
    #endregion

    #region Currency
    public long GetAmount(Currency.Type code)
    {
        var data = GetItem(code);
        return (data == null) ? 0 : data.amount;
    }

    /// <summary>
    /// 해당 금액 이상을 가지고 있는가?
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool HasAmount(Currency.Type code, long amount)
    {
        return (amount <= GetAmount(code));
    }


    public long Increase(Currency.Type code, long amount)
    {
        long result = 0;


        var data = GetItem(code);
        if (data == null)
        {
            AddItem(code, amount);
            result = amount;
        }
        else
        {
            if (amount == 0)
                return data.amount;

            result = data.amount;
            try
            {
                result = checked(result + amount);
            }
            catch (OverflowException e)
            {
                result = long.MaxValue;
                Debug.LogWarning($"{GetType()}::{nameof(Increase)} : {e}, amount={amount}");
            }

            data.amount = result;
        }

        SetDirty();
        OnUpdateCurrency?.Invoke(code, result, amount);
        OnCollectCurrency?.Invoke(code, result, amount);
        return result;
    }

    public (bool success, long value) Decrease(Currency.Type code, long amount)
    {
        var data = GetItem(code);
        if (data == null)
        {
            return (false, 0);
        }

        if (data.amount < amount)
            return (false, data.amount);

        data.amount -= amount;
        if (data.amount < 0)
            data.amount = 0;

        SetDirty();
        OnUpdateCurrency?.Invoke(code, data.amount, amount * -1);
        return (true, data.amount);
    }
    #endregion
}
