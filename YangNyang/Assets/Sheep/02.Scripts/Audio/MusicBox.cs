using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    [Header("[BGM]")]
    [SerializeField]
    public List<AssetReferenceT<AudioClip>> bgmReferences;
    [Header("[AudioClips]")]
    [SerializeField] private List<SfxData> sfxs;


    [Header("[Debug]")]
    [SerializeField, ReadOnly]
    private int _bgmIndex = -1;
    [SerializeField, ReadOnly]
    private List<AudioSource> _sfxAudioSources;//= new List<AudioSource>();
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

    #region Addresables
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
    //public void PlayBGM(AudioClip clip)
    //{
    //    UtilFunctions.PlaySound(bgmAudioSource, clip, true);
    //}
    //public void StopBGM()
    //{
    //    UtilFunctions.StopSound(bgmAudioSource);
    //}

    public void PlayBGM()
    {
        StopBGM();

        if (bgmReferences.Count <= 0)
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlayBGM)}: ��ϵ� BGM �� ����.");
            return;
        }

        // loop cycle.
        _bgmIndex++;
        if (_bgmIndex >= bgmReferences.Count)
            _bgmIndex = 0;

        // assetReference �� ����ִ��� üũ.
        // https://forum.unity.com/threads/how-to-test-for-empty-assetreference.649999/
        if (!bgmReferences[_bgmIndex].RuntimeKeyIsValid())
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlayBGM)}: ��ϵ� BGM�� ������ ����.");
            return;
        }


        _playBGMCoroutine = StartCoroutine(LoadAndPlayBGMCoroutine(
            bgmAudioSource, bgmReferences[_bgmIndex],
            () =>
            {
                // ���������� ������ ����Ǿ��ٸ� ������ �÷���.
                PlayBGM();
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

    IEnumerator LoadAndPlayBGMCoroutine(AudioSource audioSource, AssetReferenceT<AudioClip> audioClipReference, Action cbEnd)
    {
        // Get and assign the AudioClips
        _bgmAssetHandle = Addressables.LoadAssetAsync<AudioClip>(audioClipReference);

        // yielding when already done still waits until the next frame
        // so don't yield if done.
        if (!_bgmAssetHandle.IsDone)
            yield return _bgmAssetHandle;

        if (_bgmAssetHandle.Status == AsyncOperationStatus.Succeeded)
        {
            _isLoadedBGMAssetReference = true;
            var clip = _bgmAssetHandle.Result;
            PlaySound(audioSource, clip, false);
            Debug.Log($"{GetType()}::{nameof(LoadAndPlayBGMCoroutine)}: AudioClip({audioClipReference}) �ε� ����, length={clip.length}");

            // ���� ���������� ���.
            // yield return new WaitWhile(() => audioSource.isPlaying); <- �ŷ����� üũ�ϹǷ� WaitForSeconds�� ����ϴ� ������ ����.
            yield return new WaitForSeconds(clip.length);

            // Release the AudioClip from memory
            ReleaseBGMAsset();

            cbEnd?.Invoke();
        }
        else
        {
            Debug.LogWarning($"{GetType()}::{nameof(LoadAndPlayBGMCoroutine)}: AudioClip({audioClipReference}) �ε� ����.");
        }
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


    /// <summary>
    /// ������ ����ð��� ���������� ����Ѵ�.��ŵ�� �Ұ����ϴ�.
    /// </summary>
    /// <param name="clip">����� ����</param>
    /// <param name="playtime">����� �ð�</param>
    public void PlaySFX(AudioClip clip, float playtime)
    {
        if (_sfxAudioSources.Count <= 0)
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlaySFX)}: ���õǾ� �ִ� SFX AudioSource �� ����.");
            return;
        }

        // cycle

        var currentIndex = GetUsableIndex();


        //����Ʈ�� ���� ��ġ�� �߰�
        _skipIndexs.Add(currentIndex);

        var audioSource = _sfxAudioSources[currentIndex];

        PlaySound(audioSource, clip, false);
        StartCoroutine(WaitForSeconds(playtime, () =>
        {
            StopSound(audioSource);
            _skipIndexs.Remove(currentIndex);

        }));
    }

    /// <summary>
    /// skipIndexs�� ���Ե������� ��������� Index�� ã���ϴ�.
    /// SipIndexs�� ũ���_sfxAudioSources�� ũ�Ⱑ ���Ƽ� ���̻� ���� ������ ���ٸ�, �� AudioSource�� ����ϴ�.
    /// </summary>
    /// <returns></returns>
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
        //UtilFunctions.StopSound(sfxAudioSource);
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