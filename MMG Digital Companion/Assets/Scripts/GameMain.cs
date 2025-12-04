using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private IEnumerator handleCurrentPlayer(int idx)
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
            //TODO: Wait for turn skip
            yield return GameView.WaitForTurnEnd();
            curPlayer.setActionState(GameEnums.ActionState.Normal);
            yield break;
        }

        GameView.DisplayMainChoice();
        //TODO: Wait for turn end
        yield return GameView.WaitForTurnEnd();
    }

    private void StartGame()
    {
        //Initialize
        int nTotalPlrs = 2; //TODO: Change this
        GameModel.setTotalPlayers(nTotalPlrs);
        GameModel.setPlayers(new Player[nTotalPlrs]);

        //Game Start
        //TODO: Let Players scan roles
        
        //Start of actual game loop
        //TODO: handle player turn
        while(!GameModel.checkForWinner())
        {
            GameView.SetTurnCount(GameModel.getCurrentTurn());

            handleCurrentPlayer();
            GameModel.moveToNextPlayer();
        }
    }
}
