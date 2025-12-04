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
        int[] rolls = new int[numDice];

        for (int i = 0; i < numDice; i++)
        {
            rolls[i] = Random.Range(1, 6);
        }

        return rolls;
    }

    private bool checkForWinner()
    {
        if (this.winner != null) return true;

        //Check if there's 1 player remaining
        Player AlivePlayer = null;
        for (int i = 0; i < this.TotalPlayers; i++)
        {
            GameEnums.HealthState healthState = this.Players[i].getHealthState();
            if (healthState == GameEnums.HealthState.Dead) continue;
            if (AlivePlayer != null) return false; //there's no winner if >2 alive players
            AlivePlayer = this.Players[i];
        }

        if (AlivePlayer != null)
        {
            this.winner = AlivePlayer;
            return true;
        }

        return false;
    }

    private void handleCurrentPlayer(int idx)
    {
        Player curPlayer = Players[idx];
        GameEnums.HealthState healthState = curPlayer.getHealthState();
        GameEnums.ActionState actionState = curPlayer.getActionState();

        //Auto skip if player is dead
        if (healthState == GameEnums.HealthState.Dead) return;

        //TODO: Properly prompt stunned / turn skip
        if (actionState == GameEnums.ActionState.Stunned)
        {
            //TODO: Disable all main choice actions
            GameView.DisplayMainChoice();
            //TODO: Wait for turn skip
            curPlayer.setActionState(GameEnums.ActionState.Normal);
            return;
        }

        GameView.DisplayMainChoice();
        //TODO: Wait for turn end
    }

    private void StartGame()
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
        while(!checkForWinner())
        {
            GameView.SetTurnCount(this.CurrentTurn);

            handleCurrentPlayer(this.CurrentPlayerIdx);
            this.CurrentPlayerIdx++;
            if (this.CurrentPlayerIdx == this.TotalPlayers)
            {
                this.CurrentPlayerIdx = 0;
                this.CurrentTurn++;
            }
        }
    }
}
