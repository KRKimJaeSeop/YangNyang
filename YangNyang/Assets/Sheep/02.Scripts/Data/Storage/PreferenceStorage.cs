using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreferenceStorage : BaseStorage
{
    public const string PLAYERPREFS_PREFERENCES = "Preferences";

    [Serializable]
    public class StorageData : ICloneable
    {
        [Tooltip("��� �ڵ�. ó�� ��ġ�� sting.Empty")]
        public string languageCode = string.Empty;

        [Tooltip("��ü �Ҹ�ũ��")]
        public float masterVolume = 0.5f; // 0.001 ~ 1f
        [Tooltip("����� �Ҹ�ũ��")]
        public float bgmVolume = 0.5f; // 0.001 ~ 1f
        [Tooltip("ȿ���� �Ҹ�ũ��")]
        public float sfxVolume = 0.5f; // 0.001 ~ 1f

        public object Clone()
        {
            StorageData clone = new StorageData();
            clone.languageCode = string.Copy(this.languageCode);
            clone.masterVolume = this.masterVolume;
            clone.bgmVolume = this.bgmVolume;
            clone.sfxVolume = this.sfxVolume;
            return clone;
        }
    }


    private StorageData _data = new StorageData();
    public StorageData Data { get { return _data; } }
    public string LanguageCode { get { return _data.languageCode; } }

    // ---- event
    public delegate void SavePreferencesDataEvent(StorageData data); // ȯ�� ������ ���� �Ǿ� ����� �� ȣ�� �ȴ�.
    public static event SavePreferencesDataEvent OnSavedPreferencesData;
    public delegate void UpdateLanguageEvent(string languageCode);
    public static event UpdateLanguageEvent OnUpdateLanguage;


    #region IStorage
    public override void Clear()
    {
        base.Clear();
        PlayerPrefs.DeleteKey(PLAYERPREFS_PREFERENCES); // reset��.
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
        if (!PlayerPrefs.HasKey(PLAYERPREFS_PREFERENCES))
        {
            SetDirty();
            Save();
        }
        // �����Ͱ� �ִٸ�.
        else
        {
            string strData = PlayerPrefs.GetString(PLAYERPREFS_PREFERENCES);
            Overwrite(strData);
        }

        return base.Load();
    }

    /// <summary>
    /// ������ ���̺�.
    /// </summary>
    public override bool Save()
    {
        if (IsDirty)
        {
            string strData = JsonUtility.ToJson(_data);
            PlayerPrefs.SetString(PLAYERPREFS_PREFERENCES, strData);
            //Debug.Log($"{GetType()}::{nameof(Save)} - {strData}");
            Debug.Log($"{GetType()}::{nameof(Save)}");
            OnSavedPreferencesData?.Invoke(_data);
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


    #region Language
    public string GetLanguageCode()
    {
        return _data.languageCode;
    }
    public void SetLanguageCode(string languageCode)
    {
        if (languageCode != _data.languageCode)
        {
            _data.languageCode = string.Copy(languageCode);
            SetDirty();
            OnUpdateLanguage?.Invoke(languageCode);
        }
    }
    #endregion

   
    #region Volume
    //public bool IsMuted(SoundController.MixerGroup mixerGroup)
    //{
    //    return (GetVolume(mixerGroup) <= 0.001f);
    //}

    //public float GetVolume(SoundController.MixerGroup mixerGroup)
    //{
    //    if (mixerGroup == SoundController.MixerGroup.Master)
    //    {
    //        return _data.masterVolume;
    //    }
    //    else if (mixerGroup == SoundController.MixerGroup.BGM)
    //    {
    //        return _data.bgmVolume;
    //    }
    //    else if (mixerGroup == SoundController.MixerGroup.SFXMaster)
    //    {
    //        return _data.sfxVolume;
    //    }
    //    return 0f;
    //}

    //public void SetVolume(SoundController.MixerGroup mixerGroup, float volume)
    //{
    //    if (mixerGroup == SoundController.MixerGroup.Master)
    //    {
    //        _data.masterVolume = volume;
    //    }
    //    else if (mixerGroup == SoundController.MixerGroup.BGM)
    //    {
    //        _data.bgmVolume = volume;
    //    }
    //    else if (mixerGroup == SoundController.MixerGroup.SFXMaster)
    //    {
    //        _data.sfxVolume = volume;
    //    }
    //    SetDirty();
    //}
    #endregion
}