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
        Skipped,
    }

    public enum Role
    {
        Unknown,
        Gent,
        Soldier,
        Thief,
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
    }

    public enum SquareType
    {
        Empty,
        Room,
        Objective,
        Snitching,
        Armory,
        McGuffin,
        Trap,
    }
}
