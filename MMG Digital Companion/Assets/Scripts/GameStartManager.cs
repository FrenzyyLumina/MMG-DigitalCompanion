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
    private Player[] players;
    private bool hasDoubleAgent = false;

    void Start()
    {
        // Get player count from GameManager or GameModel
        totalPlayers = GameManager.Instance.TotalPlayers;
        currentPlayerIndex = 0;
        
        // Initialize players array (will be stored in GameModel later)
        players = new Player[totalPlayers];
        for (int i = 0; i < totalPlayers; i++)
        {
            players[i] = new Player();
        }
        
        UpdateUI();
        SetBackgroundColor();
    }

    void UpdateUI()
    {
        if (txtCurrentPlayer != null)
        {
            txtCurrentPlayer.text = $"Player {currentPlayerIndex + 1}";
        }
    }

    void SetBackgroundColor()
    {
        if (Background != null)
        {
            Image bgImage = Background.GetComponent<Image>();
            if (bgImage != null)
            {
                // Cycle through colors for each player like in Game scene
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
        // Transition to QR Scanner scene
        GameManager.Instance.StartQRScannerScene();
    }

    // Called when returning from QR Scanner
    void OnEnable()
    {
        // Skip if not initialized yet (OnEnable runs before Start)
        if (players == null || GameManager.Instance == null)
        {
            return;
        }
        
        // Check if we just returned from scanning
        if (currentPlayerIndex > 0 || GameManager.Instance.CurrentScanningPlayer > 0)
        {
            currentPlayerIndex = GameManager.Instance.CurrentScanningPlayer;
            
            // Update player role from GameManager
            if (currentPlayerIndex > 0)
            {
                GameEnums.Role scannedRole = GameManager.Instance.PlayerRoles[currentPlayerIndex - 1];
                players[currentPlayerIndex - 1].setRole(scannedRole);
                
                // Check for Double Agent
                if (scannedRole == GameEnums.Role.Double_Agent)
                {
                    hasDoubleAgent = true;
                    Debug.Log("Double Agent detected in the game!");
                }
            }
            
            // Check if all players have scanned
            if (currentPlayerIndex >= totalPlayers)
            {
                // Store players in GameModel and start Game scene
                StartGame();
                return;
            }
            
            UpdateUI();
            SetBackgroundColor();
        }
    }

    void StartGame()
    {
        // Transfer player data to GameModel
        GameModel.setTotalPlayers(totalPlayers);
        for (int i = 0; i < totalPlayers; i++)
        {
            GameModel.getPlayerByIdx(i).setRole(players[i].getPlayerRole());
        }
        
        // Store Double Agent flag (for later use in Game scene)
        PlayerPrefs.SetInt("HasDoubleAgent", hasDoubleAgent ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log($"Starting game with {totalPlayers} players. Double Agent present: {hasDoubleAgent}");
        
        // Transition to Game scene
        GameManager.Instance.StartGameScene();
    }
}
