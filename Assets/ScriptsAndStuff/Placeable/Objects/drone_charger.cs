using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Drone Charger and controller
public class drone_charger : Placeable, IMechanicalDevices
{
    //TODO
    public class DestinationData
    {
        public Tile destination;
        public Arrow arrow;
        public bool isPushing;

        public DestinationData(Tile destination, bool isPushing, Arrow arrow)
        {
            this.destination = destination;
            this.isPushing = isPushing;
            this.arrow = arrow;
        }
    }

    public override Item.Items item_link { get => Item.Items.DroneCharger; }
    public List<DestinationData> destinations;

    private bool ShowArrows;
    public bool showArrows
    {
        get
        {
            return ShowArrows;
        }
        set
        {
            if (ShowArrows != value)
            {
                foreach (var item in destinations)
                {
                    item.arrow.SetVisible(value);
                }
                
                ShowArrows = value;
            }
        }
    }

    public float radius = 5.0f;

    public GameObject droneInstance;
    public float DroneSpeed = 2.3f;
    public const float droneFlightHeight = 2.0f;
    public Vector3 destination;
    public drone Drone;
    public Terrain terrain;

    //IMechanical
    public string last_connection { get; set; }
    public Dictionary<Sides, Energy> states { get; set; }
    public Dictionary<Sides, OutBound> output_devices { get; set; }

    //Drone's Inventory
    public int MAX_AMOUNT;
    public ItemSlot carried_item;

    private Sides InputFace;
    private int current_destination = 0;

    private void Start()
    {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        destinations = new List<DestinationData>(2);
    }
    private void OnDestory()
    {
        Destroy(Drone.gameObject);
    }

    public bool isInRange(Tile This)
    {
        return (This.PlacedObject.position - this.position).magnitude <= radius;
    }
   

    public Vector3 GetNextDestinationPos()
    {
        current_destination = (current_destination + 1) % destinations.Count;
        Placeable pos = destinations[current_destination].destination.PlacedObject;
        if (pos == null)
        {
            pos = this;
        }
        return new Vector3(pos.position.x, droneFlightHeight, pos.position.y);
    }

    public Tile GetCurrentDestination() => destinations[current_destination].destination;
    public bool isCurrentDestinationPushing() => destinations[current_destination].isPushing;

    void move()
    {
        if (Vector3.SqrMagnitude(Drone.transform.position - destination) < 0.001f)
        {
            //Arrived
            if (Drone.state != drone.drone_state.rotation_end)
            {
                OnArrive();
                Drone.state = drone.drone_state.rotating_init;
                destination = GetNextDestinationPos();
                Drone.direction = destination - Drone.transform.position;
            }
        }
        else
        {
            if (Drone.state == drone.drone_state.rotation_end)
            {
                Drone.state = drone.drone_state.flying;
            }
            if (Drone.state == drone.drone_state.flying)
            {
                //Flying
                Drone.transform.position = Vector3.MoveTowards(Drone.transform.position, destination, Time.deltaTime * DroneSpeed);
            }
        }
    }

    private void OnArrive()
    {
        Tile dest = GetCurrentDestination();

        IItemDevice dest_inventory = dest.getItemDevice();
        if (dest_inventory != null)
        {
            dest_inventory.itemSlots.Sort();

            foreach (var slot in dest_inventory.itemSlots)
            {
                if (carried_item.GetAmout() == 0)
                {
                    break;
                }
                slot.TryPushItems(ref carried_item);
            }
        }
    }

    public long speed;
    public long torque;
    private long current_anim_speed;

    public override void handle_animations()
    {
        if (current_anim_speed != speed)
        {
            anim.SetFloat("Speed", speed / 100.0f);
            current_anim_speed = speed;
        }
    }

    public void initialize()
    {
        InputFace = MatchTouchingSide(Sides.Down);
        states = new Dictionary<Sides, Energy>(1);
        states[InputFace] = new Energy();
        output_devices = new Dictionary<Sides, OutBound>(0);
        Drone = Instantiate(droneInstance, new Vector3(transform.position.x, droneFlightHeight, transform.position.z), Quaternion.identity).GetComponent<drone>();
        carried_item = new ItemSlot(ItemSlot.SLOT_MODE.INOUT);
        var item = new Item(Item.Items.Pump, 9999);

        carried_item.TryPushItem(ref item);
    }

    public void update_states()
    {
        
    }

    public bool isOutputing(Sides side) => false;

    public bool isInputing(Sides side) => side == InputFace;
}
