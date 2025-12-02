using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] GameObject txtTurnCount;
    [SerializeField] GameObject SwingPanel;

    [SerializeField] GameObject containerMainChoice;
    [SerializeField] GameObject containerMoveChoice;
    [SerializeField] GameObject containerTargetChoice;
    [SerializeField] GameObject containerBinaryChoice;
    [SerializeField] GameObject txtBinaryPrompt;

    // Basic Features
    public void SetTurnCount(int turnCount)
    {
        this.txtTurnCount.GetComponent<UnityEngine.UI.Text>().text = "Turn: " + turnCount.ToString();
    }
    public void SetBinaryPrompt(string prompt)
    {
        this.containerBinaryChoice.GetComponentInChildren<UnityEngine.UI.Text>().text = prompt;
    }
    public void displayMainChoice()
    {
        this.containerMainChoice.SetActive(true);
        this.containerMoveChoice.SetActive(false);
        this.containerTargetChoice.SetActive(false);
        this.containerBinaryChoice.SetActive(false);
    }
    public void displayTargetChoice()
    {
        this.containerMainChoice.SetActive(false);
        this.containerMoveChoice.SetActive(false);
        this.containerTargetChoice.SetActive(true);
        this.containerBinaryChoice.SetActive(false);
    }
    public void displayBinaryChoice()
    {
        this.containerMainChoice.SetActive(false);
        this.containerMoveChoice.SetActive(false);
        this.containerTargetChoice.SetActive(false);
        this.containerBinaryChoice.SetActive(true);
    }

    public void OnViewPlayersPressed()
    {
        SwingPanel.SetActive(true);
    }
    public void OnBackViewPlayersPressed()
    {
        SwingPanel.SetActive(false);
    }

    // Main Choices
    public void OnMovePressed()
    {
        this.containerMainChoice.SetActive(false);
        this.containerMoveChoice.SetActive(true);
        this.containerTargetChoice.SetActive(false);
        this.containerBinaryChoice.SetActive(false);
    }
    public void OnUseItemPressed()
    {
        print("Item Used Pressed");
    }
    public void OnCompleteObjectivePressed()
    {
        print("Complete Objective Pressed");
    }
    public void OnSkipTurnPressed()
    {
        print("Skip Turn Pressed");
    }

    // Move Choices
    public void OnBackMovePressed()
    {
        displayMainChoice();
    }

    public void OnSoftPressed()
    {
        print("Soft Move Pressed");
        SetBinaryPrompt("Did you land on the same square with another player?");
        displayBinaryChoice();
    }

    public void OnLoudShortPressed()
    {
        print("Loud Short Move Pressed");
        SetBinaryPrompt("Did you land on the same square with another player?");
        displayBinaryChoice();
    }

    public void OnLoudLongPressed()
    {
        print("Loud Long Move Pressed");
        SetBinaryPrompt("Did you land on the same square with another player?");
        displayBinaryChoice();
    }

    // Target Choices
    public void OnPlayerAPressed()
    {
        print("Player A Targeted");
    }
    public void OnPlayerBPressed()
    {
        print("Player B Targeted");
    }
    public void OnPlayerCPressed()
    {
        print("Player C Targeted");
    }

    // Binary Choices
    public void OnYes()
    {
        print("Yes");
        displayMainChoice();
    }
    public void OnNo()
    {
        print("No");
        displayMainChoice();
    }
}
