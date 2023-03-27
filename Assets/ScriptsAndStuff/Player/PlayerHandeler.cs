using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class PlayerHandeler : MonoBehaviour
{
    public enum INTERFACE_STATE
    {
        STANDARD_INTERACT,
        PLAYER_INVENTORY_LOOKUP,
        FORGEIN_INVENTORY_LOOKUP,
        DRONES_INTERACT,
        PAUSE_MENU,
    }

    public GameObject POINTER;
    public GameObject ARROW_PREFAB;
    public Terrain terrain;

    public Tile PointedTile { get; private set; }
    public int RotationOfObject = 0;
    public Vector3 OnMouseBlock { get; private set; }
    public Item ObjectInHands;
    public GUI_canvas GUI_master;
    public const float interactable_hover_time_threshold = 1.3f;
    public INTERFACE_STATE state;

    public Circle circle;
    public drone_charger active_drone_tile;

    private RaycastHit MousePointerHit;
    private CameraMovement CamMov;
    private Pointer pointer;
    private Ray MousePointer;
    private PlaceblePrefabHandeler PrefabHandeler;

    private MechanicalEnergyMenager EnergyMenager;
    private FluidMenager fluid_menager;

    private PlayerInventory inventory;
    private Placable_3D_GUI_HANDLE _GUI_HANDLE;

    private float interactable_hover_time;


    // Start is called before the first frame update
    void Start()
    {
        CamMov = GetComponent<CameraMovement>();
        pointer = POINTER.GetComponent<Pointer>();
        PrefabHandeler = GetComponent<PlaceblePrefabHandeler>();
        inventory = GetComponent<PlayerInventory>();
        EnergyMenager = GameObject.Find("Menager").GetComponent<MechanicalEnergyMenager >();
        fluid_menager = GameObject.Find("Menager").GetComponent<FluidMenager>();
        GUI_master = GameObject.Find("Canvas").GetComponent<GUI_canvas>();
        _GUI_HANDLE = GameObject.Find("FloatingMenu").GetComponent<Placable_3D_GUI_HANDLE>();
    }

    // Update is called once per frame
    void Update()
    {
        terrain.SetPosition(CamMov.transform.position);

        PlayerMouseUpdate();

        UpdateHotBarIcons();

        PointerUpdate();
        PointerGuiUpdate();

        MouseUse();

        CheckKeyboard();

        if (ObjectInHands != null && ObjectInHands.type != Item.Items.None)
        {
            PlayerRotateObject();
        }

        ChooseObjectInHands();
    }


    void CheckKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //pass for now (in future - set state to PLAYER_INV OR FORGAIN_INV
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (state)
            {
                case INTERFACE_STATE.STANDARD_INTERACT:
                    break;
                case INTERFACE_STATE.PLAYER_INVENTORY_LOOKUP:
                    state = INTERFACE_STATE.STANDARD_INTERACT;
                    break;
                case INTERFACE_STATE.FORGEIN_INVENTORY_LOOKUP:
                    state = INTERFACE_STATE.STANDARD_INTERACT;
                    break;
                case INTERFACE_STATE.DRONES_INTERACT:
                    state = INTERFACE_STATE.STANDARD_INTERACT;
                    active_drone_tile.showArrows = false;
                    active_drone_tile = null;
                    circle.animateFadeOut(0.2f);
                    break;
                case INTERFACE_STATE.PAUSE_MENU:
                    state = INTERFACE_STATE.STANDARD_INTERACT;
                    break;
            }
        }
    }

    void UpdateHotBarIcons()
    {
        for(int i = 0; i < inventory.HotBar.Length;i++)
        {
            GUI_master.hotBar.set_item(i, inventory.HotBar[i]);
        }
    }

    void ChooseObjectInHands()
    {
        int selectedSlot = GUI_master.getHotbar();
        inventory.get_item_in_hotbar(selectedSlot, ref ObjectInHands);
    }
   
    //All Pointer animations
    void PointerUpdate()
    {
        Tile pointed = GetPointedBlock();
        
        if (pointed != null)
        {
            PointedTile = pointed;
            pointer.TargetPosition = OnMouseBlock;

            Pointer.Colors color;

            switch (state)
            {
                case INTERFACE_STATE.STANDARD_INTERACT:
                    color = ChoosePointerColorStandard();
                    break;
                case INTERFACE_STATE.PLAYER_INVENTORY_LOOKUP:
                    color = Pointer.Colors.Gray;
                    break;
                case INTERFACE_STATE.FORGEIN_INVENTORY_LOOKUP:
                    color = Pointer.Colors.Gray;
                    break;
                case INTERFACE_STATE.DRONES_INTERACT:
                    color = ChoosePointerColorDrones();
                    break;
                case INTERFACE_STATE.PAUSE_MENU:
                    color = ChoosePointerColorStandard();
                    break;
                default:
                    color = Pointer.Colors.Gray;
                    break;
            }
            pointer.SetColor(color);
        }
    }

    Pointer.Colors ChoosePointerColorDrones()
    {
        if(PointedTile.PlacedObject == null || (PointedTile.PlacedObject.position-active_drone_tile.position).magnitude > active_drone_tile.radius)
        {
            return Pointer.Colors.Gray;
        }

        if(PointedTile.getItemDevice() != null)
        {
            return Pointer.Colors.Green;
        }

        return Pointer.Colors.Gray;
    }

    //Pointed Color On Standard 
    Pointer.Colors ChoosePointerColorStandard()
    {
        if (PointedTile.getMechanicalDevice() != null || PointedTile.getItemDevice() != null || PointedTile.getFluidDevice() != null)
        {
            return Pointer.Colors.Green;
        }

        if (ObjectInHands == null)
        {
            return Pointer.Colors.Gray;
        }

        if (PointedTile.CanPlaceObject(ObjectInHands))
        {
            return Pointer.Colors.Red; //Settable
        }
        else
        {
            return Pointer.Colors.Gray; //None Settble
        }
    }

    void PointerGuiUpdate()
    {
        if(_GUI_HANDLE.describe_tile != PointedTile)
        {
            _GUI_HANDLE.describe_tile = PointedTile;
        }

        if(pointer.getColor() != Pointer.Colors.Green)
        {
            interactable_hover_time += Time.deltaTime;
            if(interactable_hover_time > interactable_hover_time_threshold)
            {
                interactable_hover_time = 0;
                _GUI_HANDLE.hide();
                _GUI_HANDLE.describe_tile = null;
            }
        }
        else
        {
            interactable_hover_time = 0;
        }
    }

    public Vector3 GetPreciseMousePos()
    {
        return MousePointerHit.point;
    }

    //Find Mouse pos on grid
    void PlayerMouseUpdate()
    {
        MousePointer = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        Physics.Raycast(MousePointer, out MousePointerHit);
        OnMouseBlock = new Vector3(Mathf.FloorToInt(MousePointerHit.point.x), 0.5f, Mathf.FloorToInt(MousePointerHit.point.z));
    }

    //Handle Object Rotation (to place)
    void PlayerRotateObject()
    {
        if (Input.GetKeyDown(KeyCode.R) && !Input.GetKey(KeyCode.LeftShift))
        {
            RotationOfObject = (RotationOfObject+1)%4;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            
            RotationOfObject = RotationOfObject==0?3:RotationOfObject-1;
        }
    }

    void MouseUse()
    {
        if (state == INTERFACE_STATE.STANDARD_INTERACT)
        {
            StandardBeheivor();
        }
        else if(state == INTERFACE_STATE.DRONES_INTERACT)
        {
            DroneInterfaceBeheivor();
        }
    }

    void StandardBeheivor()
    {
        if (Input.GetMouseButton(0))
        {
            if (PointedTile.CanPlaceObject(ObjectInHands))
            {
                PlaceObjectOnPointedBlock();
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Interakcja jeżeli postawiony blok
            if (PointedTile.isPlacedObject())
            {
                _GUI_HANDLE.describe_tile = PointedTile;
                _GUI_HANDLE.show();
                if (PointedTile.PlacedObject is drone_charger)
                {
                    state = INTERFACE_STATE.DRONES_INTERACT;
                    active_drone_tile = (drone_charger)PointedTile.PlacedObject;
                    active_drone_tile.showArrows = true;
                    circle.setPos(active_drone_tile.position);
                    circle.animateCircle(active_drone_tile.radius, 0.2f);
                }
                return;
            }
        }
        //Delete
        if (Input.GetMouseButtonUp(1))
        {
            if (PointedTile.CanRemoveObject())
            {
                RemoveObjectOnPointedBlock();
            }
            else
            {
                //Interakcja z terenem (Usuń)
            }
            return;
        }
    }

    Arrow getArrow(Vector2 start_pos)
    {
        Arrow arrow = Instantiate(ARROW_PREFAB).GetComponent<Arrow>();
        arrow.StartPosition = start_pos;
        return arrow;
    }

    private Arrow arrow;
    private Tile source;

    void DroneInterfaceBeheivor()
    {
        //Start
        if (Input.GetMouseButtonDown(0))
        {
            if (PointedTile.getItemDevice() != null && active_drone_tile.isInRange(PointedTile))
            {
                arrow = getArrow(PointedTile.PlacedObject.position);
                source = PointedTile;
            }
        }

        if(Input.GetMouseButton(0) && arrow != null)
        {
            //Dragging
            //TODO FIX IT (ADD smoothdump)
            arrow.Destination = new Vector2(Mathf.FloorToInt(GetPreciseMousePos().x), Mathf.FloorToInt(GetPreciseMousePos().z));
        }

    }

    void onArrowDestroy()
    {
        if (arrow != null)
        {
            Destroy(arrow.gameObject);
            arrow = null;
            //On relase arrow is not cleard!
            print("Not Connected!");
        }
    }

    //Call when there is a certainty that on the PointedTile can be placed block.
    void PlaceObjectOnPointedBlock()
    {
        if(ObjectInHands.amount <= 0)
        {
            Debug.Log("Not enought items! ObjectInHands: " + ObjectInHands);
            return;
        }

        //Obj,tile init
        //TODO FIX

        Placeable placed = PrefabHandeler.CreatePrefab(ObjectInHands.type, RotationOfObject, Vector2Int.FloorToInt(Utils.XZtoVector2(OnMouseBlock)));
        ObjectInHands.amount -= 1;


        if (PointedTile.PlaceObject(placed))
        {
            terrain.GetChunkOnCordinates(new Vector2(OnMouseBlock.x, OnMouseBlock.y)).isFresh = false;
        }
        else
        {
            Debug.LogError("Can't place object");
        }

        update_mechanical_device(placed);
        update_fluid_device(placed);


    }

    
    void update_fluid_device(Placeable placed)
    {
        //Test if Placeable is Mechanical Devices
        try
        {
            IFluidDevice placedC = (IFluidDevice)placed;

            var neighbours = terrain.get_neighbours(Utils.XZtoVector2(OnMouseBlock));
            
            fluid_menager.connect_devices(placedC, neighbours);


        }
        catch (Exception)
        {
            //Nothing wrong here. It just handle IMechanicalDevices placedC = (IMechanicalDevices)placed; when placed isn't mechanical device.
        }
    }

    void update_mechanical_device(Placeable placed)
    {
        //Test if Placeable is Mechanical Devices
        try
        {
            IMechanicalDevices placedC = (IMechanicalDevices)placed;

            EnergyMenager.connect_devices(placedC);


        }
        catch (Exception)
        {
            //Nothing wrong here. It just handle IMechanicalDevices placedC = (IMechanicalDevices)placed; when placed isn't mechanical device.
        }
    }

    void RemoveObjectOnPointedBlock()
    {
        var pointed = GetPointedBlock();
        if(pointed.getFluidDevice() != null)
        {
            fluid_menager.remove_device(pointed.getFluidDevice(), terrain.get_neighbours(Utils.XZtoVector2(OnMouseBlock)));
        }
        if(pointed.getMechanicalDevice() != null)
        {
            EnergyMenager.remove_device(pointed.getMechanicalDevice());
        }
        if(pointed.getItemDevice() != null)
        {
            //relese items or sth
            //TODO

            //Drone will do only one connection.

            //remove arrows and links from drones
            foreach(var i in pointed.getItemDevice().connected_drones)
            {
                i.destinations.RemoveAll(x => x.destination == pointed);
            }
        }
        GetPointedBlock().RemoveObject();
    }

    Tile GetPointedBlock(Vector2 pos)
    {
        return terrain.GetTileOnCordinates(pos);
    }

    Tile GetPointedBlock()
    {
        return GetPointedBlock(new Vector2(OnMouseBlock.x, OnMouseBlock.z));
    }

}
