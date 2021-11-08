using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoSingleTon<SoundManager>
{
    [SerializeField] AudioClip[] bgms = null;
    [SerializeField] AudioClip[] effectSounds = null;
    private AudioSource bgmAudio;
    private AudioSource playerEffectAudio;
    private AudioSource effectAudio;
    private void Awake()
    {
        SoundManager[] smanagers = FindObjectsOfType<SoundManager>();
        if (smanagers.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        bgmAudio = GetComponent<AudioSource>();
        effectAudio = transform.GetChild(0).GetComponent<AudioSource>();
        playerEffectAudio = transform.GetChild(1).GetComponent<AudioSource>();
        VolumeSetting();
    }
    public void VolumeSetting()
    {
        bgmAudio.volume = PlayerPrefs.GetFloat("BgmAudio", 0.5f);
        effectAudio.volume = PlayerPrefs.GetFloat("EffectAudio", 0.5f);
        playerEffectAudio.volume = PlayerPrefs.GetFloat("EffectAudio", 0.5f);
        effectAudio.mute = GetBGMMute();
        effectAudio.mute = GetEffectMute();
        playerEffectAudio.mute = GetPlayerEffectMute();
    }

    public float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat("BgmAudio", 0.5f);
    }
    public bool GetBGMMute()
    {
        return PlayerPrefs.GetInt("BgmMute", 1) == 0;
    }
    public bool GetEffectMute()
    {
        return PlayerPrefs.GetInt("EffectMute", 1) == 0;
    }
    public bool GetPlayerEffectMute()
    {
        return PlayerPrefs.GetInt("EffectMute", 1) == 0 || PlayerPrefs.GetInt("PlayerMute", 1) == 0;
    }
    public float GetEffectVolume()
    {
        return PlayerPrefs.GetFloat("EffectAudio", 0.5f);
    }

    public void BGMVolume(float value)
    {
        if (bgmAudio == null) return;
        PlayerPrefs.SetFloat("BgmAudio", value);
        Debug.Log(PlayerPrefs.GetFloat("BgmAudio", 0.5f));
        Debug.Log(value);
        bgmAudio.volume = PlayerPrefs.GetFloat("BgmAudio", 0.5f);
    }

    public void EffectVolume(float value)
    {
        if (effectAudio == null) return;
        PlayerPrefs.SetFloat("EffectAudio", value);

        effectAudio.volume = PlayerPrefs.GetFloat("EffectAudio", 0.5f);
        playerEffectAudio.volume = PlayerPrefs.GetFloat("EffectAudio", 0.5f);

    }

    public void BGMMute(bool isMute)
    {
        PlayerPrefs.SetInt("BgmMute", isMute ? 0 : 1);
        bgmAudio.mute = PlayerPrefs.GetInt("BgmMute", 1) == 0;
    }
    public void EffectMute(bool isMute)
    {
        PlayerPrefs.SetInt("EffectMute", isMute ? 0 : 1);
        
        effectAudio.mute = PlayerPrefs.GetInt("EffectMute", 1) == 0;
        playerEffectAudio.mute = PlayerPrefs.GetInt("EffectMute", 1) == 0 || PlayerPrefs.GetInt("PlayerMute", 1) == 0;
    }

    public void PlayerEffectMute(bool isMute)
    {
        PlayerPrefs.SetInt("PlayerMute", isMute ? 0 : 1);

        playerEffectAudio.mute = PlayerPrefs.GetInt("EffectMute", 1) == 0 || PlayerPrefs.GetInt("PlayerMute", 1) == 0;
    }


    public void SetBGM(int bgmNum)
    {
        bgmAudio.Stop();
        bgmAudio.clip = bgms[bgmNum];
        bgmAudio.Play();
    }
    public void SetEffectSound(int effectNum)
    {
        effectAudio.clip = effectSounds[effectNum];
        effectAudio.Play();
    }    
    public void SetPlayerEffectSound(int effectNum)
    {
        playerEffectAudio.clip = effectSounds[effectNum];
        playerEffectAudio.Play();
    }
    public void StopBGM()
    {
        bgmAudio.Stop();
    }
    
}
