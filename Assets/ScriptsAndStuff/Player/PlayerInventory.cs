using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Item[] HotBar;
    public Item[,] Inventory;

    // Start is called before the first frame update
    void Start()
    {
        HotBar = new Item[6];
        for (int i = 0; i < HotBar.Length; i++)
        {
            HotBar[i] = new Item(amount: 9999);
        }
        HotBar[0].type = Item.Items.StraightShaft;
        HotBar[1].type = Item.Items.SteamEngine;
        HotBar[2].type = Item.Items.ShaftTurn;
        HotBar[3].type = Item.Items.Gear2;
        HotBar[4].type = Item.Items.Gear4;
        HotBar[5].type = Item.Items.Pump;
    }

    public void get_item_in_hotbar(int id, ref Item item)
    {
        if (id < 6)
        {
            item = HotBar[id];
        }
        else
        {
            Debug.LogError("Wrong hotbarslot!");
        }
    } 
}
