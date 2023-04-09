using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterStatsUI : MonoBehaviour
{
    public Text Name = null, Hp = null, Lv = null;
    [SerializeField]
    Text Atk = null, Def = null,Class=null;
    [SerializeField]
    GameObject[] slots = null;

    public void SetUp(SaveData data)
    {
        Stats stat = data.stat;
        Name.text = "Name: " + data.characterName;
        SetUp(stat);
        foreach (var i in slots)
        {
            i.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
        if(data.equip.Count>0)
        {
            for(int i=0;i<data.equip.Count;i++)
            {
                Equip e = Resources.Load<Equip>("Items/"+ data.equip[i]);
                if(e!=null)
                    SetEquip(e.sprite, i);
            }
        }
    }
    public void SetUp(Stats stat)
    {
        Hp.text = "Stamina: " + stat.Stamina;
        Lv.text = "Lv: " + stat.Level;
        Atk.text = "Strenght: " + stat.Strenght;
        Def.text = "Agility: " + stat.Agility;
        Class.text = "Class: " + stat.charClass.ToString();
    }

    void SetEquip(Sprite s,int index)
    {
        var img = slots[index].transform.GetChild(0).GetComponent<Image>();
        img.sprite = s;
        img.enabled = true;
    }
}
