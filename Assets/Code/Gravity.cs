using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    PointEffector2D effector;
    new CircleCollider2D collider;

    public float Radius { get { return collider.radius; } }

    public float ForceMagnitude { get { return GetComponent<PointEffector2D>().forceMagnitude; } }

    void Awake()
    {
        effector = GetComponent<PointEffector2D>();
        collider = GetComponent<CircleCollider2D>();
    }

    public void Disable()
    {
        effector.enabled = false;
    }

    public void Enable()
    {
        effector.enabled = true;
    }

    public void SetSOI(float radius)
    {
        collider.radius = radius;
    }
}
