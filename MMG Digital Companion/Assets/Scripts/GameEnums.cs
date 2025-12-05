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
        Assassin,
        Hacker,
        Double_Agent,
        Vengeful,
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
    public enum Item
    {
        None,

        //Mechanic Related
        Dice,
        McGuffin,

        //Defensive
        Body_Armor,
        Personal_Radar,
        Rations,
        Adrenaline,
        Active_Projectile_Dome,
        Revival_Pill,

        //Offensive
        Hush_Puppy,
        Poison_Blowdart,
        M9_Bayonet,
        Tripmine,

        //Utility
        Universal_Multi_Tool,
        Proximity_Detector,
        Truth_Serum,
        Sneaking_Suit,
        Cardboard_Box,
        Coin,
        The_Doohickey,
    }
}
