using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, Interactable
{
    [SerializeField]
    List<Item> itemToGive = new List<Item>();
    [SerializeField]
    float minDistance = 5;
    [SerializeField]
    AnimName animName = AnimName.MiningLoop;
    Player player;
    public void Interact(Player p = null)
    {
        float distance = Vector3.Distance(p.transform.position, transform.position);
        if(distance>minDistance)
        {
            return;
        }
        if(itemToGive.Count<1)
        {
            Destroy(gameObject);
            return;
        }
        p.controller.Gather(animName.ToString());
        player = p;
        Invoke("CloseEvent", 1);
    }

    void CloseEvent()
    {
        player.GetInventory().AddToInventory(itemToGive[0]);
        itemToGive.RemoveAt(0);
        if (itemToGive.Count < 1)
        {
            Destroy(gameObject);
        }
    }
}
