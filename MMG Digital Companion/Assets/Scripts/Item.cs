using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private readonly string itemName;
    private readonly GameEnums.ItemUseType itemUseType;
    private int count;

    public Item(string name, GameEnums.ItemUseType useType)
    {
        this.itemName = name;
        this.itemUseType = useType;
        this.count = 1;
    }

    public string getItemName()
    {
        return this.itemName;
    }

    public int getCount()
    {
        return this.count;
    }
    public void incCount()
    {
        this.count++;
    }
    public void decCount()
    {
        this.count--;
    }
}
