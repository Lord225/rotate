using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : Placeable, IFluidDevice
{
    //IFluidDevice
    public Dictionary<Sides, IfluidInOutType> in_behevior { get; set; }
    public Dictionary<Sides, IFluidDevice> connected { get; set; } = new Dictionary<Sides, IFluidDevice>();
    public Fluid fluid_inside { get; set; } = new Fluid();
    public override Item.Items item_link { get => Item.Items.valve; }
    public double maximum_capacity { get; set; } = 1000;
    public double maxflow { get; set; } = 100;

    public Transform fluid_level;

    public double flow_rate = 100;

    public override void handle_animations()
    {

    }

    public void initialize()
    {
        in_behevior = new Dictionary<Sides, IfluidInOutType>()
                    {
                        { MatchTouchingSide(Sides.Up), IfluidInOutType.passive },
                        { MatchTouchingSide(Sides.Down), IfluidInOutType.push },
                        { MatchTouchingSide(Sides.Right), IfluidInOutType.blocked},
                        { MatchTouchingSide(Sides.Left), IfluidInOutType.blocked}
                    };
    }

    public void OnOverFlow()
    {
        print("Valve Would Explode!");
    }
}
