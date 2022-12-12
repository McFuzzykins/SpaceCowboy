using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;       
    public float timeToWin;              //time needed for player to hold sombrero for W
    public float invincibleDuration;     //how long after a player gets el sombrero they are invincible
    private float hatPickupTime;         //the time the hat was picked up by the current holder

    [Header("Players")]
    public string playerPrefabLocation;  //path in Resources folder to the Player prefab
    public Transform[] spawnPoints;      //array of all available spawn points
    public PlayerController[] players;   //array of all the players
    public int playerWithHat;            //id of the player with El Sombrero
    private int playersInGame;           //# of players in the game

    //instance
    public static GameManager instance;

    void Awake()
    {
        //lazy singleton according to Slease, but says narrator justifies it
        //so don't do this again. go to NetworkManager for a better singleton
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    //spawns player and initializes it
    void SpawnPlayer()
    {
        //instantiate player across network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        //get player script
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        //initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer (int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer (GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject); 
    }

    //called when player collides with El Del Sombrero
    [PunRPC]
    public void GiveHat (int playerID, bool initialGive)
    {
        //remove el sombrero from El Del Sombrero
        if (!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        //give hat to new player
        playerWithHat = playerID;
        GetPlayer(playerID).SetHat(true);
        hatPickupTime = Time.time;
    }

    //can we actually get the hat? 
    public bool CanGetHat()
    {
        if (Time.time > hatPickupTime + invincibleDuration)
            return true;
        else
            return false;
    }

    [PunRPC]
    void WinGame (int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        //set UI to show who's won

        Invoke("GoBackToMenu", 3.0f);

        GameUI.instance.SetWinText(player.photonPlayer.NickName);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
