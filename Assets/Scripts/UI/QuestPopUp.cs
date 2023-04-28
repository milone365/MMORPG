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
        player.LockPlayer();
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
        else
        {
            bool completed = true;
            Inventory inventory = player.GetInventory();
            foreach(var r in quest.data.requiredObjetList)
            {
                switch (quest.type)
                {
                    case QuestType.Kill:
                        if (inventory.killRecord.Count < 1) completed = false;
                        if(inventory.GetKilledCount(r.objectName)<r.currentAmount)
                        {
                            completed = false;
                            break;
                        }
                        break;
                    case QuestType.Find:
                        if (inventory.items.Count < 1) completed = false;
                        if (inventory.GetItemCount(r.objectName) < r.currentAmount)
                        {
                            completed = false;
                            break;
                        }
                        break;
                }
            }

            if(completed)
            {
                confirmButton.gameObject.SetActive(false);
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
            int currentAmount = 0;
            switch (quest.type)
            {
                case QuestType.Kill:
                    currentAmount= player.GetInventory().GetKilledCount(r.objectName);
                    break;
                case QuestType.Find:
                    currentAmount = player.GetInventory().GetItemCount(r.objectName);
                    break;
            }
            g.GetComponentInChildren<Text>().text = currentAmount + "/" + r.requiredAmount;
            if(r.objectSprite!=null)
            g.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = r.objectSprite; 
            description.text += "*" + r.objectName + " x " + r.requiredAmount;
            description.text += "\n";
        }
        if(quest.itemToGive!=null)
        {
            slot.SetActive(true);
            slot.transform.GetChild(0).GetComponent<Image>().sprite = quest.itemToGive.sprite;
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
        RemoveQuestObjects();
        player.data.coin = quest.coin;
        if(quest.itemToGive!=null)
        {
            player.GetInventory().AddToInventory(quest.itemToGive);
        }
        foreach (var item in player.data.activeQuestList)
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

    void RemoveQuestObjects()
    {
        if(quest.type==QuestType.Find)
        {
            foreach(var r in quest.data.requiredObjetList)
            {
                player.GetInventory().RemoveAllItemByName(r.objectName);
            }
        }
    }
}
