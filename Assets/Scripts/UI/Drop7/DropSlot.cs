using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour
{
    Item item;
    Player player;
    [SerializeField]
    Image icon = null;
    [SerializeField]
    Button button = null;
    DropPanel panel;
    public void SetUp(DropPanel panel,Item item,Player p)
    {
        this.panel = panel;
        this.item = item;
        this.player = p;
        icon.sprite = item.sprite;
        button.onClick.AddListener(TakeObject);
    }

    public void TakeObject()
    {
        panel.RemoveObject(gameObject);
        player.GetInventory().AddToInventory(item);
        Destroy(gameObject);
    }
}
