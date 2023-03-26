using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    const string world = "World";
    [SerializeField]
    GameObject character = null;
    bool loading = false;

    private void Start()
    {
        if(instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        UIManager.instance.ShowLoading(false);
    }

    public void StartGame()
    {
        if (loading) return;
        loading = true;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 20;
        PhotonNetwork.JoinOrCreateRoom(world, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("create room");
    }
    public override void OnJoinedRoom()
    {
        StartCoroutine(EntryCo());
    }

    IEnumerator EntryCo()
    {
        PhotonNetwork.LoadLevel(1);
        yield return new WaitForSeconds(1);
        PhotonNetwork.Instantiate(character.name, new Vector3(0, 1, 0), Quaternion.identity);
        UIManager.instance.CloseStartScreen();
    }
}
