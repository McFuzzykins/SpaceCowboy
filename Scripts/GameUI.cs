using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;

    //instance
    public static GameUI instance; 

    void Awake()
    {
        //set the instance to this script
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerUI();
    }

    void InitializePlayerUI()
    {
        //loop through all containers
        for (int i = 0; i < playerContainers.Length; i++)
        {
            PlayerUIContainer container = playerContainers[i];

            //only enable and modify UI containers we need 
            if (i < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[i].NickName;
                container.hatTimeSlider.maxValue = GameManager.instance.timeToWin;
            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerUI();
    }
    
    void UpdatePlayerUI()
    {
        //loop through all players
        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            if (GameManager.instance.players[i] != null)
            {
                playerContainers[i].hatTimeSlider.value = GameManager.instance.players[i].curHatTime;
            }
        }
    }

    public void SetWinText (string winnerName)
    {
        winText.gameObject.SetActive(true);

        if (winnerName == "Spike" || winnerName == "spike")
        {
            winText.text = "See you later, Space Cowboy";
        }
        else
        {
            winText.text = winnerName + " wins";
        }
    } 
}

[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;
}