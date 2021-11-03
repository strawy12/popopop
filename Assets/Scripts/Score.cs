using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EObjectType = GameManager.EObjectType;

public class Score : Block
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Block"))
        {
            GameManager.Inst.AddScore(1);
            Despawn();
        }
    }

    public override void ChangeColor()
    {
        base.ChangeColor();
        spriteRenderer.color = Color.yellow;
    }

    public override void Despawn(bool colorChanged = false)
    {
        SoundManager.Inst.SetEffectSound(5);
        gameObject.SetActive(false);
        GameManager.Inst.PoolEnqueue(EObjectType.score, gameObject);
        GameManager.Inst.ScorePositionEmpty(xPos, yPos);
    }
}
