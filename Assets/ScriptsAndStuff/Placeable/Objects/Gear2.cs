using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear2 : Gears
{
    public override double ratio { get => 2; }
    public override Item.Items item_link { get => Item.Items.Gear2; }
}
