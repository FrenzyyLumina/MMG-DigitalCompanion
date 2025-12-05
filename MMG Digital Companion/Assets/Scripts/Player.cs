using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private GameEnums.HealthState   healthState;
    private GameEnums.Role role;
    private GameEnums.ActionState   action;

    private readonly Inventory inventory;
    private readonly Inventory itemUsedInv;
    private List<float> modifiers; //Multiplicative
    private bool isRoleRevealed;
    private bool isInRoom;
    private bool isDoingObjective;

    public Player(/*GameEnums.Role role*/)
    {
        this.healthState = GameEnums.HealthState.Normal;
        this.role = GameEnums.Role.Unknown;
        this.action = GameEnums.ActionState.Normal;
        
        this.inventory = new Inventory();
        this.itemUsedInv = new Inventory();
        this.modifiers = new List<float>();
        this.isRoleRevealed = false;
    }

    public GameEnums.HealthState getHealthState()
    {
        return this.healthState;
    }
    public GameEnums.Role getPlayerRole()
    {
        return this.role;
    }
    public GameEnums.ActionState getActionState()
    {
        return this.action;
    }
    public Inventory getInventory()
    {
        return this.inventory;
    }
    public Inventory getItemUsedInv()
    {
        return this.itemUsedInv;
    }
    public float getModifier()
    {
        float baseMod = 1;
        for ( int i = 0; i < this.modifiers.Count; i++)
        {
            baseMod *= this.modifiers[i];
        }
        return baseMod;
    }


    public void setState(GameEnums.HealthState state)
    {
        this.healthState = state;
    }

    public void setRole(GameEnums.Role role)
    {
        this.role = role;
    }

    public void setActionState(GameEnums.ActionState action)
    {
        this.action = action;
    }

    // Role Ability Methods
    
    // The Gent: Can reroll if both loud dice show same value
    public bool CanGentReroll(int[] loudRolls)
    {
        if (role != GameEnums.Role.Gent || loudRolls.Length != 2) return false;
        return loudRolls[0] == loudRolls[1];
    }

    // The Soldier: +3 to CQC, cannot sneak attack
    public int GetCQCModifier()
    {
        if (role == GameEnums.Role.Soldier) return 3;
        if (role == GameEnums.Role.Thief) return -3;
        return 0;
    }

    public bool CanSneakAttack()
    {
        if (role == GameEnums.Role.Soldier) return false;
        if (role == GameEnums.Role.Double_Agent && isRoleRevealed) return false;
        return true;
    }

    // The Assassin: +3 to quiet rolls, -3 to loud rolls, cannot initiate CQC, +2 item range
    public int GetMovementModifier(GameEnums.Movement movement)
    {
        if (role == GameEnums.Role.Assassin)
        {
            if (movement == GameEnums.Movement.Soft) return 3;
            if (movement == GameEnums.Movement.Loud) return -3;
        }
        return 0;
    }

    public bool CanInitiateCQC()
    {
        if (role == GameEnums.Role.Assassin) return false;
        return true;
    }

    public int GetItemRangeBonus()
    {
        if (role == GameEnums.Role.Assassin) return 2;
        return 0;
    }

    // The Vengeful: Instant objective completion
    public bool CanInstantCompleteObjective()
    {
        return role == GameEnums.Role.Vengeful;
    }

    // The Double Agent: See all roles when it's their turn
    public bool CanSeeAllRoles()
    {
        return role == GameEnums.Role.Double_Agent;
    }

    public void RevealRole()
    {
        isRoleRevealed = true;
    }

    public bool IsRoleRevealed()
    {
        return isRoleRevealed;
    }
}
