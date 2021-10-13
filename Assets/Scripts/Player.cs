using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Block
{

    public override void ChangeColor()
    {
        base.ChangeColor();
        spriteRenderer.color = Color.red;
    }
}
