using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TalentBook : MonoBehaviour
{
    [SerializeField]
    TalentSlots[] slots = null;
    [SerializeField]
    UnityEngine.UI.Text talentName=null, talentDescription=null;
    public UnityEngine.UI.Text dataText=null;
    SaveData data;
    Player player;
    public void Intit(Player player)
    {
        this.player = player;
        this.data = player.data;
        slots = GetComponentsInChildren<TalentSlots>();
        CharacterClass charClass = data.stat.charClass;
        var arr = Resources.LoadAll<Talent>("Talents").Where(x => x.charClass == charClass).ToArray();
        if (arr.Length < 1) return;
        if (data.talentList.Count < 1)
        {
            foreach(var a in arr)
            {
                data.talentList.Add(new Pair<string, int>() { key = a.name, value = 0 });
            }
            UIManager.instance.SaveGame();
        }
        for (int i=0;i<slots.Length;i++)
        {
            if (arr.Length-1 < i) continue;
            slots[i].Initialize(this,arr[i], data);
        }
    }

    public void Close()
    {
        for(int i=0;i<slots.Length;i++)
        {
            for(int j=0;j<data.talentList.Count;j++)
            {
                if(data.talentList[j].key==slots[i].talent.name)
                {
                    data.talentList[j].value = slots[i].currentAmount;
                }
            }
        }
        player.UpdateTalentList();
        player.OnChangeItem();
        Destroy(gameObject);
    }
    
    public void ShowDetails(string name,string description)
    {
        talentName.text = name;
        talentDescription.text = description;
    }

    public void ResePoints()
    {
        foreach(var s in slots)
        {
            data.talentPoint += s.toRemove;
            s.currentAmount -= s.toRemove;
            s.toRemove = 0;
            s.Redraw();
        }
        dataText.text = "(" + data.talentPoint + ")";
    }
}
