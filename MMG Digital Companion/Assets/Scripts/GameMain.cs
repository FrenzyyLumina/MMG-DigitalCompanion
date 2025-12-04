using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    //Helper Functions
    private void handleRerollTrapPrompt()
    {
        int[] coord = GameModel.spawnTrapAtRandomPos();
        GameView.setTrapCoord(coord[0], coord[1]);
        GameView.showTrapPrompt();
    }

    private void handleTargetPostMove(int targetIdx)
    {
        GameView.OnPlayerTargetedEvent -= handleTargetPostMove;
        print($"CQC Chosen: {targetIdx}");

        //Auto damage if soft
        if (GameModel.getMovement() == GameEnums.Movement.Soft)
        {
            GameModel.damagePlayer(targetIdx);
            GameView.OnTurnEnd();
            return;
        }

        //Perform cqc roll
        //TODO: Modify to handle extra rolls
        int[] attackerRolls = GameModel.rollD6Dices(1);
        int[] attackeeRolls = GameModel.rollD6Dices(1);
        int attackerTotal = GameModel.totalFromDiceRolls(attackerRolls);
        int attackeeTotal = GameModel.totalFromDiceRolls(attackeeRolls);

        void handle()
        {
            GameView.OnCqcResultContinueEvent -= handle;

            if (attackerTotal > attackeeTotal)
                GameModel.damagePlayer(targetIdx);
            else if (attackerTotal < attackeeTotal)
                GameModel.damagePlayer(GameModel.getCurrentPlrIdx());

            GameView.OnTurnEnd();
        }

        GameView.setCqcRolls(
            GameModel.getCurrentPlrIdx(), attackerRolls, attackerTotal,
            targetIdx, attackeeRolls, attackeeTotal
        );
        GameView.OnCqcResultContinueEvent += handle;
        GameView.showCqcRollResult();
    }
    private void handlePostMoveEvent(bool yes)
    {
        GameView.OnBinaryChoiceEvent -= handlePostMoveEvent;
        print($"Choice selected: {yes}");

        if (!yes) GameView.OnTurnEnd();

        GameView.OnPlayerTargetedEvent += handleTargetPostMove;
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
    private void handleOnCompleteObjective() => GameModel.getCurrentPlayerToAct().setActionState(GameEnums.ActionState.CompletingObjective);
    private void handleOnSnitch() => GameModel.attemptSnitch(GameModel.getCurrentPlrIdx());
    
    //End of Helper Methods
    private IEnumerator handleCurrentPlayer()
    {
        Player curPlayer = GameModel.getCurrentPlayerToAct();
        GameEnums.HealthState healthState = curPlayer.getHealthState();
        GameEnums.ActionState actionState = curPlayer.getActionState();

        //Auto skip if player is dead
        if (healthState == GameEnums.HealthState.Dead) yield break;

        //TODO: Properly prompt stunned / turn skip
        if (actionState == GameEnums.ActionState.Stunned || actionState == GameEnums.ActionState.CompletingObjective)
        {
            //TODO: Disable all main choice actions
            GameView.setMainChoiceButtonsInteractable(false);
            GameView.DisplayMainChoice();
            yield return GameView.WaitForTurnEnd();
            curPlayer.setActionState(GameEnums.ActionState.Normal);
            yield break;
        }
        
        //TODO: Add listener for complete objective

        void handleSoft()
        {
            print("Handle Soft Triggered");
            int BASE_COUNT = 1;
            GameModel.setMovement(GameEnums.Movement.Soft);
            handleGeneralMove(BASE_COUNT, false);
        }
        void handleLoudShort()
        {
            print("Handle Loud Short Triggered");
            int BASE_COUNT = 1;
            GameModel.setMovement(GameEnums.Movement.Loud);
            handleGeneralMove(BASE_COUNT, true);
        }
        void handleLoudLong()
        {
            print("Handle Loud Long Triggered");
            int BASE_COUNT = 2;
            GameModel.setMovement(GameEnums.Movement.Loud);
            handleGeneralMove(BASE_COUNT, false);
        }

        GameView.OnSoftPressedEvent         += handleSoft;
        GameView.OnLoudShortPressedEvent    += handleLoudShort;
        GameView.OnLoudLongPressedEvent     += handleLoudLong;

        GameView.setMainChoiceButtonsInteractable(true);
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
        int totalPlayers = 3;
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
        
        GameView.OnTrapSpawnRerollEvent += handleRerollTrapPrompt;
        GameView.OnSnitchEvent += handleOnSnitch;
        GameView.OnCompleteObjectivePressedEvent += handleOnCompleteObjective;

        //Start of actual game loop
        while (!GameModel.checkForWinner())
        {
            GameView.updateSwingPanel(GameModel.getPlayers());
            GameView.SetTurnCount(GameModel.getCurrentTurn());
            GameView.SetCurrentPlayer(GameModel.getCurrentPlrIdx() + 1);
            yield return StartCoroutine(handleCurrentPlayer());

            if (GameModel.getCurrentPlrIdx() == totalPlayers - 1)
            {   
                //TODO: if it is, Give items to all players and spawn a trap
                handleRerollTrapPrompt();
            }

            GameModel.moveToNextPlayer();
        }

        print("We have a winner!!");
        //Player winner = GameModel.getWinner();
        int winnerIdx = GameModel.getIdxOfWinner();

        print($"Player who won: {winnerIdx + 1}");
        GameView.setTxtWinner(winnerIdx + 1);
        GameView.DisplayWinner();

        GameView.OnTrapSpawnRerollEvent -= handleRerollTrapPrompt;
        GameView.OnSnitchEvent -= handleOnSnitch;
        GameView.OnCompleteObjectivePressedEvent -= handleOnCompleteObjective;
    }

    private void Start()
    {
        print("GameMain is running...");
        GameModel.Reset();
        StartCoroutine(GameLoop());
    }
}
