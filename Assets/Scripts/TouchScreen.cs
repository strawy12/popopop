using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchScreen : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    bool HV = false;
    bool isPlma = false;
    bool isDrag = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Inst.Loading) return;
        HV = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Inst.Loading) return;

        if (HV) // Horizontal
        {
            isPlma = eventData.delta.x > 0;
        }
        else // Vertical
        {
            isPlma = eventData.delta.y > 0;
        }
        if(!isDrag)
        {
            isDrag = true;
            GameManager.Inst.MoveBlock(HV, isPlma);
            Invoke("EndDrag", 1f);

        }
    }

    private void EndDrag()
    {
        isDrag = false;
    }
}
