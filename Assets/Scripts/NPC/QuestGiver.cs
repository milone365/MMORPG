using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour,Interactable
{
    [SerializeField]
    Quest quest=null;
    [SerializeField]
    QuestPopUp prefab = null;
    QuestPopUp popUp;
    public void Interact(Player p = null)
    {
        if(popUp==null)
        {
            QuestData currentData = Helper.GetQuestData(p.data.completedQuestList, quest.data);
            if (currentData != null)
            {
                
                return;
            }
            popUp = Instantiate(prefab);
            popUp.Initialize(quest, this, p);
        }
    }

}

