using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour, Interactable
{
    [SerializeField]
    string LevelName = "Test2";
    bool interacting = false;
    public void Interact(Player p = null)
    {
        if (interacting) return;
        interacting = true;
        if(NetworkManager.instance!=null)
        {
            NetworkManager.instance.ChangeRoom(LevelName);
        }
        else
        {
            SceneManager.LoadScene(LevelName);
        }
    }
}
