using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup MainMenu;

    public void PlayGame()
    {
        Debug.Log("Play button pressed.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Back()
    {
        Debug.Log("Back button pressed.");
        MainMenu.alpha = 0;
        MainMenu.blocksRaycasts = false;
    }

    public void Level()
    {
        Debug.Log("Level button pressed.");
        MainMenu.alpha = 1;
        MainMenu.blocksRaycasts = true;
    }

    public void Exit()
    {
        Debug.Log("Exit button pressed. Quitting application...");
        Application.Quit();
    }
}