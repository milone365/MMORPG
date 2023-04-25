using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopUp : MonoBehaviour
{
    Quest quest;
    QuestGiver giver;
    Player player;
    [SerializeField]
    Text title = null, coinText = null, expText = null, description = null;
    [SerializeField]
    GameObject slot = null;
    [SerializeField]
    Button confirmButton = null,cancelButton=null,completeButton=null;
    [SerializeField]
    GameObject requiredSlot = null;
    [SerializeField]
    Transform parent = null;

    public void Initialize(Quest quest,QuestGiver giver,Player player)
    {
        this.quest=quest;
        this.giver=giver;
        this.player=player;
        title.text = quest.data.questName;
        coinText.text = "coin : " + quest.coin;
        expText.text = "exp: " + quest.expPoints;
        description.text = quest.description;
        CreateBox();
        QuestData currentData = Helper.GetQuestData(player.data.activeQuestList,quest.data);
        if(currentData!=null)
        {
            confirmButton.gameObject.SetActive(false);
            if(currentData.questCompleted==true)
            {
                cancelButton.gameObject.SetActive(false);
                completeButton.gameObject.SetActive(true);
            }
        }
    }

    void CreateBox()
    {
        description.text += "\n";
        description.text += "\n";
        description.text += "Bring Me: " + "\n";
        description.text += "\n";

        foreach (var r in quest.data.requiredObjetList)
        {
            GameObject g = Instantiate(requiredSlot, parent);
            g.SetActive(true);
            g.GetComponentInChildren<Text>().text = r.currentAmount + "/" + r.requiredAmount;
            if(r.objectSprite!=null)
            g.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = r.objectSprite; 
            description.text += "*" + r.objectName + " x " + r.requiredAmount;
            description.text += "\n";
        }
    }
    public void ClosePopUp()
    {
        player.CanMove = true;
        Destroy(gameObject);
    }
    public void Confirm()
    {
        QuestData dat = quest.data;
        dat.questCompleted = false;
        foreach(var r in dat.requiredObjetList)
        {
            r.currentAmount = 0;
        }
        player.data.activeQuestList.Add(dat);
        UIManager.instance.ShowBanner("Quest Accepted: " + quest.data.questName);
        ClosePopUp();
    }

    public void CompleteQuest()
    {
        QuestData toRemove = null;
        foreach(var item in player.data.activeQuestList)
        {
            if(item.questName==quest.data.questName)
            {
                toRemove= item;
                break;
            }
        }
        player.AddExperience(quest.expPoints);
        player.data.activeQuestList.Remove(toRemove);
        player.data.completedQuestList.Add(quest.data);
        UIManager.instance.ShowBanner("Quest Completed: " + quest.data.questName);
        ClosePopUp();
    }
}
