using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlaceblePrefabHandeler : MonoBehaviour
{
    public GameObject ParentForPrefabs;

    private Quaternion SwitchAxes = Quaternion.Euler(-90, 0, 0);
    private Dictionary<Item.Items, GameObject> placable = new Dictionary<Item.Items, GameObject>();

    public void Start()
    {
        var resources = Resources.LoadAll<GameObject>("Prefabs");
      
        foreach (var i in resources)
        {
            try
            {
                placable.Add(i.GetComponent<Placeable>().item_link, i);
            }
            catch (Exception err)
            {
                Debug.LogError("Cannot create create item! " + err + " With item: " + i);
            }
        }
        
    }

    public Placeable CreatePrefab(Item.Items item, int rotation, Vector2Int pos)
    {
        try
        {
            var obj = Instantiate(placable[item]);
            var obj_comp = obj.GetComponent<Placeable>();

            if (rotation != -1)
            {
                obj_comp.Init_Frontend(pos, rotation);
            }
            return obj_comp;
        }
        catch
        {
            Debug.LogError("Cannot find item: " + item.ToString());
            return null;
        }
    }
}
