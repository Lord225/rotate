using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Placeable, IMechanicalDevices, IFluidDevice
{
    private Sides InputFace;
    private Energy energy = new Energy();
    private double current_anim_speed = -1;

    //Imechanical
    public Dictionary<Sides, Energy> states { get; set; }
    public Dictionary<Sides, OutBound> output_devices { get; set; }

    public override Item.Items item_link { get => Item.Items.Pump; }
    //Ifluid
    public Dictionary<Sides, IfluidInOutType> in_behevior { get; set; }
    public Dictionary<Sides, IFluidDevice> connected { get; set; } = new Dictionary<Sides, IFluidDevice>();
    public Fluid fluid_inside { get; set; } = new Fluid();
    public double maximum_capacity { get; set; } = 5000;
    public double maxflow { get; set; } = 400;

    [SerializeField]
    public double amount = 0;

    public void initialize()
    {
        InputFace = MatchTouchingSide(Sides.Left);
        in_behevior = new Dictionary<Sides, IfluidInOutType>()
                    {
                        { MatchTouchingSide(Sides.Up), IfluidInOutType.blocked},
                        { MatchTouchingSide(Sides.Down), IfluidInOutType.active},
                        { MatchTouchingSide(Sides.Right), IfluidInOutType.blocked},
                        { MatchTouchingSide(Sides.Left), IfluidInOutType.blocked}
                    };
        states = new Dictionary<Sides, Energy>(1);
        states[InputFace] = new Energy();
        output_devices = new Dictionary<Sides, OutBound>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            fluid_inside.amount += 3000; //If it would appere with small amount (100, 200) it would not destroy pipes
        }
        amount = fluid_inside.amount;
    }
    public bool isInputing(Sides side) => side == InputFace;

    public bool isOutputing(Sides side) => false;

    public void update_states()
    {
        print(states[InputFace]);
    }

    //placable
    public override void handle_animations()
    {
        if (energy.Speed != current_anim_speed)
        {
            anim.SetFloat("Speed", (float)(energy.Speed / 100.0));
            current_anim_speed = energy.Speed;
        }
    }

    public void OnOverFlow()
    {
        fluid_inside.amount = 1000;
    }
}
