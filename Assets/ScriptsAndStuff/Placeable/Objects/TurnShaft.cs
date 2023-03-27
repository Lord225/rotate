using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnShaft : Placeable, IMechanicalTransferDevices
{
    private List<Sides> InputFaces { get; set; }
    private List<Sides> OutputFaces { get; set; }
    public override Item.Items item_link { get => Item.Items.ShaftTurn; }
    public string last_connection { get; set; }
    public Dictionary<Sides, Energy> states { get; set; }
    public Dictionary<Sides, OutBound> output_devices { get; set; }

    private double current_anim_speed = 1;

    public double speed;
    public double torque;


    public void initialize()
    {
        InputFaces = new List<Sides>(2)
        { MatchTouchingSide(Sides.Down), MatchTouchingSide(Sides.Left) };
        OutputFaces = new List<Sides>(2)
        { MatchTouchingSide(Sides.Left), MatchTouchingSide(Sides.Down) };
        output_devices = new Dictionary<Sides, OutBound>(InputFaces.Count);
        states = new Dictionary<Sides, Energy>(InputFaces.Count);

        foreach (var i in InputFaces)
        {
            states.Add(i, new Energy());
        }
        foreach (var i in OutputFaces)
        {
            output_devices.Add(i, OutBound.Empty());
        }
        
    }
    public bool isOutputing(Sides side) => OutputFaces.Contains(side);

    public bool isInputing(Sides side) => InputFaces.Contains(side);

    public void update_states()
    {
        
    }

    public override void handle_animations()
    {
        if (current_anim_speed != speed)
        {
            anim.SetFloat("Speed", (float)(speed / 100.0));
            current_anim_speed = speed;
        }
    }

    public Sides MatchSide(Sides side)
    {
        try
        {
            return OutputFaces[InputFaces.FindIndex(x => x == side)];
        }
        catch
        {
            return Sides.Down;
        }
    }

    void IMechanicalTransferDevices.SetAnim(double speed)
    {
        this.speed = speed;
    }

}
