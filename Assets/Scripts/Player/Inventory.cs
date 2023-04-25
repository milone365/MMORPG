using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Inventory 
{
    public List<Pair<Skill, int>> equippedSkill = new List<Pair<Skill, int>>();
    public List<Skill> skills = new List<Skill>();
    public List<Item> items = new List<Item>();
    public Equip head, body, leg, shoes,belt,shoulder;
    public Equip leftWeapon, rightWeapon;
    public Equip lastEquip;
    public int coin = 0;
    List<Quest> allQuest = new List<Quest>();
    List<string> killRecord = new List<string>();
    public List<Equip> AllEquip()
    {
        List<Equip> equipList = new List<Equip>();
        equipList.Add(head);
        equipList.Add(body);
        equipList.Add(leg);
        equipList.Add(shoes);
        equipList.Add(belt);
        equipList.Add(shoulder);
        equipList.Add(leftWeapon);
        equipList.Add(rightWeapon);

        return equipList;
    }
    Player player;
    public int GetParameter(string s)
    {
        var equip = AllEquip().Where(x => x != null);
        int val = 0;
        foreach(var e in equip)
        {
            switch (s)
            {
                case StaticStrings.stamina:
                    val += e.stamina;
                    break;
                case StaticStrings.strenght:
                    val += e.strenght;
                    break;
                case StaticStrings.intellect:
                    val += e.intellect;
                    break;
                case StaticStrings.agility:
                    val += e.agility;
                    break;
                case StaticStrings.armor:
                    val += e.armor;
                    break;
            }
        }
        return val;
    }

    public void Init(Player p)
    {
        var allItems = Resources.LoadAll<Item>("");
        var allSkills = Resources.LoadAll<Skill>("");
        allQuest = Resources.LoadAll<Quest>("Quest").ToList();
        this.player = p;
        SaveData data = player.data;
        foreach (var d in data.equip)
        {
            var item = GetItem<Item>(allItems, d.key);
            if(item!=null)
            {
                SetEquip(item as Equip,d.value);
            }
        }
        foreach (var d in data.items)
        {
            var item = GetItem<Item>(allItems, d);
            if (item != null)
            {
                items.Add(item);
            }
        }
        foreach (var d in data.skills)
        {
            var skill = GetItem<Skill>(allSkills, d);
            if (skill != null)
            {
                skills.Add(skill);
            }
        }
        foreach (var d in data.equipSkills)
        {
            var skill = GetItem<Skill>(allSkills, d.key);
            if (skill != null)
            {
                var pair = new Pair<Skill, int>() { key = skill, value = d.value };
                equippedSkill.Add(pair);
            }
        }
    }

    public void SetEquip(Equip equip,int id=0)
    {
        //if (equip == null) return;
        switch (equip.type)
        {
            case EquipType.head:
                lastEquip = head;
                head = equip;
                break;
            case EquipType.body:
                lastEquip = body;
                body = equip;
                break;
            case EquipType.leg:
                lastEquip = leg;
                leg = equip;
                break;
            case EquipType.shoes:
                lastEquip = shoes;
                shoes = equip;
                break;
            case EquipType.belt:
                lastEquip = belt;
                belt = equip;
                break;
            case EquipType.shoulder:
                lastEquip = shoulder;
                shoulder = equip;
                break;
            case EquipType.weapon:
                if(id<7)
                {
                    lastEquip = leftWeapon;
                    leftWeapon = equip;
                    player.ChangeWeapon(leftWeapon, true);
                }
                else
                {
                    lastEquip = rightWeapon;
                    rightWeapon = equip;
                    player.ChangeWeapon(rightWeapon, false);
                }
                break;
        }
        if(lastEquip!=null)
        {
            items.Add(lastEquip);
            lastEquip = null;
        }
        player.OnChangeItem();
    }

    
    public void RemoveItem(Equip e)
    {
        foreach(var i in items)
        {
            if(i.name==e.name)
            {
                items.Remove(i);
                break;
            }
        }
    }
    T GetItem<T>(T[] arr,string s) where T: ScriptableObject
    {
        foreach(var a in arr)
        {
            if(a.name==s)
            {
                return a;
            }
        }
        return null;
    }

    public void UpdateSkill(ActionController controller)
    {
        equippedSkill.Clear();
        for (int i=0;i<controller.actions.Count;i++)
        {
            var skill = controller.actions[i].skill;
            if (skill!=null)
            {
                var pair = new Pair<Skill, int>() { key = skill, value = i };
                equippedSkill.Add(pair);
            }
        }
    }

    public void AddToInventory(Item item)
    {
        items.Add(item);
        QuestCheck(item.name);
    }

    void QuestCheck(string ItemName,bool isEnemy=false)
    {
        QuestData data = null;
        foreach(var q in player.data.activeQuestList)
        {
            foreach(var r in q.requiredObjetList)
            {
                if (r.objectName == ItemName)
                {
                    data = q;
                    break;
                }
            }   
        }
        if(data==null)
        {
            if(!isEnemy)
                UIManager.instance.ShowDropBanner("Recieve " + ItemName, 1.5f);
        }
        else
        {
            Quest q = Helper.GetQuest(allQuest, data);
            if(q!=null)
            {
                bool questCompleted = true;
                foreach (var r in data.requiredObjetList)
                {
                    if (r.objectName == ItemName)
                    {
                        r.currentAmount++;
                        UIManager.instance.ShowDropBanner(ItemName + " " + r.currentAmount + " / " + r.requiredAmount, 2.5f);
                    }
                    if (r.currentAmount < r.requiredAmount)
                    {
                        questCompleted = false;
                    }
                }
                if(questCompleted)
                {
                    data.questCompleted = true;
                }
               
                
            }
        }
    }
    public void KillRecord(string killed)
    {
        killRecord.Add(killed);
        QuestCheck(killed,true);
    }
}


[System.Serializable]
public class Pair<T1,T2>
{
    public T1 key;
    public T2 value;
}