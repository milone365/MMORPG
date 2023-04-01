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
    private void Awake()
    {
        instance = this;
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
            if(item.transform.position.y<deathZone.y)
            {
                item.transform.position = respawnPoint.position;
            }
        }
    }
}
