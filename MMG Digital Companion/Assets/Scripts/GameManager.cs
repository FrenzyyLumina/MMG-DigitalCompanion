using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int TotalPlayers { get; set; } = 2;
    public int CurrentScanningPlayer { get; set; } = 0;
    public GameEnums.Role[] PlayerRoles { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PlayerRoles = new GameEnums.Role[4]; // Max 4 players
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerCount(int count)
    {
        TotalPlayers = count;
        CurrentScanningPlayer = 0;
        PlayerRoles = new GameEnums.Role[4];
    }

    public void SetPlayerRole(int playerIndex, GameEnums.Role role)
    {
        if (playerIndex >= 0 && playerIndex < PlayerRoles.Length)
        {
            PlayerRoles[playerIndex] = role;
            Debug.Log($"Player {playerIndex + 1} role set to: {role}");
        }
    }

    public void StartGameStartScene()
    {
        CurrentScanningPlayer = 0;
        SceneManager.LoadScene("GameStart");
    }

    public void StartGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void StartQRScannerScene()
    {
        SceneManager.LoadScene("QRScanner");
    }

    public void ReturnFromQRScanner(GameEnums.Role scannedRole)
    {
        SetPlayerRole(CurrentScanningPlayer, scannedRole);
        CurrentScanningPlayer++;
        
        if (CurrentScanningPlayer >= TotalPlayers)
        {
            // All players have scanned, go to Game scene
            StartGameScene();
        }
        else
        {
            // Return to GameStart for next player
            SceneManager.LoadScene("GameStart");
        }
    }
}
