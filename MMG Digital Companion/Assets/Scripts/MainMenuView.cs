using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.Rendering.LookDev; -- commented due to compiler error (unused import)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private GameObject SidePanel;
    [SerializeField] private GameObject containerOptions;
    [SerializeField] private GameObject titleText;
    [SerializeField] private GameObject creditsText;
    [SerializeField] private TextMeshProUGUI playerCountIndicator;
    
    private Image coverImage;

    void Start()
    {
        // Find the CoverImage by name
        GameObject coverImageObj = GameObject.Find("CoverImage");
        if (coverImageObj != null)
        {
            coverImage = coverImageObj.GetComponent<Image>();
        }
        else
        {
            Debug.LogWarning("CoverImage GameObject not found!");
        }
        
        UpdatePlayerCountIndicator();
    }

    void UpdatePlayerCountIndicator()
    {
        if (playerCountIndicator != null && GameManager.Instance != null)
        {
            int playerCount = GameManager.Instance.TotalPlayers;
            if (playerCount == 0) playerCount = 2; // Default
            playerCountIndicator.text = $"{playerCount} Players";
        }
    }

    public void OnOptionsPressed()
    {
        this.SidePanel.SetActive(false);
        this.containerOptions.SetActive(true);
        if (coverImage != null)
        {
            coverImage.enabled = false;
        }
        if (titleText != null)
        {
            titleText.SetActive(false);
        }
        if (creditsText != null)
        {
            creditsText.SetActive(false);
        }
    }

    public void OnBackOptionsPressed()
    {
        this.SidePanel.SetActive(true);
        this.containerOptions.SetActive(false);
        if (coverImage != null)
        {
            coverImage.enabled = true;
        }
        if (titleText != null)
        {
            titleText.SetActive(true);
        }
        if (creditsText != null)
        {
            creditsText.SetActive(true);
        }
    }

    public void OnTwoPlayerPressed()
    {
        print("Two Player Pressed");
        GameManager.Instance.SetPlayerCount(2);
        Debug.Log($"Player count set to: {GameManager.Instance.TotalPlayers}");
        UpdatePlayerCountIndicator();
    }

    public void OnThreePlayerPressed()
    {
        print("Three Player Pressed");
        GameManager.Instance.SetPlayerCount(3);
        Debug.Log($"Player count set to: {GameManager.Instance.TotalPlayers}");
        UpdatePlayerCountIndicator();
    }

    public void OnFourPlayerPressed()
    {
        print("Four Player Pressed");
        GameManager.Instance.SetPlayerCount(4);
        Debug.Log($"Player count set to: {GameManager.Instance.TotalPlayers}");
        UpdatePlayerCountIndicator();
    }

    public void OnStartPressed()
    {
        Debug.Log("Start button pressed!");
        
        // Check if player count has been set, if not default to 2
        if (GameManager.Instance != null)
        {
            Debug.Log($"GameManager found. Current player count: {GameManager.Instance.TotalPlayers}");
            
            // If player count is 0, set default to 2
            if (GameManager.Instance.TotalPlayers == 0)
            {
                Debug.Log("Player count was 0, setting to 2");
                GameManager.Instance.SetPlayerCount(2);
            }
            
            Debug.Log("Calling StartGameStartScene...");
            GameManager.Instance.StartGameStartScene();
        }
        else
        {
            Debug.LogError("GameManager not found! Make sure GameManager exists in the scene.");
        }
    }
}
