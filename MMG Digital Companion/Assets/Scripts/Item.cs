using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private readonly string itemName;
    private readonly GameEnums.ItemUseType itemUseType;

    public Item(string name, GameEnums.ItemUseType useType)
    {
        this.itemName = name;
        this.itemUseType = useType;
    }

    public string getItemName()
    {
        return this.itemName;
    }


}
