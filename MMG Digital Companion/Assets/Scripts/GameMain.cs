using System;
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
            //Item: Personal Radar
            Inventory inv = GameModel.getPlayerByIdx(targetIdx).getInventory();
            if (inv.findItemByItemType(GameEnums.Item.Personal_Radar) == null)
                GameModel.damagePlayer(targetIdx);
            else
                inv.removeItemByItemType(GameEnums.Item.Personal_Radar);

            GameView.OnTurnEnd();
            return;
        }

        //Perform cqc roll
        //TODO: Modify to handle extra rolls
        int[] attackerRolls = GameModel.rollD6Dices(1);
        int[] attackeeRolls = GameModel.rollD6Dices(1);
        
        // Apply role modifiers for CQC
        Player attacker = GameModel.getCurrentPlayerToAct();
        Player defender = GameModel.getPlayerByIdx(targetIdx);
        
        int attackerTotal = GameModel.totalFromDiceRolls(attackerRolls) + attacker.GetCQCModifier();
        int attackeeTotal = GameModel.totalFromDiceRolls(attackeeRolls) + defender.GetCQCModifier();

        void handle()
        {
            GameView.OnCqcResultContinueEvent -= handle;
            int idxToDamage = -1;

            if (attackerTotal > attackeeTotal)
                idxToDamage = targetIdx;
            else if (attackerTotal < attackeeTotal)
                idxToDamage = GameModel.getCurrentPlrIdx();

            //Item: Body Armor
            if (idxToDamage != -1)
            {
                Inventory inv = GameModel.getPlayerByIdx(idxToDamage).getInventory();
                if (inv.findItemByItemType(GameEnums.Item.Body_Armor) == null)
                    GameModel.damagePlayer(idxToDamage);
                else
                    inv.removeItemByItemType(GameEnums.Item.Body_Armor);
            }

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
            inv.removeItemByName(GameEnums.Item.Dice.ToString());
        }

        if (isLoudShort)
        {
            Item newDice = new Item(GameEnums.Item.Dice, GameEnums.ItemUseType.TurnUsable);
            inv.addItem(newDice);
        }

        int[] baseRoles = GameModel.rollD6Dices(dicesToUsed);
        int rollTotal = GameModel.totalFromDiceRolls(baseRoles);
        
        // The Gent: Extra d6 if both loud dice show same value
        if (curPlayer.CanGentReroll(baseRoles) && !isLoudShort)
        {
            int[] bonusRoll = GameModel.rollD6Dices(1);
            rollTotal += bonusRoll[0];
            Debug.Log($"The Gent bonus roll: {bonusRoll[0]}. New total: {rollTotal}");
        }
        
        // Apply movement modifiers (Assassin)
        rollTotal += curPlayer.GetMovementModifier(GameModel.getMovement());
        
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
        //DEBUG: Change this after testing
        //int totalPlayers = 2; //Debug
        int totalPlayers = GameManager.Instance.TotalPlayers;
        Debug.Log($"GameMain: Using {totalPlayers} players from GameManager");
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
            GameView.setInventory(GameModel.getCurrentPlayerToAct().getInventory());

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
