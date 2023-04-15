using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class CharacterCreate : MonoBehaviour
{
    const string location = "Assets/Resources/Data";
    public List<SaveData> allData = new List<SaveData>();
    [SerializeField]
    CharButton charButton = null;
    [SerializeField]
    Transform charParent = null;
    public static SaveData selectedData;
    List<CharButton> buttons = new List<CharButton>();
    [SerializeField]
    CharacterStatsUI stats = null;
    [SerializeField]
    GameObject creteCharacterPanel=null;
    [SerializeField]
    InputField field = null;
    [SerializeField]
    Dropdown dropDown = null;
    [SerializeField]
    GameObject startButton = null, createButton = null;
    [SerializeField]
    CharacterClass currentClass = CharacterClass.warrior;
    [SerializeField]
    int MaxPlayers = 3;
    string[] files;
    private void Start()
    {
        dropDown.onValueChanged.AddListener(SelectCharacter);
        selectedData = null;
        field.onSubmit.AddListener(delegate
        {
            selectedData.characterName = field.text;
            stats.SetUp(selectedData);
        }
        );
        //
        if(!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
            startButton.SetActive(false);
            selectedData = new SaveData();
            selectedData.stat = GetStat(CharacterClass.warrior);
            selectedData.characterName = "Guest";
            stats.SetUp(selectedData);
            return;
        }
        //
        files = Directory.GetFiles(location).Where(x=>!x.Contains(".meta")).ToArray();
        if(files.Length<1)
        {
            startButton.SetActive(false);
            selectedData = new SaveData();
            selectedData.stat = GetStat(CharacterClass.warrior);
            selectedData.characterName = "Guest";
        }
        else
        {
            int id = 0;
            foreach(var f in files)
            {
                string name = Path.GetFileName(f);
                SaveData data = SaveManager.LoadData<SaveData>(name);
                allData.Add(data);
                var button= Instantiate(charButton, charParent);
                button.Init(this, id,name);
                id++;
                buttons.Add(button);
            }
            Select(0);
        }
        if(files.Length>=MaxPlayers)
        {
            createButton.SetActive(false);
        }
    }
    
    public void Select(int ID)
    {
        foreach(var b in buttons)
        {
            b.icon.SetActive(false);
        }
        selectedData = allData[ID];
        buttons[ID].icon.SetActive(true);
        stats.SetUp(selectedData);
    }

    public void CreateCharacter()
    {
        createButton.SetActive(false);
        creteCharacterPanel.SetActive(true);
        startButton.SetActive(true);
        stats.SetUp(selectedData);
    }

    public static Stats GetStat(CharacterClass cla)
    {
        Stats newStat = new Stats();
        switch (cla)
        {
            case CharacterClass.warrior:
                newStat.Agility = 10;
                newStat.Strenght = 17;
                newStat.Intellect = 8;
                newStat.Stamina = 11;
                newStat.charClass = CharacterClass.warrior;
                break;
            case CharacterClass.mage:
                newStat.Agility = 10;
                newStat.Strenght = 8;
                newStat.Intellect = 12;
                newStat.Stamina = 11;
                newStat.charClass = CharacterClass.mage;
                break;
            case CharacterClass.priest:
                newStat.Agility = 12;
                newStat.Strenght = 10;
                newStat.Intellect = 12;
                newStat.Stamina = 11;
                newStat.charClass = CharacterClass.priest;
                break;
            case CharacterClass.paladin:
                newStat.Agility = 5;
                newStat.Strenght = 17;
                newStat.Intellect = 12;
                newStat.Stamina = 11;
                newStat.charClass = CharacterClass.paladin;
                break;
            case CharacterClass.shaman:
                newStat.Agility = 15;
                newStat.Strenght = 7;
                newStat.Intellect = 12;
                newStat.Stamina = 11;
                newStat.charClass = CharacterClass.shaman;
                break;
            case CharacterClass.druid:
                newStat.Agility = 15;
                newStat.Strenght = 7;
                newStat.Intellect = 12;
                newStat.Stamina = 11;
                newStat.charClass = CharacterClass.druid;
                break;
            case CharacterClass.rogue:
                newStat.Agility = 15;
                newStat.Strenght = 14;
                newStat.Intellect = 8;
                newStat.Stamina = 11;
                newStat.charClass = CharacterClass.rogue;
                break;
            case CharacterClass.ranger:
                newStat.Agility = 15;
                newStat.Strenght = 10;
                newStat.Intellect = 10;
                newStat.Stamina = 11;
                newStat.charClass = CharacterClass.ranger;
                break;
            case CharacterClass.warlock:
                newStat.Agility = 12;
                newStat.Strenght = 6;
                newStat.Intellect = 12;
                newStat.Stamina = 11;
                newStat.charClass = CharacterClass.warlock;
                break;
        }
        return newStat;
    }

    public void SelectCharacter(int val)
    {
        currentClass = (CharacterClass)val;
        Stats stat = GetStat(currentClass);
        stats.SetUp(stat);
        string name = field.text;
        stats.Name.text = "Name: " + name;
        selectedData.stat = stat;
        if(string.IsNullOrEmpty(name))
        {
            name = "Guest";
        }
        selectedData.characterName = name;
    }

    public void DeleteCharacter(int id)
    {
        var popup = Resources.Load<PopUpBase>("UI/PopUpBase");
        PopUpBase b = Instantiate(popup, transform.parent);
        b.transform.SetAsLastSibling();
        string title = "Do You Want Delete This Character?";
        b.Init(title,()=> { DeleteData(id,b.gameObject); }, () => { Destroy(b.gameObject); } );
    }

    void DeleteData(int id,GameObject popup)
    {
        Destroy(buttons[id].gameObject);
        Destroy(popup);
        File.Delete(files[id]);
        createButton.SetActive(true);
    }
}
