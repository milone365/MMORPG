using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Chat : MonoBehaviour,IChatClientListener
{
    [SerializeField]
    InputField field = null;
    [SerializeField]
    Button sendBtn = null, cancelBtn;
    [SerializeField]
    Text textPrefab = null;
    [SerializeField]
    RectTransform content = null;
    ChatClient client;
    [SerializeField]
    string userName = "name";
    List<string> channels = new List<string>()
    {
        "Global"
    };
    string channel;
    [SerializeField]
    InputField privateField = null;
    [SerializeField]
    ChatType type=ChatType.Global;
    [SerializeField]
    GameObject privateButton = null;
    [SerializeField]
    Color selected, unselected;
    [SerializeField]
    GameObject globalButton = null;
    [SerializeField]
    GameObject globalChat = null, privateChat = null;
    [SerializeField]
    Transform privateContent = null;
    [SerializeField]
    Text privatePrefab = null;
    [SerializeField]
    int maxLimit = 10;
    List<GameObject> privateList = new List<GameObject>();
    List<GameObject> gloalList = new List<GameObject>();
    NetworkPlayer player;
    public void Init(NetworkPlayer player)
    {
        this.player = player;
        channel = channels[0];
        userName = player.userName;
        client = new ChatClient(this, ConnectionProtocol.Tcp);
        client.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.AppVersion, new AuthenticationValues(userName));
        globalButton.GetComponent<Image>().color = selected;
        privateButton.GetComponent<Image>().color = unselected;
    }
    private void Update()
    {
        if(client!=null)
        {
            //aggiornamento
            client.Service();
        }
    }

    public void Send()
    {
        if (client == null)
        {
            Debug.LogAssertion("Client is null");
            CancelText();
            return;
        }

        string s = field.text;
        if (string.IsNullOrEmpty(s)) return;
        switch (type)
        {
            case ChatType.Global:
                client.PublishMessage(channel, s);
                break;
            case ChatType.Private:
                string name = privateField.text;
                if(!string.IsNullOrEmpty(name))
                {
                    client.SendPrivateMessage(name, s);
                }
                break;
        }
        CancelText();
    }

    public void CancelText()
    {
        field.text = "";
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for( int i=0;i<senders.Length;i++)
        {
            var m = senders[i] + " : " + messages[i];
            var o= CreateMessage(m,content,textPrefab);
            gloalList.Add(o);
            LenghtCheck(gloalList);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {     
            var m = "*private*" + sender + " : " + message;
            var o= CreateMessage(m,privateContent,privatePrefab);
            privateList.Add(o);
            LenghtCheck(privateList);
    }

    GameObject CreateMessage(string s,Transform content,Text pref)
    {
        var t = Instantiate(pref, content);
        t.text = s;

        return t.gameObject;
    }

    void LenghtCheck(List<GameObject> list)
    {
        if(list.Count>maxLimit)
        {
            var toDelete = list[0];
            list.RemoveAt(0);
            Destroy(toDelete);
        }
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected");
    }

    public void OnConnected()
    {
        Debug.Log("Connection");
        client.Subscribe(channel);
    }

    public void PrivateView()
    {
        privateButton.SetActive(false);
        privateField.gameObject.SetActive(true);
        type = ChatType.Private;
        globalButton.GetComponent<Image>().color = unselected;
        privateButton.GetComponent<Image>().color = selected;
        privateChat.SetActive(true);
        globalChat.SetActive(false);
    }
    public void GlobalView()
    {
        privateButton.SetActive(true);
        privateField.gameObject.SetActive(false);
        type = ChatType.Global;
        globalButton.GetComponent<Image>().color = selected;
        privateButton.GetComponent<Image>().color = unselected;
        privateChat.SetActive(false);
        globalChat.SetActive(true);
    }

    #region OTHER
    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
    public void OnChatStateChange(ChatState state)
    {
    }
    #endregion

    enum ChatType
    {
        Global,
        Private
    }
}
