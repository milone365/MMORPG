using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipSlot : MonoBehaviour, IDragHandler, IDropHandler,IEndDragHandler,IPointerEnterHandler
{
    [SerializeField]
    RectTransform icon = null;
    UnityEngine.UI.Image img;
    [SerializeField]
    Equip equip = null;
    [SerializeField]
    float lenght = 10;
    PlayerPanel panel;
    public void OnDrag(PointerEventData eventData)
    {
        icon.position = Input.mousePosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Vector3 origin = icon.position + (icon.forward * lenght);
        Vector3 direction = icon.position + (-icon.forward * lenght);
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction);
        foreach (var h in hits)
        {
            BodySlot body = h.transform.GetComponent<BodySlot>();
            if(body!=null)
            {
                if(body.SetUp(equip,panel.inventory))
                {
                    panel.inventory.RemoveItem(equip);
                    panel.ShowSlots(equip.type);
                }
            }
        }
        icon.localPosition = Vector3.zero;
    }

    public void Init(Equip equip,PlayerPanel panel)
    {
        this.panel = panel;
        this.equip = equip;
        icon.GetComponent<UnityEngine.UI.Image>().color = Helper.GetColor(equip.rarety);
        img = icon.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        if(img!=null)
        {
            img.sprite = equip.sprite;
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        icon.localPosition = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        panel.ShowDifference(equip);
    }
}
