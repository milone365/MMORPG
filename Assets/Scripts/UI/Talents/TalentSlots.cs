using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TalentSlots : MonoBehaviour,IPointerEnterHandler
{
    [SerializeField]
    Button button = null;
    [SerializeField]
    Image icon = null, blackImage = null;
    [SerializeField]
    Text text = null;
    [SerializeField]
    Image[] roads = null;
    public Talent talent { get; set; }
    TalentBook book;
    SaveData data;
    public int currentAmount=0;
    public int toRemove = 0;
    public bool SlotEnable = false;
    [SerializeField]
    TalentSlots[] nextSlots = null;
    public void Initialize(TalentBook book,Talent talent,SaveData data)
    {
        this.data = data;
        button.onClick.AddListener(OnClickEvent);
        this.book = book;
        this.talent = talent;
        var saved = GetSavedTalent(talent.name, data);
        icon.sprite = talent.sprite;
        if(saved!=null)
        {
            currentAmount = saved.value;
            text.text = "(" + saved.value +")";
            blackImage.enabled = false;
            if(saved.value>=talent.maxLevel)
            {
                UnlockRoads(true);
            }
        }
        book.dataText.text = "(" + data.talentPoint + ")";
    }

    void UnlockRoads(bool val)
    {
        foreach(var r in roads)
        {
            r.enabled = val;
        }
        if(val==true)
        {
            foreach(var s in nextSlots)
            {
                s.SlotEnable = true;
            }
        }
    }

    Pair<string,int>GetSavedTalent(string Name,SaveData data)
    {
        foreach(var item in data.talentList)
        {
            if(item.key==Name)
            {
                return item;
            }
        }
        return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (talent == null) return;
        string Name = (string.IsNullOrEmpty(talent.talentName) ? talent.name : talent.talentName);
        book.ShowDetails(Name, talent.description);
    }

    public void OnClickEvent()
    {
        if (talent == null|| !SlotEnable) return;
        if (data.talentPoint < 1 || currentAmount >= talent.maxLevel) return;
        currentAmount++;
        data.talentPoint--;
        toRemove++;
        book.dataText.text = "(" + data.talentPoint + ")";
        text.text = "(" + currentAmount + ")";
        if (currentAmount >= talent.maxLevel)
        {
            UnlockRoads(true);
        }
    }

    public void Redraw()
    {
        text.text = "(" + currentAmount + ")";
        if(currentAmount<=0)
        {
            blackImage.enabled = true;
            UnlockRoads(false);
        }
    }
}
    
