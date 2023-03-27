using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObjectAnim : MonoBehaviour
{
    //Settings
    public GameObject PlayerHolder;
    public float speed;
    public float rotationSpeed;
    public float MaximumSwing;
    public Material PlaceSemiTransparentMaterialHere;

    //Parametrs
    public bool Visible;
    public int RotationOfObject { get; set; }
    
    //Links
    private PlaceblePrefabHandeler Prefabs;
    private PlayerHandeler Player;
    private Placeable CurrentMachineInstanceGhost;

    //Priv Stuff
    private Quaternion TargetRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion Yrot;
    private Quaternion Xrot;
    private Item.Items CurrentObject;
    private Vector3 acc;
    private Vector3 mouseAcc;
    private Vector3 priveusMousePos;
    private Vector3 Postition;
    private Vector3 Rotation;
    private readonly Vector3 HalfOffset = new Vector3(0.5f,0,0.5f);
    private bool HideState = true;

    void Start()
    {
        Prefabs = PlayerHolder.GetComponent<PlaceblePrefabHandeler>();
        Player = PlayerHolder.GetComponent<PlayerHandeler>();
        Yrot = Quaternion.Euler(MaximumSwing, 0f, 0f);
        Xrot = Quaternion.Euler(0, 0f, MaximumSwing);
    }

    // Update is called once per frame
    void Update()
    {

        if (HideState == Visible)
        {
            RotateAndMove();
        }
        else
        {
            ChangeVisibilityState();
        }

        if (Player.ObjectInHands != null)
        {
            if (CurrentObject != Player.ObjectInHands.type)
            {
                ChangeObject();
            }
        }
        else
        {
            if (CurrentObject != Item.Items.None)
            {
                ChangeObject();
            }
        }

        DetermineVisibilityState();

    }

    void ChangeVisibilityState()
    {
        if (CurrentMachineInstanceGhost != null)
        {
            var RenderersOfChildren = CurrentMachineInstanceGhost.gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var item in RenderersOfChildren)
            {
                item.enabled = Visible;
            }
            HideState = Visible;
        }
    }

    void DetermineVisibilityState()
    {
        if (Player.PointedTile != null)
        {
            Visible = Terrain.CanPlaceOnThisBlock(Player.PointedTile.type) && 
                      Player.PointedTile.CanPlaceObject(Player.ObjectInHands) && 
                      Player.state == PlayerHandeler.INTERFACE_STATE.STANDARD_INTERACT;
        }
    }

    void RotateAndMove()
    {
        Postition = Vector3.SmoothDamp(Postition, new Vector3(Player.OnMouseBlock.x + 0.5f, 0.75f, Player.OnMouseBlock.z + 0.5f), ref acc, speed, 1000, Time.deltaTime);
        transform.position = Postition;
        mouseAcc = priveusMousePos - Player.GetPreciseMousePos();
        mouseAcc.x = Mathf.Abs(mouseAcc.x) < 0.05f ? 0 : mouseAcc.x;
        mouseAcc.z = Mathf.Abs(mouseAcc.z) < 0.05f ? 0 : mouseAcc.z;


        //Treshold for mouse acc
        priveusMousePos = Player.GetPreciseMousePos();

        if (CurrentMachineInstanceGhost != null)
        {
            TargetRotation = Quaternion.LerpUnclamped(Quaternion.identity, Xrot, Mathf.Clamp(-mouseAcc.x, -1.0f, 1.0f)) *
                             Quaternion.LerpUnclamped(Quaternion.identity, Yrot, Mathf.Clamp(-mouseAcc.z, -1.0f, 1.0f)) *
                             Quaternion.Euler(0, 90 * Player.RotationOfObject, 0);


            transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void ChangeObject()
    {
        Quaternion Rot = Quaternion.Euler(-90,0,0);
        if (CurrentMachineInstanceGhost != null)
        {
            Rot = CurrentMachineInstanceGhost.gameObject.transform.rotation;
            Destroy(CurrentMachineInstanceGhost.gameObject);

        }

        if (Player.ObjectInHands.type != Item.Items.None)
        {
            //TODO FIX
            CurrentMachineInstanceGhost = Prefabs.CreatePrefab(Player.ObjectInHands.type, transform.position);
            if(CurrentMachineInstanceGhost is null)
            {
                Destroy(CurrentMachineInstanceGhost);
                Debug.LogWarning("null ghost!");
                return;
            }
            CurrentMachineInstanceGhost.gameObject.transform.rotation = Rot; 
            CurrentMachineInstanceGhost.enabled = false;
            CurrentMachineInstanceGhost.gameObject.transform.parent = this.transform;
            CurrentMachineInstanceGhost.gameObject.transform.localPosition -= HalfOffset;


            var RenderersOfChildren = CurrentMachineInstanceGhost.gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var item in RenderersOfChildren)
            {
                item.sharedMaterial = PlaceSemiTransparentMaterialHere;
            }
        }
        else
        {
            CurrentMachineInstanceGhost = null;
        }

        if (Player.ObjectInHands != null)
        {
            CurrentObject = Player.ObjectInHands.type;
        }
        else
        {
            CurrentObject = Item.Items.None;
        }

        DetermineVisibilityState();
        ChangeVisibilityState();
    }
}
