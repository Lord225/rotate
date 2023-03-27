using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

public class FluidMenager : MonoBehaviour
{
    [Flags]
    private enum side_flags
    {
        up = 1,
        down = 2,
        left = 4,
        right = 8
    }
    const float fluid_tick = 0.2f;

    float fluid_timing = 0;

    public Mesh Streight_UpDowm;
    public Mesh Streight_RightLeft;
    public Mesh Turn1;
    public Mesh Turn2;
    public Mesh Turn3;
    public Mesh Turn4;
    public Mesh Only1;
    public Mesh Only2;
    public Mesh Only3;
    public Mesh Only4;
    public Mesh TShape1;
    public Mesh TShape2;
    public Mesh TShape3;
    public Mesh TShape4;
    public Mesh AllFour;


    readonly Sides[] all_sides = (Sides[])Enum.GetValues(typeof(Sides));

    LinkedList<IFluidDevice> AllDevicesLists = new LinkedList<IFluidDevice>();

    private Task lastTask;

    private void Start()
    {
        lastTask = new Task(tick);
        lastTask.Start();
    }


    void Update()
    {
        fluid_timing += Time.deltaTime;

        if (fluid_timing > fluid_tick && lastTask.IsCompleted)
        {
            lastTask = new Task(tick);
            lastTask.Start();
            
            fluid_timing = 0;
        }

    }
    
    public void connect_devices(IFluidDevice master, Tile[] neighbours)
    {
        AllDevicesLists.AddLast(master);
        master.initialize();

        foreach (var side in all_sides)
        {
            if (master.in_behevior[side] != IfluidInOutType.blocked)
            {
                IFluidDevice neighbour = MapListToSides(neighbours, side).getFluidDevice();

                if (neighbour != null)
                {
                    if (neighbour.in_behevior[side] != IfluidInOutType.blocked)
                    {
                        Sides side_maped = Placeable.GetSideThatIsRelativeOnEdge(side);

                        //add new block to neighbour
                        neighbour.connected.Add(side_maped, master);

                        //add new block to master
                        master.connected.Add(side, neighbour);

                        update_mesh(neighbour);
                        //update neighbour mesh 
                    }
                }
            }
        }
        update_mesh(master);
        //update master mesh
    }

    public void remove_device(IFluidDevice target, Tile[] neighbours)
    {
        AllDevicesLists.Remove(target);

        foreach (var side in all_sides)
        {
            IFluidDevice neighbour = MapListToSides(neighbours, side).getFluidDevice();
            if (neighbour != null) {
                try
                {
                    neighbour.connected.Remove(Placeable.GetSideThatIsRelativeOnEdge(side));
                    update_mesh(neighbour);
                }
                catch
                {
                    continue;
                }
            }
        }
    }

    void update_mesh(IFluidDevice target)
    {
        if(!(target is pipe))
        {
            return;
        }
        try
        {
            var converted = (Placeable)target;

            if(converted.rotation != 0)
            {
                converted.Init_Frontend(Vector2Int.zero,0);
            }
            side_flags flaged = 0;
            Mesh choosed_mesh;

            foreach (var side in target.connected.Keys)
            {
                if (target.in_behevior[side]==IfluidInOutType.blocked)
                {
                    continue;
                }
                switch (side)
                {
                    case Sides.Right:
                        flaged |= side_flags.right;
                        break;
                    case Sides.Down:
                        flaged |= side_flags.down;
                        break;
                    case Sides.Left:
                        flaged |= side_flags.left;
                        break;
                    case Sides.Up:
                        flaged |= side_flags.up;
                        break;
                }

            }
            //TODO - fix it.
            switch ((int)flaged)
            {
                case 0:
                    choosed_mesh = AllFour; //R
                    break;
                case 1:
                    choosed_mesh = Only3; //R
                    break;
                case 2:
                    choosed_mesh = Only2; //R
                    break;
                case 3:
                    choosed_mesh = Streight_UpDowm; //R
                    break;
                case 4:
                    choosed_mesh = Only1; //R
                    break;
                case 5:
                    choosed_mesh = Turn2; //R
                    break;
                case 6:
                    choosed_mesh = Turn1; //R
                    break;
                case 7:
                    choosed_mesh = TShape1; //R
                    break;
                case 8:
                    choosed_mesh = Only4; //R
                    break;
                case 9:
                    choosed_mesh = Turn3; //R
                    break;
                case 10:
                    choosed_mesh = Turn4; //R
                    break;
                case 11:
                    choosed_mesh = TShape3; //R
                    break;
                case 12:
                    choosed_mesh = Streight_RightLeft; //R
                    break;
                case 13:
                    choosed_mesh = TShape2; //R
                    break;
                case 14:
                    choosed_mesh = TShape4;  //R
                    break;
                case 15:
                    choosed_mesh = AllFour; //R
                    break;
                default:
                    choosed_mesh = AllFour; //R
                    break;
            }

            converted.filter.sharedMesh = choosed_mesh;
        }
        catch
        {
            Debug.LogError("Can't convert " + target + " to Placable");
        }
    }

    public void tick()
    {
        //TODO Lazy loop/job system
        foreach (var i in AllDevicesLists)
        {
            update_device_unthreaded(i);
        }


        foreach (var i in AllDevicesLists)
        {
            finalize(i);
        }
    }

    void finalize(IFluidDevice target)
    {

        if(target.fluid_inside.amount > target.maximum_capacity)
        {
            target.OnOverFlow();
        }
    }


    void update_device_unthreaded(IFluidDevice target)
    {
        double amount;
        double flow = 0;
        int device = 1;


        amount = target.fluid_inside.amount;
        

        foreach (var side in all_sides)
        {
            if (target.connected.TryGetValue(side, out IFluidDevice neighbour))
            {
                if (neighbour.in_behevior[Placeable.GetSideThatIsRelativeOnEdge(side)] == IfluidInOutType.active)
                {
                    
                    flow = Math.Min(Math.Min(neighbour.fluid_inside.amount, neighbour.maxflow), target.maxflow);
                    neighbour.fluid_inside.amount -= flow;
                    
                    amount += flow;
                    device++;
                }
            }
        }
        
        foreach (var side in all_sides)
        {
            if (target.connected.TryGetValue(side, out IFluidDevice neighbour))
            {
                if(neighbour.in_behevior[Placeable.GetSideThatIsRelativeOnEdge(side)] == IfluidInOutType.active)
                {
                    if(target.in_behevior[side] == IfluidInOutType.push || target.in_behevior[side] == IfluidInOutType.active) 
                    {
                    
                        flow = Math.Min(Math.Min(amount / device, neighbour.maxflow), target.maxflow);
                        neighbour.fluid_inside.amount += flow;
                        
                        amount -= flow;
                    }
                }
                else if (neighbour.in_behevior[Placeable.GetSideThatIsRelativeOnEdge(side)] == IfluidInOutType.passive)
                {

                    flow = Math.Min(Math.Min(amount / device, neighbour.maxflow), target.maxflow);
                    neighbour.fluid_inside.amount += flow;
                    
                    amount -= flow;
                }
            }
        }

        target.fluid_inside.amount = amount;
        
    }

    //Find neighbour that is on "side" side.
    Tile MapListToSides(Tile[] neighbours, Sides side)
    {
        switch (side)
        {
            case Sides.Left:
                return neighbours[0];
            case Sides.Down:
                return neighbours[1];
            case Sides.Right:
                return neighbours[2];
            case Sides.Up:
                return neighbours[3];
            default:
                break;
        }
        return null;
    }
}
