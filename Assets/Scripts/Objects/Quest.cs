using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObject/Quest")]
public class Quest : ScriptableObject
{
    public QuestData data=new QuestData();
    public int requiredAmount = 1;
    public int coin = 1;
    public int expPoints = 100;
    public Item itemToGive = null;
    public string description = "";
    public QuestType type = QuestType.Kill;

}

[System.Serializable]
public class QuestData
{
    public string questName = "";
    public string requiredObjet = "key";
    public int currentAmount = 0;
    public bool questCompleted = false;
}

public enum QuestType
{
    Kill,
    Find
}