using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiffentWriter : MonoBehaviour
{
    [SerializeField]
    Text hpText = null, staminaText = null, strenghtText = null;
    [SerializeField]
    Text manaText = null, agilityText = null, intellectText = null;
    [SerializeField]
    Text armorText = null;
    Color red = Color.red;
    Color green = Color.green;

    public  void ShowDiffence(int val,string selected)
    {
        Text current=null;
        switch(selected)
        {
            case StaticStrings.agility:
                current = agilityText;
                break;
            case StaticStrings.armor:
                current = armorText;
                break;
            case StaticStrings.strenght:
                current = strenghtText;
                break;
            case StaticStrings.intellect:
                current = intellectText;
                break;
            case StaticStrings.hp:
                current = hpText;
                break;
            case StaticStrings.mana:
                current = manaText;
                break;
            case StaticStrings.stamina:
                current = staminaText;
                break;
        }
        if(val!=0)
        {
            if(val>0)
            {
                current.text = " + " + val;
                current.color = green;
            }
            else
            {
                current.text =  val.ToString();
                current.color = red;
            }
        }
        else
        {
            current.text = "";
        }
    }
}
