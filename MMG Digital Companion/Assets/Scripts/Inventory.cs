using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
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
    public void addItem(Item item)
    {
        this.inventory.Add(item);
    }
    public void removeItem(Item item)
    {
        this.inventory.Remove(item);
    }
}
