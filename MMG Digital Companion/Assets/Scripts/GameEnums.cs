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
        Gent,
        Soldier,
        Thief,
    }
    public enum ItemUseType
    {
        Passive,
        TurnUsable,
        CqcUsable,
    }
}
