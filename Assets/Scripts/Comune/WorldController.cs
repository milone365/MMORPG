using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public static WorldController instance;
    public List<Transform> playerList = new List<Transform>();
    public List<Transform> spawnPoints = new List<Transform>();
    [SerializeField]
    Transform deathZone = null;
    public Transform cemetery;

    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        foreach(var p in playerList)
        {
            if(p.position.y<deathZone.position.y)
            {
                p.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
            }
        }
    }
}
