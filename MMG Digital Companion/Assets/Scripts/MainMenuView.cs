using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.Rendering.LookDev; -- commented due to compiler error (unused import)
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private GameObject SidePanel;
    [SerializeField] private GameObject containerOptions;
    
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
    }

    public void OnOptionsPressed()
    {
        this.SidePanel.SetActive(false);
        this.containerOptions.SetActive(true);
        if (coverImage != null)
        {
            coverImage.enabled = false;
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
    }

    public void OnTwoPlayerPressed()
    {
        print("Two Player Pressed");
        GameManager.Instance.SetPlayerCount(2);
        GameManager.Instance.StartGameStartScene();
    }

    public void OnThreePlayerPressed()
    {
        print("Three Player Pressed");
        GameManager.Instance.SetPlayerCount(3);
        GameManager.Instance.StartGameStartScene();
    }

    public void OnFourPlayerPressed()
    {
        print("Four Player Pressed");
        GameManager.Instance.SetPlayerCount(4);
        GameManager.Instance.StartGameStartScene();
    }
}
