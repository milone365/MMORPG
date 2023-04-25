using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameObject deathPanel = null;
    public Player player { get; set; }
    public static UIManager instance;
    public bool uiIsOpen = false;
    public Chat chat;
    public List<ActionButton> buttons = new List<ActionButton>();
    [SerializeField]
    GameObject spellBook = null;
    [SerializeField]
    SkillSlot skillPrefab = null;
    [SerializeField]
    Transform content = null;
    List<GameObject> toDeleteSkills = new List<GameObject>();
    ActionController controller;
    bool initialized = false;
    [SerializeField]
    DisableOverTime banner = null;
    [SerializeField]
    PlayerPanel playerPanelPrefab = null;
    PlayerPanel playerPanel;
    [SerializeField]
    Slider hpBar = null, manaBar = null;
    [SerializeField]
    Text playerName = null, level = null;
    //photo
    [SerializeField]
    PopUpBase popUpBase = null;
    GameObject currentPopUp;
    [SerializeField]
    BuffSlot buffSlot = null;
    [SerializeField]
    Transform grid = null;
    [SerializeField]
    TalentBook talentBook = null;
    [SerializeField]
    Text manaText = null, hpText = null;
    [SerializeField]
    DisableOverTime drop_banner = null;
    [SerializeField]
    DropPanel dropPrefab = null;
    DropPanel dropPanel { get; set;}


    private void Awake()
    {
        instance = this;
    }
    
    public void SetActions(ActionController controller)
    {
        this.controller = controller;
        for(int i=0;i<buttons.Count;i++)
        {
            if(i<controller.actions.Count)
            {
                buttons[i].SetUpButton(controller, controller.actions[i]);
                controller.actions[i].button = buttons[i];
            }
        }
        initialized = true;
    }
    void Update()
    {
        if (!initialized) return;
        foreach(var item in buttons)
        {
            item.FadeCheck();
        }
    }

    public void Respawn()
    {
        deathPanel.SetActive(false);
        player.Respawn();
    }

    public void SpellBook()
    {
        spellBook.SetActive(true);
        List<Skill> allSkills = controller.inventory.skills; // inventory
        foreach(var s in allSkills)
        {
            SkillSlot slot = Instantiate(skillPrefab, content);
            slot.Init(s);
            toDeleteSkills.Add(slot.gameObject);
        }
    }
    public void CloseBook()
    {
        foreach(var item in toDeleteSkills)
        {
            Destroy(item);
        }
        toDeleteSkills.Clear();
        spellBook.SetActive(false);
    }

    public void SaveGame()
    {
        var inventoy = controller.inventory;
        var data = player.data;
        var level = SceneManager.GetActiveScene();
        data.currentLevel = level.buildIndex;
        data.LevelName = level.name;
        data.x = player.transform.position.x;
        data.y = player.transform.position.y;
        data.z = player.transform.position.z;
        data.stat = player.stats();
        data.equip.Clear();
        int index = 0;
        foreach(var item in inventoy.AllEquip())
        {
            if(item!=null)
            {
                var pair = new Pair<string, int>() { key = item.name, value = index };
                data.equip.Add(pair);
            }
            index++;
        }
        RecordStrings<Item>(ref data.items, inventoy.items);
        RecordStrings<Skill>(ref data.skills, inventoy.skills);
        data.equipSkills.Clear();
        inventoy.UpdateSkill(controller);
        foreach(var eSkill in inventoy.equippedSkill)
        {
            if(eSkill.key!=null)
            {
                data.equipSkills.Add(new Pair<string, int>() { key=eSkill.key.name,value=eSkill.value});
            }
        }
        SaveManager.SaveData<SaveData>(data.characterName,data);
        ShowBanner();
    }

    public void ShowBanner(string message="Game Saved",float lifetime=1)
    {
        banner.gameObject.SetActive(true);
        banner.Init(message, lifetime);
    }

    public void ShowDropBanner(string message,float lifetime)
    {
        drop_banner.gameObject.SetActive(true);
        drop_banner.Init(message, lifetime);
    }
    void RecordStrings<T>(ref List<string>list,List<T>template)where T:ScriptableObject
    {
        list.Clear();
        foreach(var item in template)
        {
            if(item!=null)
                list.Add(item.name);
        }
    }

    public void ShowPlayerPanel()
    {
        if(playerPanel==null)
        {
            playerPanel = Instantiate(playerPanelPrefab);
            playerPanel.Init(player,controller.inventory);
            player.LockPlayer();
        }
        else
        {
            Destroy(playerPanel.gameObject);
            player.CanMove = true;
        }
    }

    public void OpeDropList(Enemy enemy, Player player)
    {
        if(dropPanel==null)
        {
            dropPanel = Instantiate(dropPrefab);
            dropPanel.Init(enemy, player);
        }
    }

    public void SetUpPlayer(Player player)
    {
        this.player = player;
        UpdateHP(player.maxHp, player.maxHp);
        UpdateMana(player.maxMana, player.maxMana);
        playerName.text = player.data.characterName;
        level.text = "LV: " + player.data.stat.Level;
    }

    public void UpdateHP(int current, int max)
    {
        hpBar.maxValue = max;
        hpBar.value = current;
        hpText.text = current + " / " + max;
    }
    public void UpdateMana(int current, int max)
    {
        manaBar.maxValue = max;
        manaBar.value = current;
        manaText.text = current + " / " + max;
    }

    public void ShowResurrectionRequest()
    {
        var popup = Instantiate(popUpBase, deathPanel.transform);
        currentPopUp = popup.gameObject;
        popup.Init("You Recive Resurrection Request.", 
            delegate {
                player.Respawn(true);
                deathPanel.SetActive(false);
                Destroy(currentPopUp);
            }, 
            delegate { Destroy(currentPopUp);});
    }

    public void GenerateSlot(float lifeTime,string spriteName)
    {
        var slot = Instantiate(buffSlot, grid);
        slot.Init(WorldManager.instance.GetSprite(spriteName),lifeTime);
    }

    public void GenerateTalentBook()
    {
        var b = Instantiate(talentBook);
        b.Intit(player);
    }
}
