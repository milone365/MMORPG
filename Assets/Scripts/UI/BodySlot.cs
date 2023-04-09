using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BodySlot : MonoBehaviour
{
    [SerializeField]
    Image border = null, icon = null;
    [SerializeField]
    EquipType type = EquipType.head;
    [SerializeField]
    int id = 0;
    public bool SetUp(Equip e,Inventory inventory)
    {
        if (e.type != type) return false;
        inventory.SetEquip(e, id);
        icon.sprite = e.sprite;
        border.color = Helper.GetColor(e.rarety);
        return true;
    }
}
