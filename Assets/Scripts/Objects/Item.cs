using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewItem",menuName ="ScriptableObjects/Items")]
public class Item : ScriptableObject
{
    public Sprite spirte;
    public string description;
    public int cost = 0;
}
