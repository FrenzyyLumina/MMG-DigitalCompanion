using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] GameObject _SwingPanel;
    [SerializeField] GameObject _containerMainChoice;
    [SerializeField] GameObject _containerMoveChoice;
    [SerializeField] GameObject _containerTargetChoice;
    [SerializeField] GameObject _containerBinaryChoice;
    [SerializeField] TMP_Text _txtTurnCount;
    [SerializeField] TMP_Text _txtCurrentPlayer;
    [SerializeField] TMP_Text _txtBinaryPrompt;
    [SerializeField] Button _butSkipTurn;
    [SerializeField] Button _butRollResult;

    private static GameObject SwingPanel;
    private static GameObject containerMainChoice;
    private static GameObject containerMoveChoice;
    private static GameObject containerTargetChoice;
    private static GameObject containerBinaryChoice;
    private static TMP_Text txtTurnCount;
    private static TMP_Text txtCurrentPlayer;
    private static TMP_Text txtBinaryPrompt;
    private static Button butSkipTurn;
    private static Button butRollResult;

    private static event Action OnAnyTurnEnd;

    public static event Action OnMovePressedEvent;
    public static event Action OnUseItemPressedEvent;
    public static event Action OnCompleteObjectivePressedEvent;
    public static event Action OnSkipTurnPressedEvent;
    public static event Action OnSoftPressedEvent;
    public static event Action OnLoudShortPressedEvent;
    public static event Action OnLoudLongPressedEvent;
    public static event Action<string> OnPlayerTargetedEvent; // Player ID as parameter
    public static event Action<bool> OnBinaryChoiceEvent; // true for Yes, false for No
    public static event Action OnRollResultContinueEvent;

    private void Awake()
    {
        // Copy non-static references to static fields
        SwingPanel              = _SwingPanel;
        containerMainChoice     = _containerMainChoice;
        containerMoveChoice     = _containerMoveChoice;
        containerTargetChoice   = _containerTargetChoice;
        containerBinaryChoice   = _containerBinaryChoice;
        txtTurnCount            = _txtTurnCount;
        txtCurrentPlayer        = _txtCurrentPlayer;
        txtBinaryPrompt         = _txtBinaryPrompt;
        butSkipTurn             = _butSkipTurn;
        butRollResult           = _butRollResult;
    }

    // Basic Features
    public static void SetTurnCount(int turnCount)
    {
        //txtTurnCount.GetComponent<UnityEngine.UI.Text>().text = "Turn: " + turnCount.ToString();
        txtTurnCount.GetComponent<TMP_Text>().text = "Turn: " + turnCount.ToString();
    }
    public static void SetCurrentPlayer(int plrNumber)
    {
        txtCurrentPlayer.text = $"Player {plrNumber}'s Turn";
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
        //turnEnded = true;
        OnAnyTurnEnd?.Invoke();
    }

    public static IEnumerator WaitForTurnEnd()
    {
        bool turnEnded = false;

        void Handler() => turnEnded = true;
        OnAnyTurnEnd += Handler;

        yield return new WaitUntil(() => turnEnded);
        OnAnyTurnEnd -= Handler;
    }
    

    // Main Choices
    public static void OnMovePressed()
    {
        containerMainChoice.SetActive(false);
        containerMoveChoice.SetActive(true);
        containerTargetChoice.SetActive(false);
        containerBinaryChoice.SetActive(false);

        OnMovePressedEvent?.Invoke();
    }
    public static void OnUseItemPressed()
    {
        print("Item Used Pressed");
        OnUseItemPressedEvent?.Invoke();
    }
    public static void OnCompleteObjectivePressed()
    {
        print("Complete Objective Pressed");
        OnCompleteObjectivePressedEvent?.Invoke();
    }
    public static void OnSkipTurnPressed()
    {
        print("Skip Turn Pressed");
        OnSkipTurnPressedEvent?.Invoke();
    }

    // Move Choices
    public static void OnBackMovePressed()
    {
        DisplayMainChoice();
    }

    public static void OnSoftPressed()
    {
        print("Soft Move Pressed");
        //SetBinaryPrompt("Did you land on the same square with another player?");
        //DisplayBinaryChoice();
        OnSoftPressedEvent?.Invoke();
    }

    public static void OnLoudShortPressed()
    {
        print("Loud Short Move Pressed");
        //SetBinaryPrompt("Did you land on the same square with another player?");
        //DisplayBinaryChoice();
        OnLoudShortPressedEvent?.Invoke();
    }

    public static void OnLoudLongPressed()
    {
        print("Loud Long Move Pressed");
        //SetBinaryPrompt("Did you land on the same square with another player?");
        //DisplayBinaryChoice();
        OnLoudLongPressedEvent?.Invoke();
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
        //DisplayMainChoice();
        OnBinaryChoiceEvent?.Invoke(true);
    }
    public static void OnNo()
    {
        print("No");
        //DisplayMainChoice();
        OnBinaryChoiceEvent?.Invoke(false);
    }

    // Roll Result
    public static void setTxtRolls(int[] rolls, int total)
    {
        TMP_Text txtDiceRollResult = butRollResult.transform.Find("txtDiceRollResult")?.GetComponent<TMP_Text>();
        TMP_Text txtTotalResult = butRollResult.transform.Find("txtTotalResult")?.GetComponent<TMP_Text>();

        string rollsText = string.Join(", ", rolls);
        txtDiceRollResult.text = $"Dice Rolls: {rollsText}";
        txtTotalResult.text = $"Total: {total}";

    }
    public static void showRollResult()
    {
        butRollResult.gameObject.SetActive(true);
    }
    public static void OnRollResultPressed()
    {
        print("Roll Result Pressed (View)");
        butRollResult.gameObject.SetActive(false);
        OnRollResultContinueEvent?.Invoke();
    }
}
