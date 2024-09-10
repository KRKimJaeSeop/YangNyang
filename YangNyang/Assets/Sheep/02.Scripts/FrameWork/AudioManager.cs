using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


/// <summary>
/// https://gamedevbeginner.com/the-right-way-to-make-a-volume-slider-in-unity-using-logarithmic-conversion/
/// </summary>
public class AudioManager : Singleton<AudioManager>
{

    [SerializeField]
    private MusicBox musicBox;
    public  MusicBox MusicBox { get { return musicBox; } }
    private const float MAX_VOLUME = 1f; // 최대 볼륨값
    private const float MIN_VOLUME = 0.0001f; // 최소 볼륨값. (0f는 값이 깨짐.)



    public enum MixerGroup
    {
        Master,
        BGM,
        SFXMaster,
        SFX, // 소리가 나오면 BGM의 소리가 작아진다.
        UI,
    }

    public class GroupVolume
    {
        public MixerGroup group;
        public float volume;

        public GroupVolume(MixerGroup group, float volume)
        {
            this.group = group;
            this.volume = volume;
        }
    }

    [Header("[Settings]")]
    [SerializeField] private AudioMixer mixer;
    private List<GroupVolume> _listMute = new List<GroupVolume>();
    public bool IsInitialized { get; private set; }


    /// <summary>
    /// 초기화. 저장소에서 데이터를 가져와 세팅 하도록 한다.
    /// </summary>
    /// <param name="bgmVolume"></param>
    /// <param name="sfxVolume"></param>
    public void Initialize(float bgmVolume, float sfxVolume)
    {
        if (IsInitialized == true)
            return;

        SetVolume(MixerGroup.BGM, bgmVolume);
        SetVolume(MixerGroup.SFXMaster, sfxVolume);
    }

    public AudioMixerGroup[] FindMatchingGroups(MixerGroup group)
    {
        return mixer.FindMatchingGroups(group.ToString());
    }

    public AudioMixerGroup FindGroup(MixerGroup group)
    {
        AudioMixerGroup[] groups = mixer.FindMatchingGroups(group.ToString());
        for (int i = 0; i < groups.Length; ++i)
        {
            if (groups[i].name == group.ToString())
            {
                return groups[i];
            }
        }
        return null;
    }

    private string GetVolumeName(MixerGroup group)
    {
        // audio mixer 에 Expose 한 파라미터를 그룹네임+Volume으로 세팅해 놓았음.
        //return string.Format("{0}Volume", group.ToString());
        return $"{group.ToString()}Volume";
    }

    /// <summary>
    /// 0.0001f ~ 1f 으로 세팅.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="value"></param>
    public void SetVolume(MixerGroup group, float value)
    {
        value = Mathf.Clamp(value, MIN_VOLUME, MAX_VOLUME);
        mixer.SetFloat(GetVolumeName(group), Mathf.Log10(value) * 20);
        //DataManager.Instance.Storages.Preferences.SetVolume(group, value);
    }

    public void Mute(MixerGroup group)
    {
        AudioMixerGroup audioGroup = FindGroup(group);
        if (audioGroup == null)
            return;

        GroupVolume groupVolume = _listMute.Find(item => (item.group == group));
        if (groupVolume != null) // 이미 mute 되어 있다면 무시.
            return;

        // 백업.
        groupVolume = new GroupVolume(group, MIN_VOLUME);
        mixer.GetFloat(GetVolumeName(group), out groupVolume.volume);

        if (mixer.SetFloat(GetVolumeName(group), MIN_VOLUME))
            _listMute.Add(groupVolume);
    }

    public void UnlockMute(MixerGroup group)
    {
        GroupVolume groupVolume = _listMute.Find(item => (item.group == group));
        if (groupVolume == null) // 백업되어 있지 않다면 무시.
            return;

        // 복구.
        mixer.SetFloat(GetVolumeName(group), groupVolume.volume);
        _listMute.Remove(groupVolume);
    }
}