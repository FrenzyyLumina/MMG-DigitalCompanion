using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
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
    }

    public void OnThreePlayerPressed()
    {
        print("Three Player Pressed");
    }

    public void OnFourPlayerPressed()
    {
        print("Four Player Pressed");
    }
}
