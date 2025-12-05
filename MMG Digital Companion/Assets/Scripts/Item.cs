using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    private readonly string itemName;
    private readonly GameEnums.Item item;
    private readonly GameEnums.ItemUseType itemUseType;
    private int count;

    public Item(GameEnums.Item item, GameEnums.ItemUseType useType)
    {
        this.itemName = item.ToString();
        this.item = item;
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
