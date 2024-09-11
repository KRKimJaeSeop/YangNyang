using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static AddressableManager;

/// <summary>
/// ����ȿ���� (��,����� ����) ���� �ƴ� ���� ������ �����Ѵ�.
/// </summary>
public class MusicBox : MonoBehaviour
{
    public enum SfxType
    {
        None = 0,
        DefaultClick, // �⺻ Ŭ����
        OpenPositionableMenu, // ���� �̵� �޴�
    }

    [Serializable]
    private class SfxData
    {
        [Tooltip("Ÿ��")]
        public SfxType type;
        [Tooltip("��ư Ŭ����")]
        public AudioClip audioClip;
    }

    [Header("[AudioSources]")]
    [SerializeField]
    private AudioSource bgmAudioSource;
    [SerializeField, Tooltip("SFX���� AudioSource�� �ִ� ������Ʈ")]
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
            Debug.LogWarning($"{GetType()}::{nameof(PlayBGM)}: BGM ������ ã�� �� ����. assetName: {$"{code}"}");
            return;
        }

        _playBGMCoroutine = StartCoroutine(PlayBGMCoroutine(bgmAudioSource, clip, () =>
        {
            // ���������� ������ ����Ǿ��ٸ� ������ �÷���.
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
        Debug.Log($"{GetType()}::{nameof(PlayBGMCoroutine)}: AudioClip({clip.name}) �ε� ����, length={clip.length}");

        // ���� ���������� ���.
        yield return new WaitForSeconds(clip.length);

        cbEnd?.Invoke();
    }
    #endregion // BGM

    #region SFX
    public void PlaySFX(AudioClip clip)
    {
        if (_sfxAudioSources.Count <= 0)
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlaySFX)}: ���õǾ� �ִ� SFX AudioSource �� ����.");
            return;
        }

        var audioSource = _sfxAudioSources[GetUsableIndex()];
        PlaySound(audioSource, clip, false);
    }

    public void PlaySFX(AudioClip clip, float playtime)
    {
        if (_sfxAudioSources.Count <= 0)
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlaySFX)}: ���õǾ� �ִ� SFX AudioSource �� ����.");
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
            Debug.LogWarning($"{GetType()}::{nameof(PlaySFX)}: �ش� Ÿ���� �����Ͱ� ����. sfx type({type})");
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
