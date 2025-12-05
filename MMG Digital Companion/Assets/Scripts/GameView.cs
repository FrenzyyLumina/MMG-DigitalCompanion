using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] GameObject _Background;
    [SerializeField] GameObject _SwingPanel;
    [SerializeField] GameObject _containerMainChoice;
    [SerializeField] GameObject _containerMoveChoice;
    [SerializeField] GameObject _containerTargetChoice;
    [SerializeField] GameObject _containerBinaryChoice;
    [SerializeField] GameObject _panelTrapPrompt;
    [SerializeField] GameObject _panelInventory;
    [SerializeField] TMP_Text _txtTurnCount;
    [SerializeField] TMP_Text _txtCurrentPlayer;
    [SerializeField] TMP_Text _txtBinaryPrompt;
    [SerializeField] TMP_Text _txtWinner;
    [SerializeField] Button _butSkipTurn;
    [SerializeField] Button _butRollResult;
    [SerializeField] Button _butWinnerResult;
    [SerializeField] Button _butCqcResult;
    [SerializeField] Button _butItemTemplate;

    private static GameObject Background;
    private static GameObject SwingPanel;
    private static GameObject containerMainChoice;
    private static GameObject containerMoveChoice;
    private static GameObject containerTargetChoice;
    private static GameObject containerBinaryChoice;
    private static GameObject panelTrapPrompt;
    private static GameObject panelInventory;
    private static TMP_Text txtTurnCount;
    private static TMP_Text txtCurrentPlayer;
    private static TMP_Text txtBinaryPrompt;
    private static TMP_Text txtWinner;
    private static Button butSkipTurn;
    private static Button butRollResult;
    private static Button butWinnerResult;
    private static Button butCqcResult;
    private static Button butItemTemplate;

    private static event Action OnAnyTurnEnd;

    public static event Action OnMovePressedEvent;
    public static event Action OnUseItemPressedEvent;
    public static event Action OnCompleteObjectivePressedEvent;
    public static event Action OnSkipTurnPressedEvent;
    public static event Action OnSoftPressedEvent;
    public static event Action OnLoudShortPressedEvent;
    public static event Action OnLoudLongPressedEvent;
    public static event Action<int> OnPlayerTargetedEvent; // Player Idx as parameter
    public static event Action<bool> OnBinaryChoiceEvent; // true for Yes, false for No
    public static event Action OnRollResultContinueEvent;
    public static event Action OnCqcResultContinueEvent;
    public static event Action OnTrapSpawnRerollEvent;
    public static event Action OnSnitchEvent;

    public static List<Button> AddedButtons;

    private void Awake()
    {
        // Copy non-static references to static fields
        Background              = _Background;
        SwingPanel              = _SwingPanel;
        containerMainChoice     = _containerMainChoice;
        containerMoveChoice     = _containerMoveChoice;
        containerTargetChoice   = _containerTargetChoice;
        containerBinaryChoice   = _containerBinaryChoice;
        panelTrapPrompt         = _panelTrapPrompt;
        panelInventory          = _panelInventory;
        txtTurnCount            = _txtTurnCount;
        txtCurrentPlayer        = _txtCurrentPlayer;
        txtBinaryPrompt         = _txtBinaryPrompt;
        txtWinner               = _txtWinner;
        butSkipTurn             = _butSkipTurn;
        butRollResult           = _butRollResult;
        butWinnerResult         = _butWinnerResult;
        butCqcResult            = _butCqcResult;
        butItemTemplate         = _butItemTemplate;
    }

    // Basic Features
    public static void SetTurnCount(int turnCount)
    {
        //txtTurnCount.GetComponent<UnityEngine.UI.Text>().text = "Turn: " + turnCount.ToString();
        txtTurnCount.GetComponent<TMP_Text>().text = "Round: " + turnCount.ToString();
    }
    private static void SetBackgroundColorFromPlrNum(int plrNum)
    {
        Color newColor = new Color();
        switch (plrNum)
        {
            case 1: newColor = Color.red; break;
            case 2: newColor = Color.blue; break;
            case 3: newColor = Color.green; break;
            case 4: newColor = Color.yellow; break;
        }


        Background.GetComponent<Image>().color = newColor;
    }
    public static void SetCurrentPlayer(int plrNumber)
    {
        txtCurrentPlayer.text = $"Player {plrNumber}'s Turn";
        SetBackgroundColorFromPlrNum (plrNumber);
    }
    public static void SetBinaryPrompt(string prompt)
    {
        txtBinaryPrompt.GetComponent<TMP_Text>().text = prompt;
    }
    public static void setTxtWinner(int plrNumber)
    {
        txtWinner.text = $"Player: {plrNumber}";
    }
    public static void setTxtRolls(int[] rolls, int total)
    {
        TMP_Text txtDiceRollResult = butRollResult.transform.Find("txtDiceRollResult")?.GetComponent<TMP_Text>();
        TMP_Text txtTotalResult = butRollResult.transform.Find("txtTotalResult")?.GetComponent<TMP_Text>();

        string rollsText = string.Join(", ", rolls);
        txtDiceRollResult.text = $"Dice Rolls: {rollsText}";
        txtTotalResult.text = $"Total: {total}";

    }
    public static void setCqcRolls(int atkrIdx, int[] atkrRolls, int atkrTotal, int atkeIdx,  int[] atkeRolls, int atkeTotal)
    {
        Transform attackerObj = butCqcResult.transform.Find("Attacker");
        Transform attackeeObj = butCqcResult.transform.Find("Attackee");

        void setInfo(Transform obj, int idx, int[] rolls, int total, string prefix, GameEnums.CQCResult cqcResult)
        {
            TMP_Text txtPlayer = obj.transform.Find("txtPlayer").GetComponent<TMP_Text>();
            TMP_Text txtDiceRollResult = obj.transform.Find("txtDiceRollResult")?.GetComponent<TMP_Text>();
            TMP_Text txtTotalResult = obj.transform.Find("txtTotalResult")?.GetComponent<TMP_Text>();
            TMP_Text txtCqcResult = obj.transform.Find("txtCqcResult")?.GetComponent <TMP_Text>();

            txtPlayer.text = $"{prefix}\nPlayer {idx + 1}";
            string rollsText = string.Join(", ", rolls);
            txtDiceRollResult.text = $"Dice Rolls: {rollsText}";
            txtTotalResult.text = $"Total: {total}";

            switch (cqcResult)
            {
                case GameEnums.CQCResult.Winner:
                    txtCqcResult.text = "Winner!";
                    break;
                case GameEnums.CQCResult.Loser:
                    txtCqcResult.text = "Loser!";
                    break;
                case GameEnums.CQCResult.Tie:
                    txtCqcResult.text = "Tie!";
                    break;
            }
        }

        bool isTie = atkrTotal == atkeTotal;
        setInfo(attackerObj, atkrIdx, atkrRolls, atkrTotal, "Attacker",
            isTie? GameEnums.CQCResult.Tie :
            atkrTotal > atkeTotal? GameEnums.CQCResult.Winner :
            GameEnums.CQCResult.Loser
        );
        setInfo(attackeeObj, atkeIdx, atkeRolls, atkeTotal, "Attackee",
            isTie? GameEnums.CQCResult.Tie :
            atkeTotal > atkrTotal? GameEnums.CQCResult.Winner :
            GameEnums.CQCResult.Loser
        );
    }
    public static void updateSwingPanel(Player[] players)
    {
        Transform[] plrPanels = new Transform[4];
        plrPanels[0] = SwingPanel.transform.Find("Player_1");
        plrPanels[1] = SwingPanel.transform.Find("Player_2");
        plrPanels[2] = SwingPanel.transform.Find("Player_3");
        plrPanels[3] = SwingPanel.transform.Find("Player_4");

        for (int i = 0; i < plrPanels.Length; i++)
        {
            Transform curPanel = plrPanels[i];

            if (i >= players.Length)
            {
                curPanel.gameObject.SetActive(false);
                continue;
            }

            curPanel.gameObject.SetActive(true);
            TMP_Text txtPlr     = curPanel.transform.Find("txtPlayer").GetComponent<TMP_Text>();
            TMP_Text txtRole    = curPanel.transform.Find("txtRole").GetComponent<TMP_Text>();
            TMP_Text txtHealth  = curPanel.transform.Find("txtHealth").GetComponent<TMP_Text>();
            TMP_Text txtStatus  = curPanel.transform.Find("txtStatus").GetComponent<TMP_Text>();
            
            Player curPlr = players[i];
            txtPlr.text = $"Player {i + 1}";
            txtRole.text = $"{curPlr.getPlayerRole()}";
            txtHealth.text = $"{curPlr.getHealthState()}";
            txtStatus.text = $"{curPlr.getActionState()}";
        }
    }
    
    public static void setTrapCoord(int x, int y)
    {
        panelTrapPrompt.transform.Find("txtCoord").GetComponent<TMP_Text>().text = $"{(char)('a' + x)}{y + 1}";
    }

    public static void setMainChoiceButtonsInteractable(bool state)
    {
        containerMainChoice.transform.Find("butMove").GetComponent<Button>().interactable = state;
        containerMainChoice.transform.Find("butItem").GetComponent<Button>().interactable = state;
        containerMainChoice.transform.Find("butObjective").GetComponent<Button>().interactable = state;
        containerMainChoice.transform.Find("butSnitch").GetComponent<Button>().interactable = state;
    }
    public static void setInventory(Inventory inv)
    {
        //Remove old buttons
        foreach(Button but in AddedButtons)
        {
            if (but != null)
                Destroy(but);
        }
        AddedButtons.Clear();

        foreach(Item item in inv.getItems())
        {
            Button newButton = Instantiate(butItemTemplate, panelInventory.transform);
            newButton.gameObject.SetActive(true);

            TMP_Text buttonTextComponent = newButton.GetComponentInChildren<TMP_Text>();
            buttonTextComponent.text = $"x{item.getCount()}";
            //newButton.onClick.AddListener(() => On);
            AddedButtons.Add(newButton);
        }
    }

    //Display methods
    public static void DisplayMainChoice()
    {
        containerMainChoice.SetActive(true);
        containerMoveChoice.SetActive(false);
        containerTargetChoice.SetActive(false);
        containerBinaryChoice.SetActive(false);
    }
    public static void DisplayTargetChoiceWithoutOne(int numPlrs, int plrIdx)
    {
        Transform butContainer = containerTargetChoice.transform.Find("PlayerChoice");

        // Get all buttons in the container
        Button[] playerButtons = butContainer.GetComponentsInChildren<Button>(true);

        // Make sure we have enough buttons
        int i = 0;
        foreach (Button btn in playerButtons)
        {
            btn.gameObject.SetActive(false);
            if (i < numPlrs && i != plrIdx)
            {
                btn.gameObject.SetActive(true);
            }

            i++;
        }

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
    public static void DisplayWinner()
    {
        butWinnerResult.gameObject.SetActive(true);
    }
    public static void showRollResult()
    {
        butRollResult.gameObject.SetActive(true);
    }

    public static void showCqcRollResult()
    {
        butCqcResult.gameObject.SetActive(true);
    }
    public static void showTrapPrompt()
    {
        panelTrapPrompt.gameObject.SetActive(true);
    }

    //=====PRESS-EVENTS======
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
        OnTurnEnd();
    }
    public static void OnSkipTurnPressed()
    {
        print("Skip Turn Pressed");
        OnSkipTurnPressedEvent?.Invoke();
        OnTurnEnd();
    }
    public static void OnSnitch()
    {
        OnSnitchEvent?.Invoke();
        OnTurnEnd();
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
        OnPlayerTargetedEvent?.Invoke(0);
    }
    public static void OnPlayerBPressed()
    {
        print("Player B Targeted");
        OnPlayerTargetedEvent?.Invoke(1);
    }
    public static void OnPlayerCPressed()
    {
        print("Player C Targeted");
        OnPlayerTargetedEvent?.Invoke(2);
    }
    public static void OnPlayerDPressed()
    {
        print("Player D Targeted");
        OnPlayerTargetedEvent?.Invoke(3);
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
    public static void OnRollResultPressed()
    {
        print("Roll Result Pressed (View)");
        butRollResult.gameObject.SetActive(false);
        OnRollResultContinueEvent?.Invoke();
    }

    public static void OnCqcResultPressed()
    {
        print("Cqc Result Pressed");
        butCqcResult.gameObject.SetActive(false);
        OnCqcResultContinueEvent?.Invoke();
    }
    
    //Trap spawn
    public static void OnTrapSpawnReroll()
    {
        OnTrapSpawnRerollEvent?.Invoke();
    }
    public static void OnTrapSpawnContinue()
    {
        panelTrapPrompt.gameObject.SetActive(false);
    }
}
