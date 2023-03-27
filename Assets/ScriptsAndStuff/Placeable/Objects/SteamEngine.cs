using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamEngine : Placeable, IMechanicalDevices, IItemDevice
{
    private Sides OutputFace;

    //Mechanical props
    public Dictionary<Sides, Energy> states { get; set; }
    public Dictionary<Sides, OutBound> output_devices { get; set; }
    public override Item.Items item_link { get => Item.Items.SteamEngine; }
    //Item props
    public List<ItemSlot> itemSlots { get; set; }
    public List<drone_charger> connected_drones { get; set; }

    private double current_anim_speed = 1;

    public double speed = 0;
    public double torque = 0;

    public string last_connection { get; set; }

    public void initialize()
    {
        OutputFace = MatchTouchingSide(Sides.Down);

        states = new Dictionary<Sides, Energy>(0);
        output_devices = new Dictionary<Sides, OutBound>(1);
        output_devices.Add(OutputFace, OutBound.Empty());


        itemSlots = new List<ItemSlot>(2);
        connected_drones = new List<drone_charger>(1);
        itemSlots.Add(new ItemSlot(ItemSlot.SLOT_MODE.FORCE_IN, max_push:1));

        speed = 128;
        torque = 128;
    }

    public void update_states()
    {
        foreach(var output in output_devices)
        {
            output.Value.set_energy_value(speed, torque);
        }
    }

    public bool isOutputing(Sides side) => side == OutputFace;
    public bool isInputing(Sides side) => false;

    public override void handle_animations()
    {
        if (current_anim_speed != speed)
        {
            anim.SetFloat("Speed", (float)(speed / 100.0));
            current_anim_speed = speed;
        }
    }

}
