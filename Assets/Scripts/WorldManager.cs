using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Transform respawnPoint=null;
    public List<Transform> playerList = new List<Transform>();
    [SerializeField]
    Vector3 deathZone = new Vector3(0,-10,0);
    public static WorldManager instance;
    public Object[] levelList = null;
    private void Awake()
    {
        instance = this;
        levelList = Resources.LoadAll<Object>("Scenes");
    }
    private void Start()
    {
        deathZone = transform.position + deathZone;
    }
    private void Update()
    {
        if (playerList.Count < 1) return;
        foreach(var item in playerList)
        {
            if(item!=null)
            {
                if (item.transform.position.y < deathZone.y)
                {
                    item.transform.position = respawnPoint.position;
                }
            }
        }
    }
}
