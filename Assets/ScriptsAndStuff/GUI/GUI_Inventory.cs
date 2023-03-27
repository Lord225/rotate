using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Inventory : MonoBehaviour
{
    public GameObject HotBar_Button_Instance;
    public GameObject SlotsHolder;

    public const int num_of_slots = 5 * 7-1;


    private Button[] buttons = new Button[num_of_slots];


    void Start()
    {
        for (int i = 0; i < num_of_slots; i++)
        {
            var button_obj = Instantiate(HotBar_Button_Instance, SlotsHolder.transform, false);
            var button = button_obj.GetComponent<Button>();
            int temp = i;
            button.onClick.AddListener(delegate { inventory_button_event(temp); });
            button_obj.SetActive(true);
            buttons[i] = button;
        }
    }

    void inventory_button_event(int id)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
