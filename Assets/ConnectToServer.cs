using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        print("start");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        print("conencting to lobby");
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("MainMenu");
        print("connected to lobby");
    }

}
