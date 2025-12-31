using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// 오디오 믹서 그룹명
public enum SoundType
{
    bgm,
    effect
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 설정")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip[] audioClips;

    private float mCurrentVolume;

    private Dictionary<string, AudioClip> clipDict;

    private List<TemporarySoundPlayer> instantiateSounds;

    private void Start()
    {
        clipDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in audioClips)
        {
            clipDict.Add(clip.name, clip);
        }

        instantiateSounds = new List<TemporarySoundPlayer>();

        OptionEvent.OnOptionChanged += OptionToggleEvent;
    }

    /// <summary>
    /// 오디오 이름 기반으로 오디오 클립 호출
    /// </summary>
    /// <param name="clipName"> 오디오 이름 </param>
    /// <returns> 이름과 일치하는 오디오 클립 </returns>
    private AudioClip GetClip(string clipName)
    {
        AudioClip clip = clipDict[clipName];

        if (clip == null) return null;

        return clip;
    }

    /// <summary>
    /// 사운드가 자동 삭제되지 않고 루프 형태일 경우 삭제하기 위해 리스트 저장
    /// </summary>
    /// <param name="soundPlayer"> 사운드 플레이어 </param>
    private void AddToList(TemporarySoundPlayer soundPlayer)
    {
        instantiateSounds.Add(soundPlayer);
    }

    /// <summary>
    /// 루프 사운드 중지
    /// </summary>
    /// <param name="clipName"> 오디오 클립 이름 </param>
    public void StopLoopSound(string clipName)
    {
        foreach(TemporarySoundPlayer soundPlayer in instantiateSounds)
        {
            if(soundPlayer.ClipName == clipName)
            {
                instantiateSounds.Remove(soundPlayer);
                Destroy(soundPlayer.gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// 사운드 실행
    /// </summary>
    /// <param name="clipName"> 사운드 클립 이름 </param>
    /// <param name="delay"> 오디오 딜레이 </param>
    /// <param name="isLoop"> 루프 여부 </param>
    /// <param name="type"> 사운드 타입 </param>
    public void PlaySound(string clipName, float delay = 0f, bool isLoop = false, SoundType type = SoundType.effect)
    {
        GameObject obj = new GameObject("SoundPlayer");
        TemporarySoundPlayer soundPlayer = obj.AddComponent<TemporarySoundPlayer>();

        // 루프면 사운드 저장
        if (isLoop) { AddToList(soundPlayer); }

        soundPlayer.InitSound(GetClip(clipName));
        soundPlayer.PlayAudio(audioMixer.FindMatchingGroups(type.ToString())[0], delay, isLoop);
    }

    /// <summary>
    /// 사운드 볼륨을 저장된 수치로 초기화
    /// </summary>
    /// <param name="bgm"> bgm 볼륨 수치 </param>
    /// <param name="effect"> 이펙트 볼륨 수치 </param>
    public void InitVolumes(float bgm, float effect)
    {
        SetVolume(SoundType.bgm, bgm);
        SetVolume(SoundType.effect, effect);
    }

    /// <summary>
    /// 볼륨 조절 함수
    /// </summary>
    /// <param name="type"> 사운드 타입 </param>
    /// <param name="value"> 설정 볼륨 </param>
    public void SetVolume(SoundType type, float value)
    {
        audioMixer.SetFloat(type.ToString(), value);
    }

    /// <summary>
    /// 옵션 변경 이벤트 함수
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    private void OptionToggleEvent(OptionToggleControl.ToggleOptionType type, bool value)
    {
        if (type == OptionToggleControl.ToggleOptionType.SFX)
        {
            if (value == true)
            {
                SetVolume(SoundType.effect, -20.0f);
            }
            if (value == false)
            {
                SetVolume(SoundType.effect, -80.0f);
            }
        }
        else if (type == OptionToggleControl.ToggleOptionType.BGM)
        {
            if (value == true)
            {
                SetVolume(SoundType.bgm, -20.0f);
            }
            if (value == false)
            {
                SetVolume(SoundType.bgm, -80.0f);
            }
        }
    }
}
