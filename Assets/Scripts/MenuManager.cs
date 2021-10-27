using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Image[] storeImages;
    void Start()
    {
        if(PlayerPrefs.GetInt("SpriteNum", 100) == 88)
        {
            PlayerPrefs.SetInt("SpriteNum", 0);
            return;
        }
        storeImages[PlayerPrefs.GetInt("SpriteNum")].color = Color.gray;
    }

    void Update()
    {
        
    }

    public void OnClickPurchaseBtn(int num)
    {
        if (PlayerPrefs.GetInt("SpriteNum") == num) return;
        storeImages[PlayerPrefs.GetInt("SpriteNum")].color = Color.yellow;
        PlayerPrefs.SetInt("SpriteNum", num);
        storeImages[num].color = Color.gray;
    }
}
