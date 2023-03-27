using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int quality = 16;
    public Vector2 Destination = new Vector2( -5, -5 );

    public Vector2 StartPosition
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = new Vector3(value.x, 0.0f, value.y);
        }
    }

    private float a;
    private LineRenderer lineRenderer;

    private Vector3 MidOffset = new Vector3(0.5f, -0.5f, 0);
    public float Offset = 0.2f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        updateLine();
    }

    void updateLine()
    {
        Vector2 dir = (new Vector2(this.transform.position.x, this.transform.position.z) - Destination);
        float distance = dir.magnitude;
        Vector2 angle = dir.normalized;
        a = Mathf.Pow(distance,0.81f)+2f;

        Vector3[] positions = new Vector3[quality];

        for (int i = 0; i < positions.Length; i++)
        {
            float j = (float)(i+1) / positions.Length;
            positions[i] = new Vector3(-angle.x * j * distance + Vector2.Perpendicular(angle).x * Offset, 
                                        angle.y * j * distance + Vector2.Perpendicular(angle).y * Offset, 
                                        getArc(j)) + MidOffset;
        }

        lineRenderer.positionCount = quality;
        lineRenderer.SetPositions(positions);

    }
    public void SetVisible(bool shouldBeVisible)
    {
        lineRenderer.enabled = shouldBeVisible;
    }

    float getArc(float x)
    {
        return (-Mathf.Pow(x - 0.5f, 2) + 0.25f) * a;
    }
}
