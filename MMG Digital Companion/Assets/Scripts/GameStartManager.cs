using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject Background;
    [SerializeField] private TextMeshProUGUI txtCurrentPlayer;
    [SerializeField] private Button btnScanRole;

    private int currentPlayerIndex = 0;
    private int totalPlayers;
    private bool hasDoubleAgent = false;

    void Start()
    {
        // Get player count from GameManager
        totalPlayers = GameManager.Instance.TotalPlayers;
        currentPlayerIndex = GameManager.Instance.CurrentScanningPlayer;
        
        Debug.Log($"GameStart: Starting with {totalPlayers} total players, currently on player {currentPlayerIndex + 1}");
        
        // Check if all players have finished scanning
        if (currentPlayerIndex >= totalPlayers)
        {
            Debug.Log("All players scanned, starting game!");
            StartGame();
            return;
        }
        
        UpdateUI();
        SetBackgroundColor();
    }

    void UpdateUI()
    {
        if (txtCurrentPlayer != null)
        {
            txtCurrentPlayer.text = $"Player {currentPlayerIndex + 1}'s Turn";
        }
    }

    void SetBackgroundColor()
    {
        if (Background != null)
        {
            Image bgImage = Background.GetComponent<Image>();
            if (bgImage != null)
            {
                // Cycle through colors for each player
                Color[] playerColors = new Color[]
                {
                    new Color(0.8f, 0.2f, 0.2f), // Red
                    new Color(0.2f, 0.8f, 0.2f), // Green
                    new Color(0.2f, 0.2f, 0.8f), // Blue
                    new Color(0.8f, 0.8f, 0.2f)  // Yellow
                };
                
                bgImage.color = playerColors[currentPlayerIndex % playerColors.Length];
            }
        }
    }

    public void OnScanButtonPressed()
    {
        Debug.Log($"Scan button pressed for Player {currentPlayerIndex + 1}");
        // Transition to QR Scanner scene
        GameManager.Instance.StartQRScannerScene();
    }

    void StartGame()
    {
        // Initialize GameModel with total players
        GameModel.setTotalPlayers(totalPlayers);
        
        // Transfer all player roles from GameManager to GameModel
        Player[] players = GameModel.getPlayers();
        for (int i = 0; i < totalPlayers; i++)
        {
            GameEnums.Role role = GameManager.Instance.PlayerRoles[i];
            players[i].setRole(role);
            
            // Check for Double Agent
            if (role == GameEnums.Role.Double_Agent)
            {
                hasDoubleAgent = true;
            }
            
            Debug.Log($"Player {i + 1} initialized with role: {role}");
        }
        
        // Store Double Agent flag
        PlayerPrefs.SetInt("HasDoubleAgent", hasDoubleAgent ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log($"Starting game with {totalPlayers} players. Double Agent present: {hasDoubleAgent}");
        
        // Transition to Game scene
        GameManager.Instance.StartGameScene();
    }
}
