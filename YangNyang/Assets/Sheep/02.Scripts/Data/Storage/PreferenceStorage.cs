using System;
using UnityEngine;

public class PreferenceStorage : BaseStorage
{
    public const string PLAYERPREFS_PREFERENCES = "Preferences";

    [Serializable]
    public class StorageData : ICloneable
    {
        [Tooltip("언어 코드. 처음 설치시 sting.Empty")]
        public string languageCode = string.Empty;

        [Tooltip("배경음 소리크기")]
        public float bgmVolume = 0.5f; // 0.001 ~ 1f
        [Tooltip("효과음 소리크기")]
        public float sfxVolume = 0.5f; // 0.001 ~ 1f

        public object Clone()
        {
            StorageData clone = new StorageData();
            clone.languageCode = string.Copy(this.languageCode);
            clone.bgmVolume = this.bgmVolume;
            clone.sfxVolume = this.sfxVolume;
            return clone;
        }
    }


    private StorageData _data = new StorageData();
    public StorageData Data { get { return _data; } }
    public string LanguageCode { get { return _data.languageCode; } }

    // ---- event
    public delegate void SavePreferencesDataEvent(StorageData data); // 환경 설정이 세팅 되어 저장될 때 호출 된다.
    public static event SavePreferencesDataEvent OnSavedPreferencesData;
    public delegate void UpdateLanguageEvent(string languageCode);
    public static event UpdateLanguageEvent OnUpdateLanguage;


    #region IStorage
    public override void Clear()
    {
        base.Clear();
        PlayerPrefs.DeleteKey(PLAYERPREFS_PREFERENCES); // reset용.
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
        if (!PlayerPrefs.HasKey(PLAYERPREFS_PREFERENCES))
        {
            SetDirty();
            Save();
        }
        // 데이터가 있다면.
        else
        {
            string strData = PlayerPrefs.GetString(PLAYERPREFS_PREFERENCES);
            Overwrite(strData);
        }

        return base.Load();
    }

    /// <summary>
    /// 데이터 세이브.
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
    public bool IsMuted(AudioManager.MixerGroup mixerGroup)
    {
        return (GetVolume(mixerGroup) <= 0.001f);
    }

    public float GetVolume(AudioManager.MixerGroup mixerGroup)
    {
        if (mixerGroup == AudioManager.MixerGroup.BGM)
        {
            return _data.bgmVolume;
        }
        else if (mixerGroup == AudioManager.MixerGroup.SFXMaster)
        {
            return _data.sfxVolume;
        }
        return 0f;
    }

    public void SetVolume(AudioManager.MixerGroup mixerGroup, float volume)
    {
        if (mixerGroup == AudioManager.MixerGroup.BGM)
        {
            _data.bgmVolume = volume;
        }
        else if (mixerGroup == AudioManager.MixerGroup.SFXMaster)
        {
            _data.sfxVolume = volume;
        }
        SetDirty();
    }
    #endregion
}