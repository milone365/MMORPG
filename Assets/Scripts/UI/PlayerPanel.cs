using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class PlayerPanel : MonoBehaviour
{
    [SerializeField]
    Button[] buttonList = null;
    [SerializeField]
    EquipSlot slot = null;
    List<GameObject> currentSlots = new List<GameObject>();
    public Inventory inventory;
    [SerializeField]
    Transform content = null;
    string[] buttonTypes = new string[]
    {
        "head", "body", "leg", "shoes", "belt", "shoulder","weapon"
    };

    [SerializeField]
    Text hpText = null, staminaText = null, strenghtText = null;
    [SerializeField]
    Text manaText = null, agilityText = null, intellectText = null;
    [SerializeField]
    Text armorText = null, coinText = null;
    Player player;
    [SerializeField]
    DiffentWriter writer=null;

    public void Init(Player p,Inventory inventory)
    {
        player = p;
        this.inventory = inventory;
        for(int i=0;i<buttonList.Length;i++)
        {
            buttonList[i].gameObject.name= buttonTypes[i];
            buttonList[i].GetComponentInChildren<Text>().text = buttonTypes[i];
            int id = i;
            buttonList[i].onClick.AddListener(delegate
            {
                OnPressButton(id);
            }
            );
        }
        ShowStats();
    }

    public void OnPressButton(int id)
    {
        EquipType t = EquipType.head;
        switch(id)
        {
            case 1:
                t = EquipType.body;
                break;
            case 2:
                t = EquipType.leg;
                break;
            case 3:
                t = EquipType.shoes;
                break;
            case 4:
                t = EquipType.belt;
                break;
            case 5:
                t = EquipType.shoulder;
                break;
            case 6:
                t = EquipType.weapon;
                break;
        }
        ShowSlots(t);
    }

    public void ShowSlots(EquipType t)
    {
        foreach(var obj in currentSlots)
        {
            Destroy(obj);
        }
        currentSlots.Clear();
        List<Equip> list = new List<Equip>();
        foreach(var item in inventory.items)
        {
            if(item is Equip)
            {
                list.Add(item as Equip);
            }
        }
        var arr = list.Where(x=>x.type==t).ToArray();
        foreach(var element in arr)
        {
            EquipSlot newSlot = Instantiate(slot,content);
            currentSlots.Add(newSlot.gameObject);
            newSlot.Init(element,this);
        }
        ShowStats();
    }

    public void ShowStats()
    {
        Stats s = player.stats;
        int stamina, strenght, intellect, agility, armor;
        stamina = GetVal(inventory.GetParameter(StaticStrings.stamina), s.Stamina);
        strenght = GetVal(inventory.GetParameter(StaticStrings.strenght), s.Strenght);
        intellect= GetVal(inventory.GetParameter(StaticStrings.intellect), s.Intellect);
        agility = GetVal(inventory.GetParameter(StaticStrings.agility), s.Agility);
        armor = GetVal(inventory.GetParameter(StaticStrings.armor));

        staminaText.text = stamina.ToString();
        strenghtText.text = strenght.ToString();
        intellectText.text = intellect.ToString();
        agilityText.text = agility.ToString();
        armorText.text = armor.ToString();

        //hp
        hpText.text = player.maxHp.ToString();
        manaText.text = player.maxMana.ToString();
               
    }

    public void ShowDifference(Equip newEquip)
    {
        Equip oldEquip = null;
        foreach (var item in inventory.AllEquip())
        {
            if (item == null) continue;
            if(item.type==newEquip.type)
            {
                oldEquip = item;
                break;
            }
        }
        if(oldEquip!=null)
        {

        }
        else
        {
            writer.ShowDiffence(newEquip.stamina, StaticStrings.stamina);
            writer.ShowDiffence(newEquip.strenght, StaticStrings.strenght);
            writer.ShowDiffence(newEquip.agility, StaticStrings.agility);
            writer.ShowDiffence(newEquip.intellect, StaticStrings.intellect);
            writer.ShowDiffence(newEquip.armor, StaticStrings.armor);
            writer.ShowDiffence(newEquip.stamina*player.hpMultipler, StaticStrings.hp);
            writer.ShowDiffence(newEquip.intellect * player.manaMultipler, StaticStrings.mana);
        }
    }

    int GetVal(params int[] values)
    {
        var total = 0;
        foreach(var v in values)
        {
            total += v;
        }
        return total;
    }
}
