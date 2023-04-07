using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory 
{
    public List<Skill> playerSkills = new List<Skill>();
    public Equip head, body, leg, shoes,belt,shoulder;
    public Equip leftWeapon, rightWeapon;
    public void Init()
    {
        
    }

}
