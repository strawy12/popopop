using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EBlockState = GameManager.EBlockState;

public class ImageManager : MonoBehaviour
{
    private Sprite[] blockSprites = null;
    [SerializeField]private Sprite[] characterSprites = null;


    private string spriteKey = "PuyoZZik";

    private void Awake()
    {
        blockSprites = Resources.LoadAll<Sprite>(spriteKey);
    }

    public void SetSpritePlayer(SpriteRenderer sp)
    {
        sp.sprite = characterSprites[PlayerPrefs.GetInt("SpriteNum", 18)];
    }

    public void SetSprite(SpriteRenderer sp, EBlockState state)
    {
        sp.sprite = blockSprites[(int)state];
    }
}
