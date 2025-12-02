using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        //this.txtTurnCount.GetComponent<UnityEngine.UI.Text>().text = "Turn: " + turnCount.ToString();
        this.txtBinaryPrompt.GetComponent<TMP_Text>().text = "Turn: " + turnCount.ToString();
    }
    public void SetBinaryPrompt(string prompt)
    {
        //this.txtBinaryPrompt.GetComponentInChildren<UnityEngine.UI.Text>().text = prompt;
        //this.txtBinaryPrompt.text = prompt;
        this.txtBinaryPrompt.GetComponent<TMP_Text>().text = prompt;
    }
    public void DisplayMainChoice()
    {
        this.containerMainChoice.SetActive(true);
        this.containerMoveChoice.SetActive(false);
        this.containerTargetChoice.SetActive(false);
        this.containerBinaryChoice.SetActive(false);
    }
    public void DisplayTargetChoice()
    {
        this.containerMainChoice.SetActive(false);
        this.containerMoveChoice.SetActive(false);
        this.containerTargetChoice.SetActive(true);
        this.containerBinaryChoice.SetActive(false);
    }
    public void DisplayBinaryChoice()
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
        DisplayMainChoice();
    }

    public void OnSoftPressed()
    {
        print("Soft Move Pressed");
        SetBinaryPrompt("Did you land on the same square with another player?");
        DisplayBinaryChoice();
    }

    public void OnLoudShortPressed()
    {
        print("Loud Short Move Pressed");
        SetBinaryPrompt("Did you land on the same square with another player?");
        DisplayBinaryChoice();
    }

    public void OnLoudLongPressed()
    {
        print("Loud Long Move Pressed");
        SetBinaryPrompt("Did you land on the same square with another player?");
        DisplayBinaryChoice();
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
        DisplayMainChoice();
    }
    public void OnNo()
    {
        print("No");
        DisplayMainChoice();
    }
}
