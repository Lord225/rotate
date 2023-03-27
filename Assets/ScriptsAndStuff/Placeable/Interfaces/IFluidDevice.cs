using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Fluid
{
    static Color getFluidColor(fluid_type type)
    {
        return new Color();
    }

    public enum fluid_type
    {
        water,
        steam,
        oil,
    }

    public double amount { get; set; }
    public fluid_type type { get; set; }

    public Fluid(double amount = 0)
    {
        this.amount = amount;
    }



    public override string ToString()
    {
        return string.Format("{0}: {1}", type.ToString(), amount);
    }
}


public enum IfluidInOutType
{
    active,
    passive,
    push,
    blocked,
}

public interface IFluidDevice
{
    //Try to add flow_limit
    //better - add valve (will work like tank but with limited output)
    Dictionary<Sides, IfluidInOutType> in_behevior { get; set; }
    Dictionary<Sides, IFluidDevice> connected { get; set; }
    Fluid fluid_inside { get; set; }
    double maximum_capacity { get; set; }
    double maxflow { get; set; }
    void initialize();
    void OnOverFlow();
}
