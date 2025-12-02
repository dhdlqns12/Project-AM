using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioMixerGroupName
{
    Master,
    BGM,
    SFX
}

public class AudioManager : MonoBehaviour
{
    [Header("UserAudioMixer")]
    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    
    [Header("AudioSource")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource interfaceAudioSource;

    // [Header("BGM Clips")] 
    // [SerializeField] private AudioClip titleBGM;
    // [SerializeField] private AudioClip inGameBGM;
    //
    // [Header("SFX Clips")]
    // [SerializeField] private AudioClip clickSFX;
    // [SerializeField] private AudioClip warriorAttackSFX;
    // [SerializeField] private AudioClip archerAttackSFX;
    // [SerializeField] private AudioClip damageSFX;
    // [SerializeField] private AudioClip deathSFX;

    private Dictionary<AudioMixerGroupName, AudioMixerGroup> userAudios;
    
    private Dictionary<string, float> lastSFXPlaytime = new(); // 효과음 쿨타임 계산
    private float coolDownTime = 0.1f;

    private Dictionary<string, BgmData> bgmDataDict = new(); //bgm데이터 테이블 로드용
    private Dictionary<string, AudioClip> bgmClipDict = new(); //bgm클립 로드용
    
    
    private Dictionary<string, SfxData> sfxDataDict = new(); //sfx데이터 테이블 로드용
    private Dictionary<string, AudioClip> sfxClipDict = new(); //sfx클립 로드용

    /// <summary>
    /// 오디오매니저 초기화
    /// 오디오 믹서 메핑, 
    /// </summary>
    public void Init()
    {
        userAudios = new()
        {
            { AudioMixerGroupName.Master, masterMixerGroup },
            { AudioMixerGroupName.BGM, bgmMixerGroup },
            { AudioMixerGroupName.SFX, sfxMixerGroup },
        };
        
        LoadBGmTable(); 
        LoadSFxTable();
        
        Debug.Log("오디오매니저 초기화");
    }

    #region 배경음 및 효과음 로드

    private void LoadBGmTable()
    {
        var table = ResourceManager.LoadJsonDataList<BgmData>("BGMData");
        if (table == null)
        {
            Debug.Log("bgm데이터 테이블 비어있음");
            return;
        }

        foreach (var item in table)
        {
            bgmDataDict[item.File_Name] = item;
            
            AudioClip clip = ResourceManager.LoadAsset<AudioClip>($"Audio/BGM/{item.File_Name}");
            bgmClipDict[item.File_Name] = clip;
        }
    }
    
    private void LoadSFxTable()
    {
        var table = ResourceManager.LoadJsonDataList<SfxData>("SFXData");
        if (table == null)
        {
            Debug.Log("sfx데이터 테이블 비어있음");
            return;
        }
        
        foreach (var item in table)
        {
            Debug.Log(item.File_Name +" 로드중"+", 볼륨: " + item.Volume);
            sfxDataDict[item.File_Name] = item;
            
            AudioClip clip = ResourceManager.LoadAsset<AudioClip>($"Audio/SFX/{item.File_Name}");
            sfxClipDict[item.File_Name] = clip;
        }
        
    }
    
    #endregion

    #region 배경음 로직 개선 후

    public void PlayBGmByFileName(string fileName)
    {
        if (!bgmDataDict.TryGetValue(fileName, out var bgmData))
        {
            Debug.LogWarning($"{fileName}클립 데이터가 존재하지 않습니다.");
            return;
        }

        if (!bgmClipDict.TryGetValue(fileName, out var bgmClip))
        {
            Debug.LogWarning($"{fileName}클립 음원이 존재하지 않습니다.");
            return;
        }
        
        PlayBGmInternal(bgmClip,bgmData.Volume,bgmData.Loop);
        
    }

    private void PlayBGmInternal(AudioClip clip,float volume, bool loop)
    {
        if (clip == null)
        {
            Debug.LogWarning("오디오 클립이 없습니다.");
            return;
        }   
        
        if (bgmAudioSource.clip == clip) return;
        
        bgmAudioSource.clip = clip;
        bgmAudioSource.volume = volume;
        bgmAudioSource.loop = loop;
        bgmAudioSource.Play();
        
    }
    
    #endregion
    

    #region 볼륨 제어

    public void SetVolume(AudioMixerGroupName key, float volume)
    {
        var sKey = key.ToString();

        if (userAudios.ContainsKey(key))
        {
            userAudios[key].audioMixer.SetFloat(sKey, volume);
        }
        else
        {
            Debug.Log("AudioMixer not found: " + key);
        }
    }
    
    public float GetVolume(AudioMixerGroupName key)
    {
        var sKey = key.ToString();

        if (userAudios[key].audioMixer.GetFloat(sKey, out float volume))
        {
            return volume;
        }
        else
        {
            Debug.Log("AudioMixer not found: " + key);
            return 0f;
        }
    }
    
    #endregion

    #region 배경음 재생 개선 전
    //
    // public void PlayTitleBGM() => PlayBGM(titleBGM);
    //
    // public void PlayInGameBGM() => PlayBGM(inGameBGM);
    //
    // private void PlayBGM(AudioClip clip, float volume = 1.0f)
    // {
    //     if (clip == null)
    //     {
    //         Debug.Log("배경음 클립이 비어있습니다.");
    //         return;
    //     }
    //     if(bgmAudioSource.clip == clip)  return;
    //     
    //     bgmAudioSource.clip = clip;
    //     bgmAudioSource.volume = volume;
    //     bgmAudioSource.loop = true;
    //     bgmAudioSource.Play();
    // }
    
    #endregion

    #region 효과음 재생
/// <summary>
/// 각종효과음 발동 매서드입니다.
/// </summary>
    public void PlayClickSFX() => PlaySFX("Click", interfaceAudioSource, 0f);

    public void WarriorAttackSFX() => PlaySFX("warrior_swing", sfxAudioSource, coolDownTime);

    public void ArcherAtttackSFX() => PlaySFX("ArcherAtk", sfxAudioSource, coolDownTime);

    public void DamageSFX() => PlaySFX("Damage", sfxAudioSource, coolDownTime);
    public void PlayDeathSFX() => PlaySFX("Death", sfxAudioSource, coolDownTime);

    public void PlaySFX(string fileName, AudioSource audioSource, float coolDownTime)
    {
        if (!sfxDataDict.TryGetValue(fileName, out var data))
        {
            Debug.LogWarning($"{fileName}클립 데이터가 존재하지 않습니다.");
            return;
        }

        if (!sfxClipDict.TryGetValue(fileName, out var clip))
        {
            Debug.LogWarning($"{fileName}클립 데이터가 존재하지 않습니다.");
            return;
        }
        
        PlaySFXWithLimit(fileName, audioSource, clip, coolDownTime,data.Volume);
        
    }
    
    private void PlaySFXWithLimit(string fileName, AudioSource audioSource, AudioClip clip , float coolTime,float volume)
    {
        if (audioSource == null || clip == null)
        {
            Debug.Log("효과음 재생 불가");
            return;
        }
        
        float now = Time.time;

        if (lastSFXPlaytime.TryGetValue(fileName, out float time))
        {
            if(now - time < coolTime) return;
        }
        
        audioSource.PlayOneShot(clip, volume);

        lastSFXPlaytime[fileName] = now;
        
        Debug.Log($"{fileName} 효과음 재생, 볼륨 크기: {volume}" );
    }

    #endregion

}
