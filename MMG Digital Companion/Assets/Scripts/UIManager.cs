using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private readonly string MAIN_MENU_SCENE = "Main Menu";
    private readonly string GAME_SCENE = "Game";
    private readonly string QR_SCANNER_SCENE = "QRScanner";

    public void OnApplicationQuit()
    {
        print("Application Quit");
        Application.Quit();
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
