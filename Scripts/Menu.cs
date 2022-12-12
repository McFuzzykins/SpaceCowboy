using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    // Start is called before the first frame update
    void Start()
    {
        //disable buttons at the start as we're not connected to the server yet
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    //called when we connect to server. Enables buttons
    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    void setScreen (GameObject screen)
    {
        //deactivate all screens
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        
        //enable requested screen
        screen.SetActive(true);
    }

    public void OnCreateRoomButton (TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    public void OnJoinRoomButton (TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        setScreen(mainScreen);
    }

    public void OnStartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }

    public void OnPLayerNameUpdate (TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        setScreen(lobbyScreen);
        //with people in lobby, tell everyone to update lobby UI
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";

        //display all players currently in lobby
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        //only host can start
        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }

    public override void OnPlayerLeftRoom (Player otherPlayer)
    {
        //no RPC because OnJoinRoom is for client who just joined. This is called for all clients, so no need for RPC
        UpdateLobbyUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
