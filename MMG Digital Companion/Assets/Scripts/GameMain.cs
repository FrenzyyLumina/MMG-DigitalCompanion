using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameMain : MonoBehaviour
{
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
            //TODO: Set Text in View
            GameView.setTxtRolls(baseRoles, rollTotal);
            
            void handleRollResult()
            {
                print("Player wants to continue");
                GameView.OnRollResultContinueEvent -= handleRollResult;
            }

            GameView.OnRollResultContinueEvent += handleRollResult;
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
        //Initialize
        int nTotalPlrs = 2; //TODO: Change this
        GameModel.setTotalPlayers(nTotalPlrs);

        //Game Start
        //TODO: Let Players scan roles
        
        //Start of actual game loop
        //TODO: handle player turn
        while(!GameModel.checkForWinner())
        {
            GameView.SetTurnCount(GameModel.getCurrentTurn());
            yield return StartCoroutine(handleCurrentPlayer());
            GameModel.moveToNextPlayer();
        }
    }

    private void Start()
    {
        print("GameMain is running...");
        StartCoroutine(GameLoop());
    }
}
