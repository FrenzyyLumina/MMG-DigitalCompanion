using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static GameEnums;

public class GameModel : MonoBehaviour
{
    private static Player[] Players;
    private static int TotalPlayers = 2;
    private static int CurrentPlayerIdx;
    private static int CurrentTurn;
    private static Player winner;
    private static GameEnums.Movement curMovement;
    //private static SquareType[,] Board;
    private static Square[,] Board;
    private static int numObjectivesDone;
    private static int SnitchValue;

    private const int BOARD_SIZE = 6;
    private const int NUM_ROOMS = 7;
    private const int NUM_OBJECTIVES = 3;
    private static int[,] SPAWN_COORDS = new int[4, 2] {
        {0, 0},
        {0, BOARD_SIZE - 1},
        {BOARD_SIZE - 1, 0},
        {BOARD_SIZE - 1, BOARD_SIZE - 1}
    };
    private static int[,] MCGUFFIN_COORDS = new int[4, 2]
    {
        {2, 2},
        {2, 3},
        {3, 2},
        {3, 3}
    };
    private static int[,] ROOM_COORDS = new int[NUM_ROOMS, 2]
    {//Note: look at it as x, y
        {1, 0},
        {5, 1},
        {1, 2},
        {4, 3},
        {0, 4},
        {2, 4},
        {4, 5},
    };

    //TODO: SNITCH VALUE
    private static int[] getRandomRoomAssignments()
    {
        int[] roomId = new int[NUM_ROOMS];
        for (int i = 0; i < roomId.Length; i++)
        {
            roomId[i] = i;
        }

        for (int i = roomId.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = roomId[i];
            roomId[i] = roomId[j];
            roomId[j] = temp;
        }

        return roomId;
    }
    public static void Reset()
    {
        CurrentPlayerIdx = 0;
        CurrentTurn = 1;
        winner = null;
        numObjectivesDone = 0;
        SnitchValue = Random.Range(14, 19);

        //-2 = Space occupied by mcguffin, -1 = room, 0 = free space, 1 = trap
        Board = new Square[BOARD_SIZE, BOARD_SIZE];
        
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                Board[i, j] = new Square(i, j, SquareType.Empty);
            }
        }

        for (int i = 0; i < SPAWN_COORDS.GetLength(0); i++)
        {
            int x = SPAWN_COORDS[i, 0];
            int y = SPAWN_COORDS[i, 1];

            Board[x, y].setType(SquareType.Spawn);

            x = MCGUFFIN_COORDS[i, 0];
            y = MCGUFFIN_COORDS[i, 1];
            Board[x, y].setType(SquareType.McGuffin);
        }

        for (int i = 0; i < NUM_ROOMS; i++)
        {
            int x = ROOM_COORDS[i, 0];
            int y = ROOM_COORDS[i, 1];

            Board[x, y].setType(SquareType.Armory);
        }


        int[] roomIdxs = getRandomRoomAssignments();
        for (int i = 0; i < NUM_OBJECTIVES; i++)
        {
            int targetIdx = roomIdxs[i];
            int x = ROOM_COORDS[targetIdx, 0];
            int y = ROOM_COORDS[targetIdx, 1];

            Board[x, y].setType(SquareType.Objective);
        }

        int x2 = ROOM_COORDS[NUM_OBJECTIVES, 0];
        int y2 = ROOM_COORDS[NUM_OBJECTIVES, 1];
        Board[x2, y2].setType(SquareType.Snitching);
    }

    // Getters and Setters
    public static Player[] getPlayers()
    {
        return Players;
    }
    public static Player getPlayerByIdx(int idx)
    {
        return Players[idx];
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
    public static int getCurrentPlrIdx()
    {
        return CurrentPlayerIdx;
    }
    public static int getCurrentTurn()
    {
        return CurrentTurn;
    }
    public static Player getWinner()
    {
        return winner;
    }
    public static int getIdxOfWinner()
    {
        for (int i = 0; i < TotalPlayers; i++)
        {
            if (Players[i] == winner)
            {
                return i;
            }
        }

        return -1;
    }
    public static GameEnums.Movement getMovement()
    {
        return curMovement;
    }
    public static void setMovement(GameEnums.Movement newMovement)
    {
        curMovement = newMovement;
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
    public static void damagePlayer(int plrIdx)
    {
        Player targetPlr = getPlayerByIdx(plrIdx);
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
    }
    public static int[] spawnTrapAtRandomPos()
    {
        int x = Random.Range(0, BOARD_SIZE);
        int y = Random.Range(0, BOARD_SIZE);

        //Reroll if not empty
        if (Board[x, y].getType() != SquareType.Empty)
            return spawnTrapAtRandomPos();

        return new int[2] {x, y};
    }
    public static void attemptSnitch(int plrIdx)
    {
        int NUM_DICE = 3;
        int[] rolls = rollD6Dices(NUM_DICE);
        int total = totalFromDiceRolls(rolls);

        if (total >= SnitchValue)
        {
            winner = getPlayerByIdx(plrIdx);
        }
        else
        {
            damagePlayer(plrIdx);
            Inventory curInv = getPlayerByIdx(plrIdx).getInventory();
            for (int i = 0; i < NUM_DICE; i++)
            {
                curInv.removeItemByName("Dice");
            }
        }
    }

    public static void incObjective()
    {
        numObjectivesDone++;
    }

    public static bool hasCompletedAllObjectives()
    {
        return numObjectivesDone == NUM_OBJECTIVES;
    }
}
