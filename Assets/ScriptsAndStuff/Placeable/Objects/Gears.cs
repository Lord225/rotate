using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gears : Placeable, IMechanicalDevices
{
    public virtual double ratio { get; }

    private List<Sides> InputFaces;
    private List<Sides> OutputFaces;

    private double current_anim_speed = -1;
    public override Item.Items item_link { get => Item.Items.None; }

    public string last_connection { get; set; }
    public Dictionary<Sides, Energy> states { get; set; }
    public Dictionary<Sides, OutBound> output_devices { get; set; }

    public double speed;
    public double torque;


    public void initialize()
    {
        InputFaces = new List<Sides>(2)
        { MatchTouchingSide(Sides.Up), MatchTouchingSide(Sides.Down) };
        OutputFaces = new List<Sides>(2)
        { MatchTouchingSide(Sides.Down), MatchTouchingSide(Sides.Up) };
        output_devices = new Dictionary<Sides, OutBound>(InputFaces.Count);
        states = new Dictionary<Sides, Energy>(InputFaces.Count);

        foreach (var i in InputFaces)
        {
            states.Add(i, new Energy());
        }
        foreach(var i in OutputFaces)
        {
            output_devices.Add(i, OutBound.Empty());
        }
    }

    public void update_states()
    {
        output_devices[OutputFaces[0]].set_energy_value(states[InputFaces[0]].Speed* ratio, states[InputFaces[0]].Torque/ ratio);
        output_devices[OutputFaces[1]].set_energy_value(states[InputFaces[1]].Speed/ ratio, states[InputFaces[1]].Torque* ratio);
        speed = states[InputFaces[1]].Speed - states[InputFaces[0]].Speed;
        torque = states[InputFaces[1]].Torque - states[InputFaces[0]].Torque;
    }

    public bool isOutputing(Sides side) => OutputFaces.Contains(side);

    public bool isInputing(Sides side) => InputFaces.Contains(side);

    public override void handle_animations()
    {
        if (current_anim_speed != speed)
        {
            anim.SetFloat("Speed", (float)(speed / 100.0));
            current_anim_speed = speed;
        }
    }

}
