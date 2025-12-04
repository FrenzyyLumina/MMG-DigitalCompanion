using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    //Helper Functions
    private void handleCqcTarget(int targetIdx)
    {
        GameView.OnPlayerTargetedEvent -= handleCqcTarget;
        print($"CQC Chosen: {targetIdx}");

        Player targetPlr = GameModel.getPlayerByIdx(targetIdx);
        GameEnums.HealthState curState = targetPlr.getHealthState();
        
        switch (curState)
        {
            case GameEnums.HealthState.Normal:
                targetPlr.setState(GameEnums.HealthState.Wounded);
                break;

            case GameEnums.HealthState.Wounded:
                targetPlr.setState(GameEnums.HealthState.Dead);
                break;
            default:
                print($"Invalid Case: targetted a player with state: {curState}");
                break;

        }

        GameView.OnTurnEnd();
    }
    private void handlePostMoveEvent(bool yes)
    {
        GameView.OnBinaryChoiceEvent -= handlePostMoveEvent;
        print($"Choice selected: {yes}");

        if (!yes) GameView.OnTurnEnd();

        GameView.OnPlayerTargetedEvent += handleCqcTarget;
        GameView.DisplayTargetChoiceWithoutOne(
            GameModel.getTotalPlayers(),
            GameModel.getCurrentPlrIdx()
        );
    }
    private void handleMoveRollResult()
    {
        GameView.OnRollResultContinueEvent -= handleMoveRollResult;

        print("Player wants to continue");
        GameView.SetBinaryPrompt("Did you engage CQC with another player?");

        GameView.OnBinaryChoiceEvent += handlePostMoveEvent;
        GameView.DisplayBinaryChoice();
    }
    private void handleGeneralMove(int baseCount, bool isLoudShort)
    {
        int extraDiceUsed = 0; //TODO: Get the value from somewhere
        int dicesToUsed = baseCount + extraDiceUsed;

        Player curPlayer = GameModel.getCurrentPlayerToAct();
        Inventory inv = curPlayer.getInventory();
        for (int i = 0; i < extraDiceUsed; i++)
        {
            inv.removeItemByName("Dice");
        }

        if (isLoudShort)
        {
            Item newDice = new Item("Dice", GameEnums.ItemUseType.TurnUsable);
            inv.addItem(newDice);
        }

        int[] baseRoles = GameModel.rollD6Dices(dicesToUsed);
        int rollTotal = GameModel.totalFromDiceRolls(baseRoles);
        GameView.setTxtRolls(baseRoles, rollTotal);

        GameView.OnRollResultContinueEvent += handleMoveRollResult;
        GameView.showRollResult();
    }
    //End of Helper Methods
    private IEnumerator handleCurrentPlayer()
    {
        Player curPlayer = GameModel.getCurrentPlayerToAct();
        GameEnums.HealthState healthState = curPlayer.getHealthState();
        GameEnums.ActionState actionState = curPlayer.getActionState();

        //Auto skip if player is dead
        if (healthState == GameEnums.HealthState.Dead) yield break;

        //TODO: Properly prompt stunned / turn skip
        if (actionState == GameEnums.ActionState.Stunned)
        {
            //TODO: Disable all main choice actions
            GameView.DisplayMainChoice();
            yield return GameView.WaitForTurnEnd();
            curPlayer.setActionState(GameEnums.ActionState.Normal);
            yield break;
        }
        
        void handleSoft()
        {
            print("Handle Soft Triggered");
            int BASE_COUNT = 1;
            handleGeneralMove(BASE_COUNT, false);
        }
        void handleLoudShort()
        {
            print("Handle Loud Short Triggered");
            int BASE_COUNT = 1;
            handleGeneralMove(BASE_COUNT, true);
        }
        void handleLoudLong()
        {
            print("Handle Loud Long Triggered");
            int BASE_COUNT = 2;
            handleGeneralMove(BASE_COUNT, false);
        }

        GameView.OnSoftPressedEvent         += handleSoft;
        GameView.OnLoudShortPressedEvent    += handleLoudShort;
        GameView.OnLoudLongPressedEvent     += handleLoudLong;

        
        GameView.DisplayMainChoice();
        yield return GameView.WaitForTurnEnd();
        
        GameView.OnSoftPressedEvent         -= handleSoft;
        GameView.OnLoudShortPressedEvent    -= handleLoudShort;
        GameView.OnLoudLongPressedEvent     -= handleLoudLong;
    }

    private IEnumerator GameLoop()
    {
        // Get player data from GameManager
        //int totalPlayers = GameManager.Instance.TotalPlayers;
        int totalPlayers = 2;
        GameModel.setTotalPlayers(totalPlayers);

        //Initialize
        /*
        Player[] players = GameModel.getPlayers();
        for (int i = 0; i < totalPlayers; i++)
        {
            // Set the role from the scanned QR codes
            players[i].setRole(GameManager.Instance.PlayerRoles[i]);
            Debug.Log($"Player {i + 1} initialized with role: {GameManager.Instance.PlayerRoles[i]}");
        }
        */

        //Game Start
        //Roles already scanned in GameStart scene
        
        //Start of actual game loop
        while(!GameModel.checkForWinner())
        {
            GameView.SetTurnCount(GameModel.getCurrentTurn());
            GameView.SetCurrentPlayer(GameModel.getCurrentPlrIdx() + 1);
            yield return StartCoroutine(handleCurrentPlayer());
            GameModel.moveToNextPlayer();
        }

        print("We have a winner!!");
        //Player winner = GameModel.getWinner();
        int winnerIdx = GameModel.getIdxOfWinner();

        print($"Player who won: {winnerIdx + 1}");
        GameView.setTxtWinner(winnerIdx);
        GameView.DisplayWinner();
    }

    private void Start()
    {
        print("GameMain is running...");
        StartCoroutine(GameLoop());
    }
}
