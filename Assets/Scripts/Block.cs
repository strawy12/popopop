using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Block : MonoBehaviour
{
    public int xPos;
    public int yPos;

    public Block(int xPos, int yPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;
    }

    public void SetSpawnCoords()
    {
        transform.localScale = Vector3.zero;
        transform.position = SetCoords();
        transform.DOScale(Vector3.one, 0.25f);
    }

    public void SetMoveCoords()
    {
        transform.DOMove(SetCoords(), 0.25f);
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
}
