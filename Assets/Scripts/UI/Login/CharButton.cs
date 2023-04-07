using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharButton : MonoBehaviour
{
    CharacterCreate create;
    int id = 0;
    public GameObject icon;
    public void Init(CharacterCreate crea,int id,string na)
    {
        create = crea;
        this.id = id;
        GetComponentInChildren<UnityEngine.UI.Text>().text = na;
    }
    public void Select()
    {
        create.Select(id);
    }

    public void DeleteCharacter()
    {
        create.DeleteCharacter(id);
    }
}
