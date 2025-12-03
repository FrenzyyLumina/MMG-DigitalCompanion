using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] static GameObject txtTurnCount;
    [SerializeField] static GameObject SwingPanel;

    [SerializeField] static GameObject containerMainChoice;
    [SerializeField] static GameObject containerMoveChoice;
    [SerializeField] static GameObject containerTargetChoice;
    [SerializeField] static GameObject containerBinaryChoice;
    [SerializeField] static GameObject txtBinaryPrompt;
    [SerializeField] static Button butSkipTurn;

    private static bool turnEnded = false;

    // Basic Features
    public static void SetTurnCount(int turnCount)
    {
        //txtTurnCount.GetComponent<UnityEngine.UI.Text>().text = "Turn: " + turnCount.ToString();
        txtBinaryPrompt.GetComponent<TMP_Text>().text = "Turn: " + turnCount.ToString();
    }
    public static void SetBinaryPrompt(string prompt)
    {
        //txtBinaryPrompt.GetComponentInChildren<UnityEngine.UI.Text>().text = prompt;
        //txtBinaryPrompt.text = prompt;
        txtBinaryPrompt.GetComponent<TMP_Text>().text = prompt;
    }
    public static void DisplayMainChoice()
    {
        containerMainChoice.SetActive(true);
        containerMoveChoice.SetActive(false);
        containerTargetChoice.SetActive(false);
        containerBinaryChoice.SetActive(false);
    }
    public static void DisplayTargetChoice()
    {
        containerMainChoice.SetActive(false);
        containerMoveChoice.SetActive(false);
        containerTargetChoice.SetActive(true);
        containerBinaryChoice.SetActive(false);
    }
    public static void DisplayBinaryChoice()
    {
        containerMainChoice.SetActive(false);
        containerMoveChoice.SetActive(false);
        containerTargetChoice.SetActive(false);
        containerBinaryChoice.SetActive(true);
    }

    public static void OnViewPlayersPressed()
    {
        SwingPanel.SetActive(true);
    }
    public static void OnBackViewPlayersPressed()
    {
        SwingPanel.SetActive(false);
    }
    public static void OnTurnEnd()
    {
        turnEnded = true;
    }

    IEnumerator WaitForTurnEnd()
    {
        turnEnded = false;

        //TODO: Add more button detections
        butSkipTurn.onClick.AddListener(() =>  turnEnded = true);
        yield return new WaitUntil(() => turnEnded);
    }
    

    // Main Choices
    public static void OnMovePressed()
    {
        containerMainChoice.SetActive(false);
        containerMoveChoice.SetActive(true);
        containerTargetChoice.SetActive(false);
        containerBinaryChoice.SetActive(false);
    }
    public static void OnUseItemPressed()
    {
        print("Item Used Pressed");
    }
    public static void OnCompleteObjectivePressed()
    {
        print("Complete Objective Pressed");
    }
    public static void OnSkipTurnPressed()
    {
        print("Skip Turn Pressed");
    }

    // Move Choices
    public static void OnBackMovePressed()
    {
        DisplayMainChoice();
    }

    public static void OnSoftPressed()
    {
        print("Soft Move Pressed");
        SetBinaryPrompt("Did you land on the same square with another player?");
        DisplayBinaryChoice();
    }

    public static void OnLoudShortPressed()
    {
        print("Loud Short Move Pressed");
        SetBinaryPrompt("Did you land on the same square with another player?");
        DisplayBinaryChoice();
    }

    public static void OnLoudLongPressed()
    {
        print("Loud Long Move Pressed");
        SetBinaryPrompt("Did you land on the same square with another player?");
        DisplayBinaryChoice();
    }

    // Target Choices
    public static void OnPlayerAPressed()
    {
        print("Player A Targeted");
    }
    public static void OnPlayerBPressed()
    {
        print("Player B Targeted");
    }
    public static void OnPlayerCPressed()
    {
        print("Player C Targeted");
    }

    // Binary Choices
    public static void OnYes()
    {
        print("Yes");
        DisplayMainChoice();
    }
    public static void OnNo()
    {
        print("No");
        DisplayMainChoice();
    }
}
