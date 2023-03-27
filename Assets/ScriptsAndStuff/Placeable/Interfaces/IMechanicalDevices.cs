using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sides : byte
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3
}
public class Energy
{
    public double Torque;
    public double Speed;

    public double get_power()
    {
        return (double)Torque * Speed;
    }

    public void set(double Torque = 0, double Speed = 0)
    {
        if (Speed >= 1 && Torque >= 1)
        {
            this.Torque = Torque;
            this.Speed = Speed;
        }
        else
        {
            this.Torque = 0;
            this.Speed = 0;
        }

    }
    public void set(Energy refrence)
    {
        set(refrence.Torque, refrence.Speed);
    }

    public (double, double) get_truple()
    {
        return (Speed, Torque);
    }
    public static bool operator >(Energy left, Energy right)
    {
        return left.get_power() > right.get_power();
    }
    public static bool operator < (Energy left, Energy right)
    {
        return left.get_power() < right.get_power();
    }

    public override string ToString()
    {
        return "Torque: " + Torque.ToString() + "\tSpeed: " + Speed.ToString();
    }
}
public struct OutBound
{
    public OutBound(Energy energy, List<IMechanicalTransferDevices> devices)
    {
        this.energy = energy;
        this.devices = devices;
    }
    public static OutBound Empty()
    {
        return new OutBound(null, new List<IMechanicalTransferDevices>());
    }
    public void set_energy(Energy energy)
    {
        this.energy = energy;
    }
    public void set_energy_value(Energy energy)
    {
        if(this.energy != null)
        {
            this.energy.set(energy);
        }
    }
    public double get_speed()
    {
        if(energy == null)
        {
            return 0;
        }
        return energy.Speed;
    }
    public double get_Torque()
    {
        if (energy == null)
        {
            return 0;
        }
        return energy.Torque;
    }
    public void set_energy_value(double speed, double torque)
    {
        if (energy != null)
        {
            energy.set(torque, speed);
        }
    }
    
    public bool isEnergyNull()
    {
        return energy is null;
    }


    private Energy energy;
    public List<IMechanicalTransferDevices> devices;
}

//Interface that handles all mechanical devices (including engines, shafts, etc)
public interface IMechanicalDevices
{
    Dictionary<Sides, Energy> states { get; set; } //Port states (this block)
    Dictionary<Sides, OutBound> output_devices { get; set; } //Links to ports (connected devices)

    void initialize(); //Called On create.
    void update_states(); // Set output_devices in way that fits you.

    bool isOutputing(Sides side);
    bool isInputing(Sides side);
}
public interface IMechanicalTransferDevices: IMechanicalDevices
{
    Sides MatchSide(Sides side);

    void SetAnim(double speed);
}
