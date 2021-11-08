using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Image[] storeImages;
    [SerializeField] private Slider bgmController;
    [SerializeField] private Slider effectController;
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle effectToggle;
    [SerializeField] private Toggle playerEffectToggle;
    [SerializeField] private GameObject quitPanal = null;

    void Start()
    {
        if (PlayerPrefs.GetInt("SpriteNum", 100) == 100)
        {
            PlayerPrefs.SetInt("SpriteNum", 0);
        }

        storeImages[PlayerPrefs.GetInt("SpriteNum")].color = Color.gray;
        bgmController.value = SoundManager.Inst.GetBGMVolume();
        effectController.value = SoundManager.Inst.GetEffectVolume();
        bgmToggle.isOn = SoundManager.Inst.GetBGMMute();
        effectToggle.isOn = SoundManager.Inst.GetEffectMute();
        playerEffectToggle.isOn = SoundManager.Inst.GetPlayerEffectMute();
        SoundManager.Inst.SetBGM(1);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActiveQuitPanal(true);
        }
    }

    public void ActiveQuitPanal(bool isActive)
    {
        quitPanal.transform.DOScale(isActive ? Vector3.one : Vector3.zero, 0.2f);
    }

    public void SetBGMVolume(float value)
    {
        SoundManager.Inst.BGMVolume(value);
    }

    public void SetEffectVolume(float value)
    {
        SoundManager.Inst.EffectVolume(value);
    }
    public void BGMMute(bool isMute)
    {
        SoundManager.Inst.BGMMute(isMute);
    }
    public void EffectMute(bool isMute)
    {
        SoundManager.Inst.EffectMute(isMute);
    }
    public void PlayerEffectMute(bool isMute)
    {
        SoundManager.Inst.PlayerEffectMute(isMute);
    }
    public void OnCkickQuit()
    {
        Application.Quit();
    }
    public void OnClickStartBtn()
    {
        SceneManager.LoadScene("Main");
    }
    public void OnClickPurchaseBtn(int num)
    {
        if (PlayerPrefs.GetInt("SpriteNum") == num) return;
        storeImages[PlayerPrefs.GetInt("SpriteNum")].color = new Color(1f, 0.8647224f, 0.4941176f);
        PlayerPrefs.SetInt("SpriteNum", num);

        storeImages[num].color = Color.gray;
    }

    public void PlayEffectSound(int num)
    {
        SoundManager.Inst.SetEffectSound(num);
    }
}
