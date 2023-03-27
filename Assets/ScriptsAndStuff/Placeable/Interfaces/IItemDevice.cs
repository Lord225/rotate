using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemSlot
{
    public enum SLOT_MODE
    {
        INOUT = 0,
        FORCE_OUT = 1,
        FORCE_IN = 2
    }

    public SLOT_MODE MODE; //Only affects drones system.
    public Item items = new Item(amount: 0);
    public int priority;
    public int max;
    public int max_push;
    public Item.Items item_type = Item.Items.None;

    public ItemSlot(SLOT_MODE MODE, int priority = -1, int max = 100, int max_push = int.MaxValue)
    {
        this.MODE = MODE;
        this.priority = priority;
        this.max = max;
        this.max_push = max_push;
    }

    //None to allow all items
    public void SetRestrictedItemType(Item.Items type)
    {
        item_type = type;
    }

    //ref ItemSlot items --item--> this
    public void TryPushItems(ref ItemSlot items)
    {
        TryPushItem(ref items.items);
    }

    public void TryPushItem(ref Item items)
    {
        if (GetItemType() == Item.Items.None && items.type != Item.Items.None)
        {
            this.items.type = items.type;
        }

        if (item_type == items.type || item_type == Item.Items.None)
        {
            int diffrence = Math.Min(max - GetAmout(), max_push);

            if (diffrence > 0)
            {
                items.amount -= diffrence;
                this.items.amount += diffrence;
            }
        }
    }

    public static bool operator <(ItemSlot l, ItemSlot f)
    {
        return l.items.amount < f.items.amount;
    }

    public static bool operator>(ItemSlot l, ItemSlot f)
    {
        return l.items.amount > f.items.amount;
    }

    public int GetAmout()
    {
        if (items == null)
        {
            return 0;
        }
        return items.amount;
    }

    public Item.Items GetItemType()
    {
        if (items == null)
        {
            return Item.Items.None;
        }
        return items.type;
    }

    public override string ToString()
    {
        return string.Format("ItemSlot({0})", items);
    }

}

//Only for placable blocks!
public interface IItemDevice
{
    List<ItemSlot> itemSlots { get; set; }
    List<drone_charger> connected_drones { get; set; }

    void initialize();
}
