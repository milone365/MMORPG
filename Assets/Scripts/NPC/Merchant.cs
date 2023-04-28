using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour, Interactable
{
    [SerializeField]
    Shop shopPrefab = null;
    Shop shop;
    [SerializeField]
    Shop.ShopType type = Shop.ShopType.ItemShop;
    public Player player { get; set; }
    public void Interact(Player p = null)
    {
        if(shop==null)
        {
            player = p;
            p.LockPlayer();
            shop = Instantiate(shopPrefab);
            shop.Init(type,this);
        }
    }
}
