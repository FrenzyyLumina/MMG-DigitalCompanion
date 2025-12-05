using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int totalPlayers = 2;
    public int TotalPlayers 
    { 
        get 
        { 
            Debug.Log($"Getting TotalPlayers: {totalPlayers}");
            return totalPlayers; 
        }
        set 
        { 
            Debug.Log($"Setting TotalPlayers from {totalPlayers} to {value}");
            totalPlayers = value; 
        }
    }
    
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
        Debug.Log($"SetPlayerCount called with: {count}");
        TotalPlayers = count;
        CurrentScanningPlayer = 0;
        PlayerRoles = new GameEnums.Role[4];
        Debug.Log($"After SetPlayerCount, TotalPlayers is: {TotalPlayers}");
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
        Debug.Log("StartGameStartScene called - Loading GameStart scene...");
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
        Debug.Log($"QR Scanner returned role: {scannedRole} for Player {CurrentScanningPlayer + 1}");
        
        // Store the scanned role
        SetPlayerRole(CurrentScanningPlayer, scannedRole);
        
        // Move to next player
        CurrentScanningPlayer++;
        
        Debug.Log($"Moving to player {CurrentScanningPlayer + 1}. Total players: {TotalPlayers}");
        
        // Always return to GameStart, which will handle transition to Game if all players scanned
        SceneManager.LoadScene("GameStart");
    }
}
