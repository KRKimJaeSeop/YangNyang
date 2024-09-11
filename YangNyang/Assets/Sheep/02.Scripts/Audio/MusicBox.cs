using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static AddressableManager;

/// <summary>
/// 개별효과음 (양,고양이 울음) 등이 아닌 공용 음원을 관리한다.
/// </summary>
public class MusicBox : MonoBehaviour
{
    public enum SfxType
    {
        None = 0,
        DefaultClick, // 기본 클릭음
        OpenPositionableMenu, // 설비 이동 메뉴
    }

    [Serializable]
    private class SfxData
    {
        [Tooltip("타입")]
        public SfxType type;
        [Tooltip("버튼 클릭음")]
        public AudioClip audioClip;
    }

    [Header("[AudioSources]")]
    [SerializeField]
    private AudioSource bgmAudioSource;
    [SerializeField, Tooltip("SFX전용 AudioSource가 있는 오브젝트")]
    private GameObject goSfx;
    [Header("[AudioClips]")]
    [SerializeField] private List<SfxData> sfxs;

    [Header("[Debug]")]
    [SerializeField, ReadOnly]
    private List<AudioSource> _sfxAudioSources;
    private Coroutine _playBGMCoroutine;
    private AsyncOperationHandle<AudioClip> _bgmAssetHandle;
    private bool _isLoadedBGMAssetReference;
    private int _sfxAudioIndex = -1;
    [SerializeField]
    private List<int> _skipIndexs = new List<int>();

    private void Awake()
    {
        _sfxAudioSources.AddRange(goSfx.GetComponents<AudioSource>());
    }

    #region Addressables
    protected void OnDestroy()
    {
        StopBGM();
    }

    private void ReleaseBGMAsset()
    {
        if (_isLoadedBGMAssetReference)
        {
            Addressables.Release(_bgmAssetHandle);
        }
        _isLoadedBGMAssetReference = false;
    }
    #endregion

    #region BGM
    public void PlayBGM(RemoteAssetCode code)
    {
        StopBGM();

        var clip = AddressableManager.Instance.GetAsset<AudioClip>(code);
        if (clip == null)
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlayBGM)}: BGM 에셋을 찾을 수 없음. assetName: {$"{code}"}");
            return;
        }

        _playBGMCoroutine = StartCoroutine(PlayBGMCoroutine(bgmAudioSource, clip, () =>
        {
            // 정상적으로 음악이 종료되었다면 다음곡 플레이.
            PlayBGM(code);
        }));
    }

    public void StopBGM()
    {
        if (_playBGMCoroutine != null)
            StopCoroutine(_playBGMCoroutine);

        _playBGMCoroutine = null;
        StopSound(bgmAudioSource);
        ReleaseBGMAsset();
    }

    IEnumerator PlayBGMCoroutine(AudioSource audioSource, AudioClip clip, Action cbEnd)
    {
        PlaySound(audioSource, clip, false);
        Debug.Log($"{GetType()}::{nameof(PlayBGMCoroutine)}: AudioClip({clip.name}) 로딩 성공, length={clip.length}");

        // 곡이 끝날때까지 대기.
        yield return new WaitForSeconds(clip.length);

        cbEnd?.Invoke();
    }
    #endregion // BGM

    #region SFX
    public void PlaySFX(AudioClip clip)
    {
        if (_sfxAudioSources.Count <= 0)
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlaySFX)}: 세팅되어 있는 SFX AudioSource 가 없음.");
            return;
        }

        var audioSource = _sfxAudioSources[GetUsableIndex()];
        PlaySound(audioSource, clip, false);
    }

    public void PlaySFX(AudioClip clip, float playtime)
    {
        if (_sfxAudioSources.Count <= 0)
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlaySFX)}: 세팅되어 있는 SFX AudioSource 가 없음.");
            return;
        }

        var currentIndex = GetUsableIndex();
        _skipIndexs.Add(currentIndex);

        var audioSource = _sfxAudioSources[currentIndex];
        PlaySound(audioSource, clip, false);
        StartCoroutine(WaitForSeconds(playtime, () =>
        {
            StopSound(audioSource);
            _skipIndexs.Remove(currentIndex);
        }));
    }

    private int GetUsableIndex()
    {
        if (_sfxAudioSources.Count == _skipIndexs.Count)
        {
            var newAudioListner = goSfx.AddComponent<AudioSource>();
            _sfxAudioSources.Add(newAudioListner);
        }

        do
        {
            _sfxAudioIndex++;
            if (_sfxAudioIndex >= _sfxAudioSources.Count)
                _sfxAudioIndex = 0;
        }
        while (_skipIndexs.Contains(_sfxAudioIndex));
        return _sfxAudioIndex;
    }

    IEnumerator WaitForSeconds(float waitSeconds, Action callback)
    {
        yield return new WaitForSeconds(waitSeconds);
        callback();
    }

    public void StopAllSFX()
    {
        foreach (var audioSource in _sfxAudioSources)
        {
            StopSound(audioSource);
        }
    }
    #endregion

    #region Play Specific Sounds
    public void PlaySFX(SfxType type)
    {
        var data = sfxs.Find(item => (item.type == type));
        if (data != null)
            PlaySFX(data.audioClip);
        else
            Debug.LogWarning($"{GetType()}::{nameof(PlaySFX)}: 해당 타입의 데이터가 없음. sfx type({type})");
    }

    private void PlaySound(AudioSource audio, AudioClip clip, bool loop)
    {
        if (audio != null && clip != null)
        {
            audio.Stop();
            audio.loop = loop;
            audio.clip = clip;
            audio.Play();
        }
    }

    private void StopSound(AudioSource audio)
    {
        if (audio != null)
            audio.Stop();
    }
    #endregion
}
