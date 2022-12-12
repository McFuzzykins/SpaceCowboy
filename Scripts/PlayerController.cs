using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController: MonoBehaviourPunCallbacks, IPunObservable
{
    //public Transform RespawnPoint;

    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;

    [HideInInspector]
    public float curHatTime;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;

    // Start is called before the first frame update
    void Start()
    {
        //RespawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Move();
            
            if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

            //track time wearing hat
            if (hatObject.activeInHierarchy)
            {
                Debug.Log("Hat On");
                curHatTime += Time.deltaTime;
                if (curHatTime % 5 == 0)
                {
                    Debug.Log(curHatTime);
                }
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (curHatTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        rig.velocity = new Vector3(x, rig.velocity.y, z);
    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 0.7f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    //called when the player object is instantiated
    [PunRPC]
    public void Initialize (Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        //give first player hat
        if (id == 1)
        {
            GameManager.instance.GiveHat(id, true);
            hatObject.SetActive(true);
            Debug.Log(player + " Has Hat");
        }

        //if not local player, disable physics as that is controlled by user and synced to all other clients
        if (!photonView.IsMine)
            rig.isKinematic = true;
    }

    //el sombrero: verdadero o falso
    public void SetHat (bool hasHat)
    {
        hatObject.SetActive(hasHat);
        Debug.Log("Hat Active");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;

        //hit another player
        if (collision.gameObject.CompareTag("Player"))
        {
            //El del Sombrero?
            if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
            {
                //¿Podemos conseguir el sombrero?
                if (GameManager.instance.CanGetHat())
                {
                    //Tengo el sombrero
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }

    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHatTime);
        }
        else if (stream.IsReading)
        {
            curHatTime = (float)stream.ReceiveNext();
        }
    }
    
    /* void RespawnPlayer()
    {
        if (player.tag == "Player" && player.position.y <= -2)
        {
            Debug.Log("Respawn Initiated");
            player.position = new Vector3(RespawnPoint.position.x, RespawnPoint.position.y, RespawnPoint.position.z);
        }
    } */

}
