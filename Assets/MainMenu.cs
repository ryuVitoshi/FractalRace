using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
        Application.Quit();
        //Debug.Log("Game Closed");
    }

    public void StartSinglePlayer()
    {
        SceneManager.LoadScene("EnvScene");
    }

    public void StartMultiPlayer()
    {
        SceneManager.LoadScene("Lobby");
    }
}
