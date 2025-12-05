using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using static UnityEditor.Progress; -- had to comment due to compiler error with this being unused.

public class Inventory
{
    private List<Item> inventory;

    public Inventory()
    {
        this.inventory = new List<Item>();
    }
    public List<Item> getItems()
    {
        return this.inventory;
    }
    public Item findItemByName(string name)
    {
        foreach (Item item in inventory)
        {
            if (item.getItemName() == name)
            {
                return item;
            }
        }

        return null;
    }
    public Item findItemByItemType(GameEnums.Item itemType)
    {
        foreach (Item item in inventory)
        {
            if (item.getItemType() == itemType)
            {
                return item;
            }
        }

        return null;
    }
    public void addItem(Item item)
    {
        Item exist = this.findItemByName(item.getItemName());
        if (exist != null)
        {
            exist.incCount();
        }
        else
        {
            this.inventory.Add(item);
        }
    }
    public void removeItemByName(string name)
    {
        Item exist = this.findItemByName(name);
        if (exist == null)
        {
            Debug.Log("No Item Found");
            return;
        }

        exist.decCount();
        if (exist.getCount() == 0)
            this.inventory.Remove(exist);
    }
    public void removeItem(Item item)
    {
        this.removeItemByName(item.getItemName());
    }
}
