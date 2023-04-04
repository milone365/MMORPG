using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveManager 
{
    const string location = "Assets/Resources/Data";
    
    public static void SaveData<T>(string fileName,T template)
    {
        if(!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        string data = JsonUtility.ToJson(template);
        string path = Path.Combine(location, fileName);
        using(FileStream stream=new FileStream(path,FileMode.Create))
        {
            using(StreamWriter writer=new StreamWriter(stream))
            {
                writer.Write(data);
            }
            stream.Close();
        }
        Debug.Log("game saved");
    }

    public static T LoadData<T>(string fileName)where T: class
    {
        T load = null;
        string read = "";
        string path = Path.Combine(location, fileName);
        if(!File.Exists(path))
        {
            return null;
        }

        using (FileStream stream=new FileStream(path,FileMode.Open))
        {
            using(StreamReader reader=new StreamReader(stream))
            {
                read = reader.ReadToEnd();
            }
            stream.Close();
        }
        load = JsonUtility.FromJson<T>(read);

        return load;
    }
}
