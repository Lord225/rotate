using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public block type;
    public Placeable PlacedObject;
    public TerrainModifier SecondObject;

    public Tile(block _type)
    {
        type = _type;
    }

    public bool isPlacedObject()
    {
        return PlacedObject != null;
    }

    public bool PlaceObject(Placeable Object)
    {
        try
        {
            var obj = (TerrainModifier)Object;
            if (obj == null)
            {
                return false;
            }
            SecondObject = obj;
            return true;
        }
        catch
        {
            //Just Placable
        }
        if (PlacedObject != null)
        {
            return false;
        }
        PlacedObject = Object;
        return true;
    }

    //return null if is not IMechanicalDevice or there is nothing placed
    public IMechanicalDevices getMechanicalDevice()
    {
        if (PlacedObject != null)
        {
            try
            {
                return (IMechanicalDevices)PlacedObject;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    public IFluidDevice getFluidDevice()
    {
        if (PlacedObject != null)
        {
            try
            {
                return (IFluidDevice)PlacedObject;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    public IItemDevice getItemDevice()
    {
        if (PlacedObject != null)
        {
            try
            {
                return (IItemDevice)PlacedObject;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    public void RemoveObject()
    {
        if (PlacedObject != null)
        {
            Object.Destroy(PlacedObject.gameObject);
            PlacedObject = null;
            return;
        }
        if (SecondObject != null)
        {
            Object.Destroy(SecondObject.gameObject);
            SecondObject = null;
            return;
        }
    }

    public bool CanRemoveObject()
    {
        return PlacedObject != null || SecondObject != null;
    }

    public bool CanPlaceObject(Item _type)
    {
        if (_type.isTerrainModifier())
        {
            if (SecondObject == null)
            {
                //TODO Check if can palce modifier
                if (_type.type == Item.Items.Bridge && type == block.Water)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        
        if (PlacedObject == null)
        {
            if (Terrain.CanPlaceOnThisBlock(type))
            {
                return true;
            }
            if(type == block.Water && SecondObject != null && SecondObject.GetType() == typeof(Bridge))
            {
                return true;
            }
        }
        return false;
    }
}
