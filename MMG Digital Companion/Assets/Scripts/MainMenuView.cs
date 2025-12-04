using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.Rendering.LookDev; -- commented due to compiler error (unused import)
using UnityEngine;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private GameObject SidePanel;
    [SerializeField] private GameObject containerOptions;
    [SerializeField] private GameObject CoverImage;

    public void OnOptionsPressed()
    {
        this.SidePanel.SetActive(false);
        this.containerOptions.SetActive(true);
        if (this.CoverImage != null)
        {
            this.CoverImage.SetActive(false);
            Debug.Log("CoverImage hidden");
        }
        else
        {
            Debug.LogWarning("CoverImage is not assigned in MainMenuView!");
        }
    }

    public void OnBackOptionsPressed()
    {
        this.SidePanel.SetActive(true);
        this.containerOptions.SetActive(false);
        if (this.CoverImage != null)
        {
            this.CoverImage.SetActive(true);
            Debug.Log("CoverImage shown");
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
