using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private readonly string MAIN_MENU_SCENE = "Main Menu";
    private readonly string GAME_START_SCENE = "GameStart";
    private readonly string GAME_SCENE = "Game";
    private readonly string QR_SCANNER_SCENE = "QRScanner";

    public void OnApplicationQuit()
    {
        print("Application Quit");
        Application.Quit();
    }

    public void OnGameStart()
    {
        // Check if player count has been set, if not default to 2
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.TotalPlayers == 0)
            {
                GameManager.Instance.SetPlayerCount(2);
            }
            GameManager.Instance.StartGameStartScene();
        }
        else
        {
            // Fallback if GameManager doesn't exist
            SceneManager.LoadScene(GAME_START_SCENE);
        }
    }

    public void OnGameStarted()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    public void OnGameEnded()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }
    public void OnQRScannerRequested()
    {
        SceneManager.LoadScene(QR_SCANNER_SCENE);
    }
}
