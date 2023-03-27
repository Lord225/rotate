using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public float Radius
    {
        get
        {
            return radius;
        }
        set
        {
            radius = value;
            transform.localScale = new Vector3(radius, radius, 1);
            rend.material.SetFloat("Strength", radius*strength_scale);
        }
    }
    
    public void setPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void setPos(Vector2Int pos)
    {
        setPos(new Vector3(pos.x+0.5f, 0.5f, pos.y+0.5f));
    }

    public void animateCircle(float target_radius, float duration)
    {
        print("Animation"+target_radius+" "+duration);
        StartCoroutine(animate_corutine(target_radius, duration));
    }

    public void animateFadeOut(float duration)
    {
        animateCircle(0, duration);
    }

    IEnumerator animate_corutine(float target_radius, float duration)
    {
        float start = Radius;
        float journey = 0f;

        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);

            Radius = Mathf.Lerp(start, target_radius, percent);

            yield return null;
        }

    }

    private Renderer rend;
    private float radius;
    private const float strength_scale = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        rend.material.SetFloat("Vector1_B032DD0F", radius * strength_scale);
        transform.localScale = new Vector3(radius, radius, 1);
    }

}
