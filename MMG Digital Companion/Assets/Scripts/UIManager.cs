using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private const string MAIN_MENU_SCENE = "Main Menu";
    private const string GAME_SCENE = "Game";

    public void OnGameStarted()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    public void OnGameFinished()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }
}
