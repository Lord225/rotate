using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pipe : Placeable, IFluidDevice
{
    public Dictionary<Sides, IfluidInOutType> in_behevior { get; set; }
    public Dictionary<Sides, IFluidDevice> connected { get; set; } = new Dictionary<Sides, IFluidDevice>();
    public override Item.Items item_link { get => Item.Items.pipe; }
    public Fluid fluid_inside { get; set; } = new Fluid();
    public double maximum_capacity { get; set; } = 500;
    public double maxflow { get; set; } = 400;

    [SerializeField]
    public double amount = 0;

    public void initialize()
    {
        in_behevior = new Dictionary<Sides, IfluidInOutType>()
                    {
                        { MatchTouchingSide(Sides.Up), IfluidInOutType.active},
                        { MatchTouchingSide(Sides.Down), IfluidInOutType.active},
                        { MatchTouchingSide(Sides.Right), IfluidInOutType.active},
                        { MatchTouchingSide(Sides.Left), IfluidInOutType.active}
                    };
    }
    public override void handle_animations()
    {
        return;
    }
    public void OnOverFlow()
    {

        //Pipe dmg, before fluid would be spread when dmg == 100 -> pipe would broke
        //print("Pressure destorys pipe!");
    }
}
