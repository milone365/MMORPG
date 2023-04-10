using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
