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
    }
    void Update()
    {
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
        List<Skill> allSkills = controller.inventory.playerSkills; // inventory
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
}
