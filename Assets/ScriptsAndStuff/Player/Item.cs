using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    //NEW ITEMS: PlacablePrefabHandeler
    //here
    public enum Items
    {
        None,
        StraightShaft,
        ShaftTurn,
        SeparatorShaft,
        DroneCharger,
        Gear2,
        Gear4,
        Gear8,
        Gear16,
        SteamEngine,
        InfinityEngine,
        Bridge,
        Pump,
        pipe,
        tank,
        valve
    }

    public Item(Items type = Items.None, int amount = 1)
    {
        this.type = type;
        this.amount = amount;
    }

    public bool isPlacable()
    {
        switch (type)
        {
            case Items.StraightShaft:
            case Items.ShaftTurn:
            case Items.SeparatorShaft:
            case Items.DroneCharger:
            case Items.Gear2:
            case Items.Gear4:
            case Items.Gear8:
            case Items.Gear16:
            case Items.SteamEngine:
            case Items.InfinityEngine:
            case Items.Bridge:
            case Items.Pump:
            case Items.pipe:
            case Items.tank:
            case Items.valve:
                return true;
            default:
                return false;
        }
    }

    public bool isTerrainModifier()
    {
        return type == Items.Bridge;
    }

    public int amount = 9999;

    public Items type;

    public override string ToString()
    {
        return string.Format("Item({0},{1})", type, amount);
    }
}
