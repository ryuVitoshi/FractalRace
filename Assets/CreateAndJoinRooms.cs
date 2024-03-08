using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField _createInput;
    public TMP_InputField _joinInput;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(_createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("EnvScene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
