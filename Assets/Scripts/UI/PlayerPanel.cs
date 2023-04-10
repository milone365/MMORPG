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

    [Header("Current Item")]
    [SerializeField]
    Text itemStamina = null;
    [SerializeField]
    Text itemStrenght = null, itemIntellect = null;
    [SerializeField]
    Text itemName=null, itemAgility=null, itemArmor = null;

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
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
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
        Stats s = player.stats();
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
        int localStam = 0, localStrenght = 0, localAgility = 0, localIntellect = 0, localArmor = 0;

        if(oldEquip!=null)
        {
            localStam = newEquip.stamina - oldEquip.stamina;
            localStrenght = newEquip.strenght - oldEquip.strenght;
            localAgility = newEquip.agility - oldEquip.agility;
            localIntellect = newEquip.intellect - oldEquip.intellect;
            localArmor = newEquip.armor - oldEquip.armor;
            writer.ShowDiffence(localStam, StaticStrings.stamina);
            writer.ShowDiffence(localStrenght, StaticStrings.strenght);
            writer.ShowDiffence(localAgility, StaticStrings.agility);
            writer.ShowDiffence(localIntellect, StaticStrings.intellect);
            writer.ShowDiffence(localArmor, StaticStrings.armor);

            writer.ShowDiffence(player.hpMultipler * localStam, StaticStrings.hp);
            writer.ShowDiffence(player.manaMultipler * localIntellect, StaticStrings.mana);
        }
        else
        {
            writer.ShowDiffence(newEquip.stamina, StaticStrings.stamina);
            writer.ShowDiffence(newEquip.strenght, StaticStrings.strenght);
            writer.ShowDiffence(newEquip.agility, StaticStrings.agility);
            writer.ShowDiffence(newEquip.intellect, StaticStrings.intellect);
            writer.ShowDiffence(newEquip.armor, StaticStrings.armor);
            
            writer.ShowDiffence(player.hpMultipler* newEquip.stamina, StaticStrings.hp);
            writer.ShowDiffence(player.manaMultipler* newEquip.intellect, StaticStrings.mana);
        }

        ShowCurrentItem(newEquip);
    }

    void ShowCurrentItem(Equip e)
    {
        itemName.text = e.name;
        itemStamina.text = "Stamina: " + e.stamina;
        itemStrenght.text = "Strenght: " + e.strenght;
        itemAgility.text = "Agility: " + e.agility;
        itemIntellect.text = "intellect: " + e.intellect;
        itemArmor.text = "Armor: " + e.armor;
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
    
    public void ClosePanel()
    {
        player.CanMove = true;
        Destroy(gameObject);
    }
}
