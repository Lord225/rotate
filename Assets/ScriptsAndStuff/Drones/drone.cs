using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class drone : MonoBehaviour
{
    public enum drone_state
    {
        flying = 0,         //     Drone is flying
        rotating_init = 1,  //In - set it to activate rotation animation.
        rotating = 2,       //     animation is runing
        rotation_end = 3,   //     On end of animation
        idle = 4            //In - Will do nothing
    }

    private Animator anim;
    private CameraMovement cam_mov;
    private Camera cam;

    public drone_state state;
    public drone_charger main;
    public float energy { private set; get; }

    public bool shouldBeAnimated { set; get; } = true;

    public const float rotation_speed = 300.0f;
    public const float rotation_disacc_effect = 1.1f;
    public const float animation_smoothing = 2f;
    public const float animation_force = 2.0f;
    public const float noise_variation = 1.5f;
    public const float noise_power = 8.0f;
    public const float acc_anim_power = 20.0f;

    public static AnimationCurve rotation_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.75f, 1.2f), new Keyframe(1, 1));
    public static AnimationCurve rotation_curve_derivative = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.37f, 1f), new Keyframe(0.9f, -0.6f), new Keyframe(1, 0f));

    private Vector3 previous_pos;
    private Vector3 acc_holder;

    //dir for movement
    public Vector3 direction;

    private const float anim_threshold = 12.0f;

    private Quaternion destitation_rotation;
    private Quaternion start_rotation;
    private float t;
    private float x_curr, y_curr, x_vel, y_vel;

    public void set_parent(drone_charger parent)
    {
        main = parent;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        cam_mov = GameObject.Find("CameraRotationHolder").GetComponent<CameraMovement>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (cam_mov == null)
        {
            Debug.LogError("Can't Find CameraRotationHolder->CameraMovement");
        }
        if (cam == null)
        {
            Debug.LogError("Can't Find Main Camera->Camera");
        }
    }
   
    private void move_anims()
    {
        if (state == drone_state.rotating_init)
        {
            destitation_rotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg, 0);

            start_rotation = transform.rotation;
            state = drone_state.rotating;
            t = 0;
        }

        if(state == drone_state.rotating)
        {
            transform.rotation = Quaternion.LerpUnclamped(start_rotation, destitation_rotation, rotation_curve.Evaluate(t));
            t += Time.deltaTime;
            if (t > 1)
            {
                state = drone_state.rotation_end;
            }
        }
    }

    private void complex_anims()
    {
        float x = 1, y = 1, acc = rotation_curve_derivative.Evaluate(t + 0.2f);
        
        x *= acc * animation_force + (0.5f-Mathf.PerlinNoise(0, t* noise_power))* noise_variation;
        y *= -acc * animation_force + (0.5f - Mathf.PerlinNoise(10.0f, t * noise_power)) * noise_variation;

        float acceleration = acc_holder.magnitude * acc_anim_power;
        
        y += acceleration;
        x += acceleration;

        x = (float)Math.Tanh((double)x);
        y = (float)Math.Tanh((double)y);

        x_curr = Mathf.SmoothDamp(x_curr, x, ref x_vel, 0.1f);
        y_curr = Mathf.SmoothDamp(y_curr, y, ref y_vel, 0.1f);

        anim.SetFloat("RightX", x_curr);
        anim.SetFloat("RightY", y_curr);

    }

    private void determine_camera_visibility()
    {
        Vector3 screenPoint = cam.WorldToViewportPoint(transform.position);
        shouldBeAnimated = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    void Update()
    {
        determine_camera_visibility(); //lazy++
        
        acc_holder = transform.position - previous_pos;

        move_anims(); //lazy

        if (shouldBeAnimated && cam_mov.height < anim_threshold)
        {
            if (!anim.enabled)
            {
                anim.enabled = true;
            }
            complex_anims();
        }
        else
        {
            if (anim.enabled)
            {
                anim.enabled = false;
            }
        }
        previous_pos = transform.position;
    }
}
