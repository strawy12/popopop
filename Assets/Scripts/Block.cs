using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EObjectType = GameManager.EObjectType;
using EBlockState = GameManager.EBlockState;
using System;

public class Block : MonoBehaviour
{
    public EBlockState state;
    public bool isBonus;
    public int xPos;
    public int yPos;
    protected SpriteRenderer spriteRenderer = null;
    protected ParticleSystem effectSystem = null;
    protected ParticleSystem.MainModule pm;
    public virtual void SetSpawnCoords()
    {
        ChangeColor();
        transform.localScale = Vector3.zero;
        transform.position = SetCoords();
        transform.DOScale(Vector3.one, 0.1f);
    }

    public virtual void ChangeColor()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (isBonus)
        {
            spriteRenderer.color = Color.green;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
        spriteRenderer.enabled = true;
    }

    public void SetBlockState()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        string state = "";
        if (!GameManager.Inst.CheckBlock(xPos, yPos + 1) && !GameManager.Inst.CheckPlayer(xPos, yPos + 1))
        {
            if(state == "")
            {
                state = "TOP";
            }
            else
            {
                state = string.Join("_", state, "TOP");
            }
        }
        if (!GameManager.Inst.CheckBlock(xPos + 1, yPos) && !GameManager.Inst.CheckPlayer(xPos + 1, yPos))
        {
            if (state == "")
            {
                state = "RIGHT";
            }
            else
            {
                state = string.Join("_", state, "RIGHT");
            }
        }

        if (!GameManager.Inst.CheckBlock(xPos - 1, yPos) && !GameManager.Inst.CheckPlayer(xPos - 1, yPos))
        {
            if (state == "")
            {
                state = "LEFT";
            }
            else
            {
                state = string.Join("_", state, "LEFT");
            }
        }
        if (!GameManager.Inst.CheckBlock(xPos, yPos - 1) && !GameManager.Inst.CheckPlayer(xPos, yPos - 1))
        {
            if (state == "")
            {
                state = "DOWN";
            }
            else
            {
                state = string.Join("_", state, "DOWN");
            }
        }

        if(state == "")
        {
            state = "NORMAL";
        }

        this.state = (EBlockState)Enum.Parse(typeof(EBlockState), state, true);
        GameManager.Inst.Image.SetSprite(spriteRenderer, this.state);
    }

    public void SetMoveCoords()
    {
        transform.DOMove(SetCoords(), 0.15f);
    }

    protected Vector2 SetCoords()
    {
        float xPos = this.xPos;
        float yPos = this.yPos;

        xPos -= 2f;
        yPos -= 2f;

        xPos *= 1f;
        yPos *= 1f;

        return new Vector2(xPos, yPos);
    }

    public virtual void Despawn()
    {
        if(effectSystem == null)
        {
            effectSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
            pm = effectSystem.main;
        }

        StartCoroutine(DespawnEffect());
    }

    private IEnumerator DespawnEffect()
    {
        spriteRenderer.enabled = false;

        if (isBonus)
        {
            pm.startColor = Color.green;
        }
        else
        {
            pm.startColor = Color.red;
        }

        effectSystem.Play();
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        isBonus = false;
        GameManager.Inst.PoolEnqueue(EObjectType.block, gameObject);
        GameManager.Inst.SpawnScore(xPos, yPos);

    }
}
