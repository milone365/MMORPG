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
    ChatClient client;
    public List<string> channels = new List<string>()
    {
        "global"
    };
    string currentChannel;
    [SerializeField]
    Text prefab = null;
    [SerializeField]
    Transform content = null;
    public string userID = "";

    private void Update()
    {
        if (client == null) return;
        client.Service();
    }
    public void SetUP(string userID)
    {
        this.userID = userID;
        client = new ChatClient(this, ConnectionProtocol.Tcp);
        client.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.AppVersion, new AuthenticationValues(this.userID));
    }

    public void Send()
    {
        string message = field.text;
        if (string.IsNullOrEmpty(message)) return;
        client.PublishMessage(currentChannel, message);
        CancelMessage();
    }

    public void CancelMessage()
    {
        field.text = "";
    }


    public void OnConnected()
    {
        currentChannel = channels[0];
        client.Subscribe(currentChannel);
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for(int i=0;i<senders.Length;i++)
        {
            var t = Instantiate(prefab, content);
            t.text = senders[i] + " : " + messages[i];
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    #region NOTUSED

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnDisconnected()
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

    #endregion
}
