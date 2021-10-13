using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EObjectType = GameManager.EObjectType;

public class Block : MonoBehaviour
{
    public bool isBonus;
    public int xPos;
    public int yPos;
    protected SpriteRenderer spriteRenderer = null;
    public void SetSpawnCoords()
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
    }

    public void SetMoveCoords()
    {
        transform.DOMove(SetCoords(), 0.15f);
    }

    private Vector2 SetCoords()
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
        gameObject.SetActive(false);
        GameManager.Inst.PoolEnqueue(EObjectType.block, gameObject);
    }
}
