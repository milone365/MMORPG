using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
public class CharacterCreate : MonoBehaviour
{
    const string location = "Assets/Resources/Data";
    public List<SaveData> allData = new List<SaveData>();
    [SerializeField]
    CharButton charButton = null;
    [SerializeField]
    Transform charParent = null;
    SaveData selectedData;
    List<CharButton> buttons = new List<CharButton>();

    private void Start()
    {
        var files = Directory.GetFiles(location).Where(x=>!x.Contains(".meta")).ToArray();
        if(files.Length<1)
        {

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
            buttons[0].icon.SetActive(true);
        }
    }
    
    public void Select(int ID)
    {
        foreach(var b in buttons)
        {
            b.icon.SetActive(false);
        }
        Debug.Log("select");
        selectedData = allData[ID];
        buttons[ID].icon.SetActive(true);
    }
}
