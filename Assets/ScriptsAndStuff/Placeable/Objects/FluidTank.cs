using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidTank : Placeable, IFluidDevice
{
    public const double max = 50000;
    //IFluidDevice
    public Dictionary<Sides, IfluidInOutType> in_behevior { get; set; }
    public Dictionary<Sides, IFluidDevice> connected { get; set; } = new Dictionary<Sides, IFluidDevice>();
    public override Item.Items item_link { get => Item.Items.tank; }
    public Fluid fluid_inside { get; set; } = new Fluid();
    public double maximum_capacity { get; set; } = max;
    public double maxflow { get; set; } = 500;

    public Transform fluid_level;
    public double amount = 0;

    private float true_fluid_level;
    private float vel;

    private void Update()
    {
        amount = fluid_inside.amount;
        true_fluid_level = 0.6f + (float)(amount / max) * 1.15f;

        fluid_level.transform.position = new Vector3(fluid_level.transform.position.x, Mathf.SmoothDamp(fluid_level.transform.position.y, true_fluid_level, ref vel, Time.deltaTime*5), fluid_level.transform.position.z);
    }

    public override void handle_animations()
    {
        
    }

    public void initialize()
    {
        in_behevior = new Dictionary<Sides, IfluidInOutType>()
                    {
                        { MatchTouchingSide(Sides.Up), IfluidInOutType.passive },
                        { MatchTouchingSide(Sides.Down), IfluidInOutType.active },
                        { MatchTouchingSide(Sides.Right), IfluidInOutType.passive},
                        { MatchTouchingSide(Sides.Left), IfluidInOutType.passive}
                    };
    }

    public void OnOverFlow()
    {
        print("Overflow");
        fluid_inside.amount = max;
    }
}
