using Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDialogStorage : BaseStorage
{
    private const string PLAYERPREFS_UNLOCK_DIALOG = "UnlockDialog";

    [System.Serializable]
    public class StorageData : ICloneable
    {
        [Tooltip("해금 양 리스트")]
        public List<Dialog.Type> unlockDialogs = new List<Dialog.Type>();

        public object Clone()
        {
            StorageData clone = new StorageData();
            clone.unlockDialogs = new List<Dialog.Type>(this.unlockDialogs);
            return clone;
        }
    }

    private StorageData _data = new StorageData();

    public StorageData Data { get { return _data; } }

    // ---- event
    public delegate void UnlockDialogEvent(Dialog.Type type);
    public static event UnlockDialogEvent OnUnlockDialog;

    #region IStorage
    public override void Clear()
    {
        base.Clear();
        PlayerPrefs.DeleteKey(PLAYERPREFS_UNLOCK_DIALOG); // reset용.
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
        if (!PlayerPrefs.HasKey(PLAYERPREFS_UNLOCK_DIALOG))
        {
            if (Application.isPlaying)
            {
                Overwrite(GameDataManager.Instance.Storages.StartData.UnlockDialog);
            }
        }
        // 데이터가 있다면.
        else
        {
            string strData = PlayerPrefs.GetString(PLAYERPREFS_UNLOCK_DIALOG);
            Overwrite(strData);
        }

        return base.Load();
    }

    public override bool Save()
    {
        if (IsDirty)
        {
            string strData = JsonUtility.ToJson(_data);
            PlayerPrefs.SetString(PLAYERPREFS_UNLOCK_DIALOG, strData);
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

    public bool IsUnlockAllDialog()
    {
        var dialogs = GameDataManager.Instance.Tables.Dialog.GetList();
        foreach (var dialog in dialogs)
        {
            if (!_data.unlockDialogs.Contains(dialog.type))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 해당 id의 양의 해금여부를 반환한다.
    /// </summary>
    public bool IsUnlockDialogID(Dialog.Type type)
    {
        return _data.unlockDialogs.Contains(type);
    }

    /// <summary>
    /// 해당 id의 양을 해금한다.
    /// </summary>
    public void UnlockDialog(Dialog.Type type)
    {
        if (IsUnlockDialogID(type))
            return;
        _data.unlockDialogs.Add(type);
        OnUnlockDialog?.Invoke(type);
        SetDirty();
        Save();
    }


}
