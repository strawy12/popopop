using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Player : Block
{
    public override void SetSpawnCoords()
    {
        transform.localScale = Vector3.zero;
        transform.position = SetCoords();
        transform.DOScale(Vector3.one, 0.1f);
    }
    public override void ChangeColor()
    {
        base.ChangeColor();
        spriteRenderer.color = Color.red;
    }
}
