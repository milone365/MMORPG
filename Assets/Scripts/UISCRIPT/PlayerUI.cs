using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Chat chat;
    [SerializeField]
    List<ButtonAction> upButtons = new List<ButtonAction>();
    [SerializeField]
    List<ButtonAction> downButtons = new List<ButtonAction>();
    ActionManager actionManager;
    NetworkPlayer player;
    public GameObject deathPanel = null;
    public void INIT(NetworkPlayer player,ActionManager actionManager)
    {
        this.player = player;
        chat.Init(player);
        this.actionManager = actionManager;
        var up = actionManager.upActions;
        var down = actionManager.downActions;
        for(int i=0;i<upButtons.Count;i++)
        {
            if (i > up.Count - 1) continue;
            upButtons[i].INIT(up[i],actionManager);
        }
        for (int i = 0; i < downButtons.Count; i++)
        {
            if (i > down.Count - 1) continue;
            downButtons[i].INIT(down[i], actionManager);
        }
    }

    public void GoToCemetery()
    {
        player.Respawn(WorldController.instance.cemetery.position);
        deathPanel.SetActive(false);
    }
}
