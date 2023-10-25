using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menue : MonoBehaviour
{
   
    public void playGame()
    {
        SceneManager.LoadScene("DevScene");
        Debug.Log("play");
    }

    public void quitGame()
    {
        Application.Quit();
        Debug.Log("quit");
    }
}
