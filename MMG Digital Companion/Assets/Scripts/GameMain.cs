using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private Player[] Players;
    private int TotalPlayers;
    private int CurrentPlayerIdx = 0;
    private int CurrentTurn = 1;
    private Player winner;

    //Rolls numDice amount of d6
    private int[] rollD6Dices(int numDice)
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

        //TODO: addListeners
        //Listener for Move
        void handleSoft()
        {
            print("Handle Soft Triggered");
            int BASE_COUNT = 1;
            int extraDiceUsed = 0; //TODO: Get the value from somewhere
            int dicesToUsed = BASE_COUNT + extraDiceUsed;

            for(int i = 0; i < extraDiceUsed; i++)
            {
                curPlayer.getInventory().removeItemByName("Dice");
            }

            int[] baseRoles = GameModel.rollD6Dices(dicesToUsed);
            int rollTotal = GameModel.totalFromDiceRolls(baseRoles);
            GameView.setTxtRolls(baseRoles, rollTotal);
            
            GameView.OnRollResultContinueEvent += handleMoveRollResult;
            GameView.showRollResult();
        }
        void handleLoudShort()
        {
            //TODO: Copy paste from above
        }
        void handleLoudLong()
        {
            //TODO: Copy paste from above
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
        TotalPlayers = GameManager.Instance.TotalPlayers;
        
        //Initialize
        this.Players = new Player[this.TotalPlayers];
        for (int i = 0; i < this.TotalPlayers; i++)
        {
            this.Players[i] = new Player();
            // Set the role from the scanned QR codes
            this.Players[i].setRole(GameManager.Instance.PlayerRoles[i]);
            Debug.Log($"Player {i + 1} initialized with role: {GameManager.Instance.PlayerRoles[i]}");
        }

        //Game Start
        //Roles already scanned in GameStart scene
        
        //Start of actual game loop
        //TODO: handle player turn
        while(!GameModel.checkForWinner())
        {
            GameView.SetTurnCount(GameModel.getCurrentTurn());
            GameView.SetCurrentPlayer(GameModel.getCurrentPlrIdx() + 1);
            yield return StartCoroutine(handleCurrentPlayer());
            GameModel.moveToNextPlayer();
        }

        print("We have a winner!!");
        print($"Player who won: {GameModel.getCurrentPlrIdx() + 1}");
    }

    private void Start()
    {
        print("GameMain is running...");
        StartCoroutine(GameLoop());
    }
}
