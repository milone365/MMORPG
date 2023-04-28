using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewItem",menuName ="ScriptableObject/Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public string description;
    public int cost = 0;
    public int dropRate = 75;

    [Header("Craft")]
    public List<Pair<Item, int>> reciepe = new List<Pair<Item, int>>();
   
}
