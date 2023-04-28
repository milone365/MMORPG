using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopSlot : MonoBehaviour
{
    public Image icon=null, marker=null, inactiveImage=null;
    public int ID = 0;
    Shop shop=null;
    public  void Init(int val,Shop shop,Sprite sprite)
    {
        this.shop = shop;
        ID = val;
        GetComponent<Button>().onClick.AddListener(Select);
        icon.sprite = sprite;
    }

    public void Select()
    {
        shop.Select(ID);
    }
}
