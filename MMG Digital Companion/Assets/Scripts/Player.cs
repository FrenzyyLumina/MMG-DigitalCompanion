using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameEnums.HealthState   state;
    private readonly GameEnums.Role role;
    private GameEnums.ActionState   action;

    private readonly Inventory inventory;
    private List<float> modifiers; //Multiplicative

    public Player(/*GameEnums.Role role*/)
    {
        this.state = GameEnums.HealthState.Normal;
        //this.role = role;
        this.action = GameEnums.ActionState.Normal;
        
        this.inventory = new Inventory();
        this.modifiers = new List<float>();
    }

    public GameEnums.HealthState getHealthState()
    {
        return this.state;
    }
    public GameEnums.Role getPlayerRole()
    {
        return this.role;
    }
    public GameEnums.ActionState getActionState()
    {
        return this.action;
    }


    public void setState(GameEnums.HealthState state)
    {
        this.state = state;
    }

    public void setRole(GameEnums.Role role)
    {
        this.role = role
    }

    public void setActionState(GameEnums.ActionState action)
    {
        this.action = action;
    }
}
