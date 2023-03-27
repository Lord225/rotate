using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public enum Colors
    {
        Red,
        Green,
        Gray
    }

    //Settings
    public float speed;
    public Color RedLitColor;
    public Color RedAlbedoColor;
    public Color GreenLitColor;
    public Color GreenAlbedoColor;
    public Color GrayLitColor;
    public Color GrayAlbedoColor;


    public Vector3 TargetPosition { get; set; }

    private Colors Currentcolor;
    private Material material;
    private Vector3 Postition;
    private Vector3 acc;
    
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    public void SetColor(Colors color)
    {
        if (Currentcolor != color)
        {
            switch (color)
            {
                case Colors.Red:
                    material.SetColor("_LitColor", RedLitColor);
                    material.SetColor("_AlbedoColor", RedAlbedoColor);
                    break;
                case Colors.Green:
                    material.SetColor("_LitColor", GreenLitColor);
                    material.SetColor("_AlbedoColor", GreenAlbedoColor);
                    break;
                case Colors.Gray:
                    material.SetColor("_LitColor", GrayLitColor);
                    material.SetColor("_AlbedoColor", GrayAlbedoColor);
                    break;
                default:
                    break;
            }
            Currentcolor = color;
        }
    }
    public Colors getColor()
    {
        return Currentcolor;
    }

    // Update is called once per frame
    void Update()
    {
        Postition = Vector3.SmoothDamp(Postition, TargetPosition, ref acc, speed,1000,Time.deltaTime);
        transform.position = Postition;
    }
}
