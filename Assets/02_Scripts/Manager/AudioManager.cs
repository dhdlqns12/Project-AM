using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioMixerGroupName
{
    Master,
    BGM,
    SFX
}

public class AudioManager : Singleton<AudioManager>
{
    [Header("UserAudioMixer")]
    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    
    [Header("AudioSource")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource interfaceAudioSource;

    [Header("BGM Clips")] 
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip inGameBGM;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private AudioClip warriorAttackSFX;
    [SerializeField] private AudioClip archerAttackSFX;
    [SerializeField] private AudioClip damageSFX;
    [SerializeField] private AudioClip deathSFX;

    private Dictionary<AudioMixerGroupName, AudioMixerGroup> userAudios;
    
    private Dictionary<string, float> lastSFXPlaytime = new();
    private float coolDownTime = 0.1f;

    /// <summary>
    /// 오디오매니저 초기화
    /// 오디오 믹서 메핑, 
    /// </summary>
    protected override void Init()
    {
        userAudios = new()
        {
            { AudioMixerGroupName.Master, masterMixerGroup },
            { AudioMixerGroupName.BGM, bgmMixerGroup },
            { AudioMixerGroupName.SFX, sfxMixerGroup },
        };
    }

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

    public void PlayTitleBGM() => PlayBGM(titleBGM);

    public void PlayInGameBGM() => PlayBGM(inGameBGM);

    private void PlayBGM(AudioClip clip, float volume = 1.0f)
    {
        if(clip == null) return;
        if(bgmAudioSource.clip == clip)  return;
        
        bgmAudioSource.clip = clip;
        bgmAudioSource.volume = volume;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    public void PlayClickSFX() => interfaceAudioSource.PlayOneShot(clickSFX);

    public void WarriorAttackSFX() => PlaySFXWithLimit("WarriorAtk", sfxAudioSource, warriorAttackSFX, coolDownTime);

    public void ArcherAtttackSFX() => PlaySFXWithLimit("ArcherAtk", sfxAudioSource,archerAttackSFX, coolDownTime);

    public void DamageSFX() => PlaySFXWithLimit("Damage", sfxAudioSource, damageSFX, coolDownTime);
    public void PlayDeathSFX() => PlaySFXWithLimit("Death", sfxAudioSource, deathSFX, coolDownTime);
    
    private void PlaySFXWithLimit(string key, AudioSource audioSource, AudioClip clip , float coolTime)
    {
        if (audioSource == null || clip == null)
        {
            Debug.Log("효과음 재생 불가");
            return;
        }
        
        float now = Time.time;

        if (lastSFXPlaytime.TryGetValue(key, out float time))
        {
            if(now - time < coolTime) return;
        }
        
        audioSource.PlayOneShot(clip);

        lastSFXPlaytime[key] = now;
        
        Debug.Log($"{key} 효과음 재생");
    }

}
