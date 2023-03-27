using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GUI_canvas : MonoBehaviour
{
    public GUI_HotBar hotBar { private set; get; }

    public enum MENU_STATE
    {
        standard,
        player_inventory,
        placeable_gui,
        object_gui,
    }

    public int getHotbar()
    {
        return hotBar.selected_item;
    }

    public MENU_STATE state;

    // Start is called before the first frame update
    void Start()
    {
        state = MENU_STATE.standard;
        hotBar = GameObject.Find("ItemBar").GetComponent<GUI_HotBar>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
