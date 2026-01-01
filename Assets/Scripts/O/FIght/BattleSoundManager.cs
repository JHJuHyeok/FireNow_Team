using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSoundManager : MonoBehaviour
{
    [Header("BGM Settings")]
    [SerializeField] private string normalBattleBGM = "BattleScene_BGM";
    [SerializeField] private string bossBattleBGM = "BattleScene_BGM";

    [Header("Effect Sounds")]
    [SerializeField] private string waveStartSound = "wave_start";
    [SerializeField] private string bossWarningSound = "boss_warning";
    [SerializeField] private string victorySound = "victory";
    [SerializeField] private string defeatSound = "defeat";

    private string currentBGM = "";

    void Start()
    {
        // 전투 시작 시 일반 배틀 BGM 재생
        PlayBattleBGM();
    }

    // 일반 전투 BGM 재생
    public void PlayBattleBGM()
    {
        if (!string.IsNullOrEmpty(currentBGM))
        {
            SoundManager.Instance.StopLoopSound(currentBGM);
        }

        SoundManager.Instance.PlaySound(normalBattleBGM, 0f, true, SoundType.bgm);
        currentBGM = normalBattleBGM;
    }

    // 보스 BGM 재생
    public void PlayBossBGM()
    {
        if (!string.IsNullOrEmpty(currentBGM))
        {
            SoundManager.Instance.StopLoopSound(currentBGM);
        }

        SoundManager.Instance.PlaySound(bossBattleBGM, 0f, true, SoundType.bgm);
        currentBGM = bossBattleBGM;
    }

    // 웨이브 시작 사운드
    public void PlayWaveStartSound()
    {
        SoundManager.Instance.PlaySound(waveStartSound);
    }

    // 보스 경고 사운드
    public void PlayBossWarningSound()
    {
        SoundManager.Instance.PlaySound(bossWarningSound);
    }

    // 승리 사운드
    public void PlayVictorySound()
    {
        if (!string.IsNullOrEmpty(currentBGM))
        {
            SoundManager.Instance.StopLoopSound(currentBGM);
        }

        SoundManager.Instance.PlaySound(victorySound, 0f, false, SoundType.bgm);
    }

    // 패배 사운드
    public void PlayDefeatSound()
    {
        if (!string.IsNullOrEmpty(currentBGM))
        {
            SoundManager.Instance.StopLoopSound(currentBGM);
        }

        SoundManager.Instance.PlaySound(defeatSound, 0f, false, SoundType.bgm);
    }

    // BGM 정지
    public void StopBGM()
    {
        if (!string.IsNullOrEmpty(currentBGM))
        {
            SoundManager.Instance.StopLoopSound(currentBGM);
            currentBGM = "";
        }
    }

    void OnDestroy()
    {
        // 씬 전환 시 BGM 정지
        StopBGM();
    }
}