using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class Teleport : MonoBehaviour, Interactable
{
    [SerializeField]
    Levels level = Levels.Level1;
    bool interacting = false;
    
   
    public void Interact(Player p = null)
    {
        if (interacting) return;
        interacting = true;
        if(NetworkManager.instance!=null)
        {
            StartCoroutine(ChangeSceneCo(p));
        }
        else
        {
            SceneManager.LoadScene(level.ToString());
        }
    }

    IEnumerator ChangeSceneCo(Player p)
    {
        p.data.LevelName = level.ToString();
        p.data.currentScene = (int)level;
        SaveManager.SaveData<SaveData>(p.data.characterName, p.data);
        WorldManager.instance.playerList.Remove(p.transform);
        NetworkManager.instance.fadescreen.Play("FadeIn");
        yield return new WaitForSeconds(1.5f);
        NetworkManager.instance.ChangeRoom(level);
    }
}

public enum Levels
{
    LogIn,
    Level1,
    Level2
}