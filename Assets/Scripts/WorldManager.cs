using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WorldManager : MonoBehaviourPun
{
    public Transform respawnPoint = null;
    public List<Transform> playerList = new List<Transform>();
    [SerializeField]
    Vector3 deathZone = new Vector3(0, -10, 0);
    public static WorldManager instance;
    public Object[] levelList = null;
    public GameObject[] prefabList = null;
    public PhotonView view;
    List<Sprite> allGameImages = new List<Sprite>();

    public Sprite GetSprite(string s)
    {
        foreach(var item in allGameImages)
        {
            if(item.name==s)
            {
                return item;
            }
        }
        return null;
    }
    private void Awake()
    {
        instance = this;
        levelList = Resources.LoadAll<Object>("Scenes");
        allGameImages.AddRange(Resources.LoadAll<Sprite>("Images"));
    }
    private void Start()
    {
        deathZone = transform.position + deathZone;
        prefabList = Resources.LoadAll<GameObject>("Prefabs");
        view = PhotonView.Get(this);
    }
    private void Update()
    {
        if (playerList.Count < 1) return;
        foreach (var item in playerList)
        {
            if (item != null)
            {
                if (item.transform.position.y < deathZone.y)
                {
                    item.transform.position = respawnPoint.position;
                }
            }
        }
    }

    public void SpawnEffect(string Name, Vector3 pos, Vector3 rot)
    {
        var g = GetPrefab(Name);
        if (g == null) return;

        if (PhotonNetwork.IsConnected)
        {
            if(view==null) view = PhotonView.Get(this);
            var position = VectorConverter(pos);
            var rotations = VectorConverter(rot);
            view.RPC("SpawnPrefabRpc", RpcTarget.All, Name, position, rotations);
        }
        else
        {
            Instantiate(g, pos, Quaternion.Euler(rot));
        }
    }

    [PunRPC]
    public void SpawnPrefabRpc(string Name, float[] pos, float[] rot)
    {
        var g = GetPrefab(Name);
        if (g == null) return;
        var position = ToVector(pos);
        var rotation = ToVector(rot);
        Instantiate(g, position, Quaternion.Euler(rotation));
    }

    public GameObject GetPrefab(string n)
    {
        foreach(var item in prefabList)
        {
            if(item.name==n)
            {
                return item;
            }
        }
        return null;
    }

    public float[] VectorConverter(Vector3 v)
    {
        return new float[]{v.x,v.y,v.z};
    }
    public Vector3 ToVector(float[] arr)
    {
        return new Vector3(arr[0], arr[1], arr[2]);
    }
}
