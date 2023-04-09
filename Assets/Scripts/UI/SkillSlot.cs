using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SkillSlot : MonoBehaviour,IDragHandler,IDropHandler,IEndDragHandler
{
    [SerializeField]
    RectTransform icon = null;
    [SerializeField]
    Skill skill=null;
    [SerializeField]
    float lenght = 10;

    public void OnDrag(PointerEventData eventData)
    {
        icon.position = Input.mousePosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Vector3 origin = icon.position + (icon.forward * lenght);
        Vector3 direction = icon.position + (-icon.forward * lenght);
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction);
        foreach(var h in hits)
        {
            ActionButton b = h.transform.GetComponent<ActionButton>();
            if(b!=null)
            {
                b.SetSkill(skill);
                break;
            }
        }
        icon.localPosition = Vector3.zero;
    }

    public void Init(Skill skill)
    { 
        this.skill = skill;
        icon.GetComponent<UnityEngine.UI.Image>().sprite = skill.sprite;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        icon.localPosition = Vector3.zero;
    }
}
