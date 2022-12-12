using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    
    void Awake()
    {
        //if instance already exists and it isn't this one - destroy 
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            //set instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    //attempt to create a new room
    public void CreateRoom (string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    //atempt to join existing room
    public void JoinRoom (string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    //change scene using Photon RPC stuff
    [PunRPC]
    public void ChangeScene (string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        //CreateRoom("testroom");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }
}
