using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    [SerializeField]
    GameObject connectPanel = null;
    const string world = "World";
    int currentLevel = 1;
    [SerializeField]
    GameObject startButton=null;
    [SerializeField]
    GameObject player = null;
    [SerializeField]
    GameObject[] disableOnStart = null;
    [SerializeField]
    Animator fadescreen = null;
    [SerializeField]
    CharacterCreate create = null;

    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        connectPanel.SetActive(false);
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
    }

    public void StartGame()
    {
        foreach(var o in disableOnStart)
        {
            o.SetActive(false);
        }
        SaveManager.SaveData(CharacterCreate.selectedData.characterName, CharacterCreate.selectedData);
        PhotonNetwork.LoadLevel(currentLevel);
        fadescreen.Play("FadeOut");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 20;
        PhotonNetwork.JoinOrCreateRoom(world, options, TypedLobby.Default);
        startButton.SetActive(false);
        StartCoroutine(JoinRoomCo());
    }

    IEnumerator JoinRoomCo()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.Instantiate(player.name, Vector3.zero, Quaternion.identity);
    }
}
