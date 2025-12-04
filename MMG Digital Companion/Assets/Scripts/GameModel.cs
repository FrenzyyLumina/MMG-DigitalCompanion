using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    private static Player[] Players;
    private static int TotalPlayers = 2;
    private static int CurrentPlayerIdx = 0;
    private static int CurrentTurn = 1;
    private static Player winner;

    // Getters and Setters
    public static Player[] getPlayers()
    {
        return Players;
    }

    public static int getTotalPlayers()
    {
        return TotalPlayers;
    }

    public static void setTotalPlayers(int total)
    {
        TotalPlayers = total;
        Players = new Player[total];
        for (int i = 0; i < total; i++)
        {
            Players[i] = new Player();
        }
    }
    public static int getCurrentTurn()
    {
        return CurrentTurn;
    }

    //Handles turns
    public static Player getCurrentPlayerToAct()
    {
        return Players[CurrentPlayerIdx];
    }
    public static void moveToNextPlayer()
    {
        CurrentPlayerIdx++;
        if (CurrentPlayerIdx < TotalPlayers) return;

        CurrentPlayerIdx = 0;
        CurrentTurn++;
    }

    //Rolls numDice amount of d6
    public static int[] rollD6Dices(int numDice)
    {
        int[] rolls = new int[numDice];

        for (int i = 0; i < numDice; i++)
        {
            rolls[i] = Random.Range(1, 7); //Note: Upperbound is exclusive
        }

        return rolls;
    }
    public static int totalFromDiceRolls(int[] rolls)
    {
        int total = 0;

        for (int i = 0; i < rolls.Length; i++)
        {
            total += rolls[i];
        }

        return total;
    }

    public static bool checkForWinner()
    {
        if (winner != null) return true;

        //Check if there's 1 player remaining
        Player AlivePlayer = null;
        for (int i = 0; i < TotalPlayers; i++)
        {
            GameEnums.HealthState healthState = Players[i].getHealthState();
            if (healthState == GameEnums.HealthState.Dead) continue;
            if (AlivePlayer != null) return false; //there's no winner if >2 alive players
            AlivePlayer = Players[i];
        }

        if (AlivePlayer != null)
        {
            winner = AlivePlayer;
            return true;
        }

        return false;
    }
}
