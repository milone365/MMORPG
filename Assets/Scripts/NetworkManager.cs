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
    string world = "World";
    int currentLevel = 1;
    [SerializeField]
    GameObject startButton=null;
    [SerializeField]
    GameObject player = null;
    [SerializeField]
    GameObject[] disableOnStart = null;
    public Animator fadescreen = null;
    [SerializeField]
    CharacterCreate create = null;
    System.Action OnChangeLevel;

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
        var data = CharacterCreate.selectedData;
        currentLevel = data.currentScene;
        world = data.LevelName;
        fadescreen.Play("FadeIn");
        SaveManager.SaveData(data.characterName, data);
        PhotonNetwork.LoadLevel(currentLevel);
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 20;
        PhotonNetwork.JoinOrCreateRoom(world, options, TypedLobby.Default);
        startButton.SetActive(false);
        
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(JoinRoomCo());
    }

    IEnumerator JoinRoomCo()
    {
        yield return new WaitForSeconds(1);
        var playerObj= PhotonNetwork.Instantiate(player.name, Vector3.zero, Quaternion.identity);
        fadescreen.Play("FadeOut");
        var data = CharacterCreate.selectedData;
        if(data!=null)
        {
            playerObj.transform.position = new Vector3(data.x, data.y, data.z);
        }
    }

    public override void OnJoinedLobby()
    {
        if(OnChangeLevel!=null)
        {
            OnChangeLevel.Invoke();
        }
    }

    public void ChangeRoom(Levels level)
    {
        currentLevel = (int)level;
        world = level.ToString();
        if (OnChangeLevel==null)
        {
            OnChangeLevel = () =>
            {
                PhotonNetwork.LoadLevel(currentLevel);
                RoomOptions options = new RoomOptions();
                options.MaxPlayers = 20;
                PhotonNetwork.JoinOrCreateRoom(world, options, TypedLobby.Default);
            };
        }
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.ConnectUsingSettings();
    }

}
