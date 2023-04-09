using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObject/Equip")]
public class Equip : Item
{
    public EquipType type = EquipType.head;
    public Rarety rarety=Rarety.comune;
    public int stamina, strenght, intellect, agility;
    public int armor;
}

public enum Rarety
{
    comune,
    good,
    rare,
    epic,
    legendary
}
public enum EquipType
{
    head, body, leg, shoes, belt, shoulder, weapon
}