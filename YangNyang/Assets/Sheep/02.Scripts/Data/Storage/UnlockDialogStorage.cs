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
        [Tooltip("�ر� �� ����Ʈ")]
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
        PlayerPrefs.DeleteKey(PLAYERPREFS_UNLOCK_DIALOG); // reset��.
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
        if (!PlayerPrefs.HasKey(PLAYERPREFS_UNLOCK_DIALOG))
        {
            if (Application.isPlaying)
            {
                Overwrite(GameDataManager.Instance.Storages.StartData.UnlockDialog);
            }
        }
        // �����Ͱ� �ִٸ�.
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
    /// �ش� id�� ���� �رݿ��θ� ��ȯ�Ѵ�.
    /// </summary>
    public bool IsUnlockDialogID(Dialog.Type type)
    {
        return _data.unlockDialogs.Contains(type);
    }

    /// <summary>
    /// �ش� id�� ���� �ر��Ѵ�.
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