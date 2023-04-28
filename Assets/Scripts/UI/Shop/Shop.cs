using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Shop : MonoBehaviour
{
    [SerializeField]
    ShopType type = ShopType.ItemShop;
    [SerializeField]
    ShopSlot slotPrefab = null;
    List<ShopSlot> slotList = new List<ShopSlot>();
    [SerializeField]
    Transform grid = null;
    public int SelectIndex = 0;
    List<Item> allItems = new List<Item>();
    List<Skill> allSkills = new List<Skill>();
    Merchant merchant;
    [SerializeField]
    Text descriptionText = null,coin=null;
    [SerializeField]
    Text amount = null,price=null;
    int itemNum = 1;
    [SerializeField]
    Button plus = null, minus = null;
    [SerializeField]
    MaterialSlot matSlot = null;
    [SerializeField]
    Transform matGrid = null;
    [SerializeField]
    GameObject MaterialPage = null;
    List<MaterialSlot> matSlotList = new List<MaterialSlot>();

    public void Init(ShopType type,Merchant merchant)
    {
        plus.onClick.AddListener(Plus);
        minus.onClick.AddListener(Minus);
        this.merchant = merchant;
        this.type = type;
        int max = 10; // object list
        List<Sprite> spriteList = null;
        switch (type)
        {
            case ShopType.ItemShop:
                allItems.AddRange(Resources.LoadAll<Item>("Items"));
                max = allItems.Count;
                break;
            case ShopType.SkillShop:
                allSkills.AddRange(Resources.LoadAll<Skill>("Skills"));
                max = allSkills.Count;
                plus.gameObject.SetActive(false);
                minus.gameObject.SetActive(false);
                break;
            case ShopType.CraftingShop:
                allItems.AddRange(Resources.LoadAll<Item>("Items").Where(x=>x.reciepe.Count>0));
                max = allItems.Count;
                MaterialPage.SetActive(true);
                plus.gameObject.SetActive(false);
                minus.gameObject.SetActive(false);
                break;
            case ShopType.SellShop:
                allItems = merchant.player.GetInventory().items;
                max = allItems.Count;
                plus.gameObject.SetActive(false);
                minus.gameObject.SetActive(false);
                break;
        }
        if(type==ShopType.SkillShop)
        {
            spriteList = GetSprites<Skill>(allSkills);
        }
        else
        {
            spriteList = GetSprites<Item>(allItems);
        }
        for(int i=0;i<max;i++)
        {
            int val = i;
            ShopSlot s = Instantiate(slotPrefab, grid);
            Sprite sprite = spriteList[i];
            s.Init(val,this,sprite);
            slotList.Add(s);
            if(type==ShopType.SkillShop)
            {
                if(allSkills[i].requireLevel > merchant.player.data.stat.Level)
                {
                    s.GetComponent<Button>().enabled = false;
                    s.inactiveImage.enabled = true;
                   
                }
                foreach (var skil in merchant.player.GetInventory().skills)
                {
                    if(skil.name==allSkills[i].name)
                    {
                        s.GetComponent<Button>().enabled = false;
                        s.inactiveImage.enabled = true;
                    }
                }
            }
        }
        Select(0);
    }

    List<Sprite> GetSprites<T>(List<T> list) where T : ScriptableObject
    {
        List<Sprite> newList = new List<Sprite>();

        foreach(var l in list)
        {
            Item item = l as Item;
            if(item!=null)
            {
                newList.Add(item.sprite);
            }
            else
            {
                Skill skill = l as Skill;
                if(skill!=null)
                {
                    newList.Add(skill.sprite);
                }
            }
        }

        return newList;
    }

    public void Select(int index)
    {
        if(type==ShopType.CraftingShop)
        {
            ShowCraft(index);
            return;
        }

        foreach(var s in slotList)
        {
            s.marker.enabled = false;
        }
        slotList[index].marker.enabled = true;
        SelectIndex = index;
        string description = "";
        string objectName = "";
        string cost = "Cost: ";
        if(type==ShopType.SkillShop)
        {
            Skill skill = allSkills[index];
            objectName = skill.name;
            description = skill.name + "\n" ;
            description += skill.desctiption + "\n";
            description += "Required Level: " + skill.requireLevel + "\n";
            description += "SpellPower: " + skill.spellPower +"\n";
            cost += skill.cost;
            
        }
        else
        {
            Item item = allItems[index];
            objectName = item.name;
            description = item.name +"\n";
            description += item.description + "\n";
            if(type==ShopType.SellShop)
            {
                cost += item.cost/2;
            }
            else
            {
                cost += item.cost;
            }

            Equip e = item as Equip;
            if(e!=null)
            {
                description += "Rare:" + e.rarety.ToString() + "\n";
                description += "\n";
                description += "Stamina:" + e.stamina+ "\n";
                description += "Strenght:" + e.strenght + "\n";
                description += "Intellect:" + e.intellect + "\n";
                description += "Agility:" + e.agility + "\n";
                description += "Armor:" + e.armor + "\n";
            }
        }

        price.text = cost;
        amount.text = objectName += " x 1"; 
        descriptionText.text = description;
        coin.text = "Gold : " + merchant.player.data.coin;
        itemNum = 1;
    }

    void ShowCraft(int index)
    {
        foreach(var s in slotList)
        {
            s.marker.enabled = false;
        }
        SelectIndex = index;
        foreach(var s in matSlotList)
        {
            Destroy(s.gameObject);
        }
        matSlotList.Clear();
        var item= allItems[index];
        slotList[index].marker.enabled = true;
        Inventory inv = merchant.player.GetInventory();
        foreach(var r in item.reciepe)
        {
            MaterialSlot s = Instantiate(matSlot,matGrid);
            s.gameObject.SetActive(true);
            s.SetUp(r.key.sprite, inv.GetItemCount(r.key.name) + "/" + r.value);
            matSlotList.Add(s);
        }
    }
    public enum ShopType
    {
        ItemShop, //buy
        SkillShop, //skill
        CraftingShop, // item
        SellShop, // sell object // playerItems
    }

    public void Close()
    {
        merchant.player.CanMove = true;
        Destroy(gameObject);
    }

    public void Trade()
    {
        switch (type)
        {
            case ShopType.ItemShop:
                BuyItem();
                break;
            case ShopType.SkillShop:
                BuySkill();
                break;
            case ShopType.CraftingShop:
                CreateObject();
                break;
            case ShopType.SellShop:
                Sell();
                break;
        }
    }

    public void Plus()
    {
        itemNum++;
        if(type==ShopType.SkillShop)
        {
            var sel = allSkills[SelectIndex];
            if(sel!=null)
            {
                amount.text = sel.name + " x " + itemNum;
            }
            else
            {
                amount.text = " x " + itemNum;
            }
        }
        else
        {
            var sel = allItems[SelectIndex];
            if (sel != null)
            {
                amount.text = sel.name + " x " + itemNum;
            }
            else
            {
                amount.text = " x " + itemNum;
            }
        }
        
    }
    public void Minus()
    {
        itemNum--;
        if (itemNum < 1) itemNum = 1;
        if (type == ShopType.SkillShop)
        {
            var sel = allSkills[SelectIndex];
            if (sel != null)
            {
                amount.text = sel.name + " x " + itemNum;
            }
            else
            {
                amount.text = " x " + itemNum;
            }
        }
        else
        {
            var sel = allItems[SelectIndex];
            if (sel != null)
            {
                amount.text = sel.name + " x " + itemNum;
            }
            else
            {
                amount.text = " x " + itemNum;
            }
        }
    }

    

    void BuySkill()
    {
           Skill skill = allSkills[SelectIndex];
           bool exist = false;
           foreach(var s in merchant.player.GetInventory().skills)
           {
                if(s.name==skill.name)
                {
                    exist = true;
                }
           }
           if(exist)
           {
                UIManager.instance.ShowBanner("You Can Buy Spell Only One Time");
                return;
           }
           if (skill.requireLevel>merchant.player.data.stat.Level)
            {
                UIManager.instance.ShowBanner("Your Level Is Too Low");
            }
            else
          {
            if (merchant.player.data.coin >= skill.cost)
            {
                merchant.player.data.coin -= skill.cost;
                coin.text = "Gold : " + merchant.player.data.coin;
                merchant.player.GetInventory().skills.Add(skill);
                slotList[SelectIndex].GetComponent<Button>().enabled = false;
                slotList[SelectIndex].inactiveImage.enabled = true;
            }
            else
            {
                UIManager.instance.ShowBanner("Don't Have Money");
            }
        }
    }
    void BuyItem()
    {
        Item item = allItems[SelectIndex];
        int price = item.cost * itemNum;
        if(merchant.player.data.coin>=price)
        {
            merchant.player.data.coin -= price;
            coin.text = "Gold : " + merchant.player.data.coin;
            for(int i=0;i<itemNum;i++)
            {
                merchant.player.GetInventory().AddToInventory(item);
            }
        }
        else
        {
            UIManager.instance.ShowBanner("Don't Have Money");
        }
    }

    void Sell()
    {
        if (allItems.Count < SelectIndex || allItems.Count<1) return;

        Item item = allItems[SelectIndex];
        int price = item.cost / 2;
        Player player = merchant.player;
        player.GetInventory().SellItem(price, item);
        foreach(var s in slotList)
        {
            Destroy(s.gameObject);
        }
        slotList.Clear();
        allItems = player.GetInventory().items;
        List<Sprite> spriteList = GetSprites<Item>(allItems);
        for (int i = 0; i < allItems.Count; i++)
        {
            int val = i;
            ShopSlot s = Instantiate(slotPrefab, grid);
            Sprite sprite = spriteList[i];
            s.Init(val, this, sprite);
            slotList.Add(s);
        }
        SelectIndex = 0;
        if(allItems.Count>0)
        {
            slotList[0].marker.enabled = true;
        }
        coin.text = "Gold : " + merchant.player.data.coin;
    }
    void CreateObject()
    {
        var item = allItems[SelectIndex];
        Inventory inv = merchant.player.GetInventory();
        bool canMakeItem = true;
        foreach (var r in item.reciepe)
        {
            if(r.value>inv.GetItemCount(r.key.name))
            {
                canMakeItem = false;
            }
        }
        if(canMakeItem)
        {
            inv.AddToInventory(item);
            foreach (var r in item.reciepe)
            {
                for(int i=0;i<r.value;i++)
                    inv.SellItem(0, r.key);
            }
        }

        Select(0);
    }
}
