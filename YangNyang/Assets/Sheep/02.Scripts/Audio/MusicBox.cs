using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
            Debug.LogWarning($"{GetType()}::{nameof(PlayBGM)}: 등록된 BGM 이 없음.");
            return;
        }

        // loop cycle.
        _bgmIndex++;
        if (_bgmIndex >= bgmReferences.Count)
            _bgmIndex = 0;

        // assetReference 가 비어있는지 체크.
        // https://forum.unity.com/threads/how-to-test-for-empty-assetreference.649999/
        if (!bgmReferences[_bgmIndex].RuntimeKeyIsValid())
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlayBGM)}: 등록된 BGM에 에셋이 없음.");
            return;
        }


        _playBGMCoroutine = StartCoroutine(LoadAndPlayBGMCoroutine(
            bgmAudioSource, bgmReferences[_bgmIndex],
            () =>
            {
                // 정상적으로 음악이 종료되었다면 다음곡 플레이.
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
            Debug.Log($"{GetType()}::{nameof(LoadAndPlayBGMCoroutine)}: AudioClip({audioClipReference}) 로딩 성공, length={clip.length}");

            // 곡이 끝날때까지 대기.
            // yield return new WaitWhile(() => audioSource.isPlaying); <- 매루프에 체크하므로 WaitForSeconds를 사용하는 것으로 하자.
            yield return new WaitForSeconds(clip.length);

            // Release the AudioClip from memory
            ReleaseBGMAsset();

            cbEnd?.Invoke();
        }
        else
        {
            Debug.LogWarning($"{GetType()}::{nameof(LoadAndPlayBGMCoroutine)}: AudioClip({audioClipReference}) 로딩 실패.");
        }
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


    /// <summary>
    /// 지정한 재생시간이 끝날때까지 재생한다.스킵이 불가능하다.
    /// </summary>
    /// <param name="clip">재생할 음원</param>
    /// <param name="playtime">재생할 시간</param>
    public void PlaySFX(AudioClip clip, float playtime)
    {
        if (_sfxAudioSources.Count <= 0)
        {
            Debug.LogWarning($"{GetType()}::{nameof(PlaySFX)}: 세팅되어 있는 SFX AudioSource 가 없음.");
            return;
        }

        // cycle

        var currentIndex = GetUsableIndex();


        //리스트에 현재 위치를 추가
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
    /// skipIndexs에 포함되지않은 재생가능한 Index를 찾습니다.
    /// SipIndexs의 크기와_sfxAudioSources의 크기가 같아서 더이상 남은 공간이 없다면, 새 AudioSource를 만듭니다.
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