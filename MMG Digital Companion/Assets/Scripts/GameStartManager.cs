using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI playerNumberText;
    [SerializeField] private Button scanButton;

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        int currentPlayer = GameManager.Instance.CurrentScanningPlayer;
        
        // Update player number text
        if (playerNumberText != null)
        {
            playerNumberText.text = $"Player {currentPlayer + 1}";
        }

        // Ensure scan button is active
        if (scanButton != null)
        {
            scanButton.interactable = true;
        }
    }

    public void OnScanButtonPressed()
    {
        GameManager.Instance.StartQRScannerScene();
    }
}
