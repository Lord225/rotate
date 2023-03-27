using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ObjectOnScene <= Placable 
 *  |     |
 *  |    IEnergyDevices
 * IMechanicalDevices
 * 
 * ect...
 * 
 * 
 */

//TODO NA PRZSZYŁOŚĆ KURWA:
//JAK NASTĘPNYM RAZEM BĘDZIESZ ROBIŁ STAWIANIE OBIEKTÓW NA SIATCE TO NIE SPIERDOL TAK BARDZO SYSTEMU WYKRYWANIA CZY MOŻNA/NIE MOŻNA PISTAWIĆ BLOKU
//BEST:
//Twaorzysz "Default mask" i dla każdego dziecka definiujesz przeciążenie bloków ktore są inne
//Np domylśnie nie możesz stawiać na Drzewach, Wodzie i ewentualne zmieniasz to w ustawieniach konkretnego bloku.
//All Mechanical Devices Use it as refrence to materials properties

//All Object that are on scene
public abstract class Placeable : MonoBehaviour
{
    
    //You can find object in graph using ID. It will have same id in diffrent environments (Mechanical,Electric ect)
    public int Id { get; protected set; }
    //global counter for "public int Id { get; protected set; }" WARNING: ID 0, 1 are reserved for graph's in,out flow!
    public static int id_counter = 1;

    //relative rotation of object
    public int rotation;
    public Animator anim;
    public MeshRenderer render;
    public MeshFilter filter;
    
    public Vector2Int position { get; set; }
    public abstract Item.Items item_link { get; }

    //Initilize graphic frontend
    public void Init_Frontend(Vector2Int pos, int rotation)
    {
        this.position = pos;
        id_counter++;
        Id = id_counter;

        transform.rotation = Quaternion.Euler(-90, 0, 90 * rotation);
        
        switch (rotation)
        {
            case 0:
                transform.position = new Vector3(position.x, transform.position.y, position.y);
                break;
            case 1:
                transform.position = new Vector3(position.x, transform.position.y, position.y + 1);
                break;
            case 2:
                transform.position = new Vector3(position.x + 1, transform.position.y, position.y + 1);
                break;
            case 3:
                transform.position = new Vector3(position.x + 1, transform.position.y, position.y);
                break;
        }
        this.rotation = rotation;

        this.gameObject.name += " ID: " + Id;
        anim   = GetComponent<Animator>();
        render = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
    }

    //Check Side after adding rotation to it ("local rotation" to "global")
    public Sides MatchTouchingSide(Sides side)
    {
        return (Sides)(((int)side + rotation) % 4);
    }

    public Sides GetRotation()
    {
        return (Sides)((rotation+1)%4);
    }


    public static Sides GetSideThatIsRelativeOnEdge(Sides side)
    {
        switch (side)
        {
            case Sides.Right:
                return Sides.Left;
            case Sides.Down:
                return Sides.Up;
            case Sides.Left:
                return Sides.Right;
            case Sides.Up:
                return Sides.Down;
        }
        return 0;
    }

    public abstract void handle_animations();

    public void Hide()
    {
        anim.enabled = false;
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var item in renderers)
        {
            item.enabled = false;
        }
    }
    public void Show()
    {
        anim.enabled = true;
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var item in renderers)
        {
            item.enabled = true;
        }
    }
}
