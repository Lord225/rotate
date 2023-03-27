using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Placable_3D_GUI_HANDLE : MonoBehaviour
{
    private Transform cam_transfrom;
    private Transform ScaleTransform;
    private Animator submenu_animator;
    private GameObject submenu_obj;
    private Tile val_describe_tile;

    public Text title_text;
    public CameraMovement player;

    public Tile describe_tile
    {
        get {
            return val_describe_tile;
        }
        set {
            val_describe_tile = value;
            update_content();
        }
    }

    bool should_hide;
   

    // Start is called before the first frame update
    void Start()
    {
        cam_transfrom = GameObject.Find("CameraRotationHolder").transform;
        player = GameObject.Find("CameraRotationHolder").GetComponent<CameraMovement>();
        ScaleTransform = GameObject.Find("ScaleHolder").transform;

        submenu_obj = GameObject.Find("submenu_background");
        submenu_animator = submenu_obj.GetComponent<Animator>();
        
        submenu_obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float rot = (cam_transfrom.rotation.eulerAngles.y*Mathf.Deg2Rad/Mathf.PI)*2;
        int rot_rounded = Mathf.RoundToInt(rot)%4;

        transform.rotation = Quaternion.Euler(0, rot_rounded * Mathf.PI * 0.5f * Mathf.Rad2Deg, 0);
        ScaleTransform.localScale = Vector3.one * (player.height)/6;

        if (should_hide && submenu_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_hide")) 
        {
            submenu_obj.SetActive(false);
            should_hide = false;
        }
    }

    private void update_content()
    {
        if (val_describe_tile != null && val_describe_tile.isPlacedObject())
        {
            title_text.text = val_describe_tile.PlacedObject.ToString();
        }
    }

    public void show()
    {
        should_hide = false;
        submenu_obj.SetActive(true);
        submenu_animator.SetTrigger("Intro");
    }
    public void hide()
    {
        submenu_animator.SetTrigger("Outro");
        should_hide = true;
    }
}
