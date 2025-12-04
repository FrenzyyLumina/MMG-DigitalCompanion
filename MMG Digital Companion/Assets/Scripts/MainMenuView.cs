using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.Rendering.LookDev; -- commented due to compiler error (unused import)
using UnityEngine;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private GameObject SidePanel;
    [SerializeField] private GameObject containerOptions;

    public void OnOptionsPressed()
    {
        this.SidePanel.SetActive(false);
        this.containerOptions.SetActive(true);
    }

    public void OnBackOptionsPressed()
    {
        this.SidePanel.SetActive(true);
        this.containerOptions.SetActive(false);
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
