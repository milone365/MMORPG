using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject deathPanel = null;
    public Player player { get; set; }

    public static UIManager instance;

    public Chat chat;
    public List<ActionButton> buttons = new List<ActionButton>();

    private void Awake()
    {
        instance = this;
    }
    
    public void SetActions(ActionController controller)
    {
        for(int i=0;i<buttons.Count;i++)
        {
            if(i<controller.actions.Count)
            {
                buttons[i].SetUpButton(controller, controller.actions[i].key);
            }
        }
    }
    void Update()
    {
        
    }

    public void Respawn()
    {
        deathPanel.SetActive(false);
        player.Respawn();
    }
}
