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
    private const float MAX_VOLUME = 1f; // �ִ� ������
    private const float MIN_VOLUME = 0.0001f; // �ּ� ������. (0f�� ���� ����.)



    public enum MixerGroup
    {
        Master,
        BGM,
        SFXMaster,
        SFX, // �Ҹ��� ������ BGM�� �Ҹ��� �۾�����.
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
    /// �ʱ�ȭ. ����ҿ��� �����͸� ������ ���� �ϵ��� �Ѵ�.
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
        // audio mixer �� Expose �� �Ķ���͸� �׷����+Volume���� ������ ������.
        //return string.Format("{0}Volume", group.ToString());
        return $"{group.ToString()}Volume";
    }

    /// <summary>
    /// 0.0001f ~ 1f ���� ����.
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
        if (groupVolume != null) // �̹� mute �Ǿ� �ִٸ� ����.
            return;

        // ���.
        groupVolume = new GroupVolume(group, MIN_VOLUME);
        mixer.GetFloat(GetVolumeName(group), out groupVolume.volume);

        if (mixer.SetFloat(GetVolumeName(group), MIN_VOLUME))
            _listMute.Add(groupVolume);
    }

    public void UnlockMute(MixerGroup group)
    {
        GroupVolume groupVolume = _listMute.Find(item => (item.group == group));
        if (groupVolume == null) // ����Ǿ� ���� �ʴٸ� ����.
            return;

        // ����.
        mixer.SetFloat(GetVolumeName(group), groupVolume.volume);
        _listMute.Remove(groupVolume);
    }
}