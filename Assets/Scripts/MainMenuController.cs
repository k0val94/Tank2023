using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject PlayMenu;
    [SerializeField] private GameObject LevelSelectionMenu;
    [SerializeField] private GameObject OptionsMenu;

    private void Start()
    {
        MainMenu.SetActive(true);
        PlayMenu.SetActive(false);
        LevelSelectionMenu.SetActive(false);
        OptionsMenu.SetActive(false);
    }

    public void PlayGame()
    {
        Debug.Log("Play button pressed.");
        MainMenu.SetActive(false);
        PlayMenu.SetActive(true);
    }

    public void OpenStartLevel1()
    {
        Debug.Log("Level 1 selected.");
        SceneManager.LoadScene("Level1");
    }

    public void OpenSkirmish()
    {
        Debug.Log("Skirmish button pressed.");
        SceneManager.LoadScene("SkirmishScene");
    }

    public void OpenOptions()
    {
        Debug.Log("Options button pressed.");
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void OpenLevelSelection()
    {
        Debug.Log("Level Selection button pressed.");
        PlayMenu.SetActive(false);
        LevelSelectionMenu.SetActive(true);
    }

    public void OpenMapCreator()
    {
        Debug.Log("MapCreator selected.");
        SceneManager.LoadScene("MapCreator");
    }

    public void BackToMainMenuFromPlayMenu()
    {
        Debug.Log("Back to Main Menu from Play Menu.");
        PlayMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void BackToPlayMenuFromLevelSelection()
    {
        Debug.Log("Back to Play Menu from Level Selection.");
        LevelSelectionMenu.SetActive(false);
        PlayMenu.SetActive(true);
    }

    public void BackToMainMenuFromOptions()
    {
        Debug.Log("Back to Main Menu from Options.");
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void Exit()
    {
        Debug.Log("Exit button pressed. Quitting application...");
        Application.Quit();
    }
}