using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows;
using UnityEngine;
using UnityEngine.UI;

public class GUI_HotBar : MonoBehaviour
{
    public GameObject HotBar_Button_Instance;
    [SerializeField]
    public int selected_item;

    const int Hot_Bar_size = 6;
    private Image[] icons = new Image[Hot_Bar_size];
    private Button[] buttons = new Button[Hot_Bar_size];
    [SerializeField]
    private Sprite None_ICON;

    // Start is called before the first frame update
    void Start()
    {
        None_ICON = Resources.Load<Sprite>("Item_Icons/ICON_None");
        icons = new Image[Hot_Bar_size];
        for (int i = 0; i < Hot_Bar_size; i++)
        {
            var button_obj = Instantiate(HotBar_Button_Instance,this.transform, false);
            var button = button_obj.GetComponent<Button>();
            int temp = i;
            button.onClick.AddListener(delegate { button_event(temp); });
        
            button_obj.SetActive(true);
            icons[i] = button_obj.GetComponentsInChildren<Image>()[1];
            buttons[i] = button;
      
            set_item(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int id = GetClickedNumericalKey();

        if(id > -1 && id <= Hot_Bar_size)
        {
            button_event(id-1);
        }
        set_active(selected_item);
    }
    public void button_event(int id)
    {
        if (id != -1 && id < Hot_Bar_size)
        {
            selected_item = id;
        }
    }
    public void set_active(int id)
    {
        if (id < Hot_Bar_size)
        {
            buttons[id].Select();
        }
    }
    public void set_item(int id, Item item = null)
    {

        if (id < Hot_Bar_size)
        {
            Sprite sprite;
            if (item == null || item.type == Item.Items.None)
            {
                sprite = None_ICON;
                
            }
            else
            {
                sprite = Resources.Load<Sprite>(string.Format("Item_Icons/ICON_{0}", item.type));
                if (sprite == null)
                {
                    sprite = None_ICON;
                }
            }
            icons[id].sprite = sprite;
        }
    }

    //Returns -min if there is no cliced numerical key.
    private int GetClickedNumericalKey()
    {
        for (int i = 0; i < 10; ++i)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                return i;
            }
        }
        return int.MinValue;
    }

}
