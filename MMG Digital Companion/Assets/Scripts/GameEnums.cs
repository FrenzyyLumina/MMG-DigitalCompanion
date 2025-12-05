using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEnums
{
    public enum HealthState
    {
        Normal,
        Wounded,
        Dead
    }

    public enum ActionState
    {
        Normal,
        Stunned,
        CompletingObjective,
        Skipped,
    }

    public enum Role
    {
        Unknown,
        Gent,
        Soldier,
        Thief,
        Double_Agent,
    }

    public enum Movement
    {
        None,
        Soft,
        Loud
    }

    public enum CQCResult
    {
        Winner,
        Loser,
        Tie
    }

    public enum ItemUseType
    {
        Passive,
        TurnUsable,
        CqcUsable,
        RollUsable
    }

    public enum SquareType
    {
        Empty,
        Objective,
        Snitching,
        Armory,
        McGuffin,
        Spawn,
        Trap,
    }
    public enum Items
    {
        None,
        Dice,
    }
}
