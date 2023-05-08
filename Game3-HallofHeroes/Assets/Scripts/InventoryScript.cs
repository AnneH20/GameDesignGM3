using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript
{
    public class Item
    {
        public string itemName;
        public string itemDescription;
        public int itemAmount;
        public int itemEffect; // For example, the amount of health to heal the player or damage to deal to the enemy.
        public float itemChance; // For example, the chance of the item being dropped by an enemy.
    }

    public class Inventory
    {
        public List<Item> items = new List<Item>();
    }
    public Inventory inventory = new Inventory();
}
