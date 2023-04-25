using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPanel : MonoBehaviour
{
    [SerializeField]
    DropSlot slot = null;
    [SerializeField]
    Transform parent = null;
    List<GameObject> dropList = new List<GameObject>();
    public void Init(Enemy enemy, Player player)
    {
        enemy.CanTakeDrop = false;
        foreach(var i in enemy.dropList)
        {
            int dice = Random.Range(1, 101);
            if (dice > i.dropRate) continue;
            DropSlot newSlot = Instantiate(slot, parent);
            newSlot.SetUp(this,i,player);
            dropList.Add(newSlot.gameObject);
        }
    }

    public void RemoveObject(GameObject g)
    {
        dropList.Remove(g);
        if(dropList.Count<1)
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        Destroy(gameObject);
    }
}
