using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MechanicalEnergyMenager : MonoBehaviour
{
    public Terrain terrain;

    const float animation_update_timing = 0.5f;
    const float mechanical_device_tick = 0.2f;
    float anim_timing = 0;
    float device_timing = 0;

    static Sides[] all_sides = (Sides[])Enum.GetValues(typeof(Sides));

    LinkedList<IMechanicalDevices> AllDevicesLists = new LinkedList<IMechanicalDevices>();
  

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        anim_timing += Time.deltaTime;
        device_timing += Time.deltaTime;

        if(anim_timing > animation_update_timing)
        {
            anim_tick();
            anim_timing = 0;
        }
        if(device_timing > mechanical_device_tick)
        {
            tick();
            device_timing = 0;
        }
        
    }
    private (List<IMechanicalTransferDevices>, IMechanicalDevices, Sides) recursive_search(Placeable source, Sides dir)
    {
        HashSet<Vector2Int> reached = new HashSet<Vector2Int>(); //safety feature. Prevents from from infinity loops

        List<IMechanicalTransferDevices> transferDevices = new List<IMechanicalTransferDevices>();

        var neighbours = terrain.get_neighbours(source.position);
        reached.Add(Vector2Int.FloorToInt(source.position));
        Tile first = MapListToSides(neighbours, dir);
        
        if(first.PlacedObject is null)
        {
            return (transferDevices, null, 0);
        }
        var mached = Placeable.GetSideThatIsRelativeOnEdge(dir);

        while (true)
        {
            var mechanical = first.getMechanicalDevice();
            if (mechanical is null)
            {
                return (transferDevices, null, 0);
            }

            try
            {
                var transfer = (IMechanicalTransferDevices)mechanical;
                mached = transfer.MatchSide(mached);
                transferDevices.Add(transfer);
            }
            catch
            {
                return (transferDevices, mechanical, mached);
            }


            neighbours = terrain.get_neighbours(first.PlacedObject.position);
            if (reached.Contains(Vector2Int.FloorToInt(first.PlacedObject.position))){
                Debug.LogError("closed-loop actuator in system!");
                return (transferDevices, null, 0);
            }
            reached.Add(Vector2Int.FloorToInt(first.PlacedObject.position));
            first = MapListToSides(neighbours, mached);
            mached = Placeable.GetSideThatIsRelativeOnEdge(mached);

            if (first.PlacedObject is null)
            {
                return (transferDevices, null, 0);
            }
        }
    }

    //Connects dev1------>dev2
    private void connect((IMechanicalDevices, Sides) dev1, (IMechanicalDevices, Sides) dev2, List<IMechanicalTransferDevices> devices)
    {
        if (!dev2.Item1.states.ContainsKey(dev2.Item2))
        {
            Debug.LogError(string.Format("{0} has not port on side {1}", dev2, dev2.Item2));
            return;
        }
        if (!(dev2.Item1.states[dev2.Item2] is null))
        {
            //Make connections
            dev1.Item1.output_devices[dev1.Item2] = new OutBound(dev2.Item1.states[dev2.Item2], devices);
        }
    }
    private void connect_transfer((IMechanicalDevices, Sides) dev1, List<IMechanicalTransferDevices> devices)
    {
        dev1.Item1.output_devices[dev1.Item2] = new OutBound(null, devices);
    }
    private void disconnect(IMechanicalDevices dev, Sides side, List<IMechanicalTransferDevices> devices)
    {
        if (dev != null)
        {
            if (!dev.output_devices.ContainsKey(side))
            {
                Debug.LogError("Device is not connected!");
            }
            foreach (var state in dev.states)
            {
                state.Value.set(0, 0);
            }
            dev.output_devices[side] = new OutBound(null, devices);
        }
    }
    private void process_endpoint(IMechanicalDevices start, IMechanicalDevices end, Sides start_side, Sides end_side, List<IMechanicalTransferDevices> transfers)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append((start is null) ? "null" : start.ToString());
        builder.Append("\t");
        builder.Append(start_side.ToString());
        builder.Append("\t--\t");
        builder.Append((end is null) ? "null" : end.ToString());
        builder.Append("\t");
        builder.Append(end_side.ToString());

        //print(builder);
        
        if(end is null)
        {
            print(string.Format("{0} -------| null", start));
            connect_transfer((start, start_side), transfers);
        }
        else
        {
            if (end.isInputing(end_side) && start.isOutputing(start_side))
            {
                print(string.Format("{0}({2}) -------> ({3}){1}", start, end, start_side, end_side));
                connect((start, start_side), (end, end_side), transfers);
            }
            if (end.isOutputing(end_side) && start.isInputing(start_side))
            {
                print(string.Format("{1}({3}) -------> ({2}){0}", start, end, start_side, end_side));
                connect((end, end_side), (start, start_side), transfers);
            }

        }
    }

    public void connect_devices(IMechanicalDevices main)
    {
        main.initialize();

        if (!(main is IMechanicalTransferDevices) && !AllDevicesLists.Contains(main))
        {
            AllDevicesLists.AddLast(main);
        }

        var main_placable = (Placeable)main;
        var final_transfer_list = new List<IMechanicalTransferDevices>(4);
        var endpoints = new List<(IMechanicalDevices, Sides, Sides, List<IMechanicalTransferDevices>)>(2);
        try
        {
            //Transfer
            foreach (var side in all_sides)
            {
                if (main.isInputing(side) || main.isOutputing(side))
                {
                    var (list, end, final_side) = recursive_search(main_placable, side);

                    final_transfer_list.AddRange(list);
                    endpoints.Add((end, side, final_side, list));
                }
            }

            foreach(var (end, side, final_side, list) in endpoints)
            {
                try
                {
                    process_endpoint(main, end, side, final_side, list);
                }
                catch (Exception err){
                    print("Cannot connect device: " + err.Message);
                }
            }

            //if (endpoints.Count == 1)
            //{
            //    //connect_transfer((main, ), final_transfer_list);
            //    print("TODO");
            //}
            //else if (endpoints.Count == 2)
            //{
            //    bool isInputing0  = endpoints[0].Item1 == null ? false : endpoints[0].Item1.isInputing(endpoints[0].Item2);
            //    bool isInputing1  = endpoints[1].Item1 == null ? false : endpoints[1].Item1.isInputing(endpoints[1].Item2);
            //    bool isOutputing0 = endpoints[0].Item1 == null ? false : endpoints[0].Item1.isOutputing(endpoints[0].Item2);
            //    bool isOutputing1 = endpoints[1].Item1 == null ? false : endpoints[1].Item1.isOutputing(endpoints[1].Item2);

            //    if (isOutputing1)
            //    {
            //        if (isInputing0)
            //        {
            //            print(endpoints[1] + " ---------> " + endpoints[0]);
            //            connect(endpoints[1], endpoints[0], final_transfer_list);
            //        }
            //        else
            //        {
            //            print(endpoints[1] + " ---------| " + endpoints[0]);
            //            connect_transfer(endpoints[1], final_transfer_list);
            //        }
            //    }
            //    if (isOutputing0)
            //    {
            //        if (isInputing1)
            //        {
            //            print(endpoints[0] + " ---------> " + endpoints[1]);
            //            connect(endpoints[0], endpoints[1], final_transfer_list);
            //        }
            //        else
            //        {
            //            print(endpoints[0] + " ---------| " + endpoints[1]);
            //            connect_transfer(endpoints[0], final_transfer_list);
            //        }
            //    }
            //}
            //else if (endpoints.Count > 2)
            //{
            //    Debug.LogError("Sth gone soo wrong.");
            //}
            
        }
        catch (Exception err)
        {
            Debug.LogError(err.Message);
        }
    }

    public void remove_device(IMechanicalDevices to_remove)
    {
        var main_placable = (Placeable)to_remove;
        var endpoints = new List<(List<IMechanicalTransferDevices>, IMechanicalDevices, Sides)>(2);

        try
        {
            //Transfer
            foreach (var side in all_sides)
            {
                if (to_remove.isInputing(side) || to_remove.isOutputing(side))
                {
                    endpoints.Add(recursive_search(main_placable, side));
                }
            }
        }
        catch (Exception err)
        {
            Debug.LogError(err.Message);
        }
        foreach(var (list, device, side) in endpoints)
        {
            disconnect(device, side, list);
            if (device != null)
            {
                print("Disconnecting " + device.ToString());
            }
        }
        AllDevicesLists.Remove(to_remove);
    }

    private async Task HandleTransfer(double speed, List<IMechanicalTransferDevices> devices)
    {
        foreach (var dev in devices)
        {
            dev.SetAnim(speed);
            ((Placeable)dev).handle_animations();
        }
        await Task.CompletedTask;
    }
    public void tick()
    {
        foreach(var i in AllDevicesLists)
        {
            i.update_states();
        }
    }
    public void anim_tick()
    {
        foreach (var i in AllDevicesLists)
        {
            // Handle IMechanicalTransferDevices
            Task[] tasks = new Task[i.output_devices.Count];
            int iter = 0;
            foreach (var outputs in i.output_devices)
            {
                tasks[iter] = HandleTransfer(outputs.Value.get_speed(), outputs.Value.devices);
                iter++;
            }
            ((Placeable)i).handle_animations();
            Task.WaitAll(tasks);
        }
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