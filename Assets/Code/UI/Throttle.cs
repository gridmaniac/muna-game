using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Throttle : MonoBehaviour
{
    public bool IsActive { get; private set; }

    Vector3 initialPoint;
    Vector3 extraPoint;

    RectTransform rectTransform;

    [SerializeField]
    RectTransform ruler;

    [SerializeField]
    RectTransform prograde;

    [SerializeField]
    RectTransform retrograde;

    [SerializeField]
    RectTransform arrow;

    [SerializeField]
    Transform player;

    float minDistance = 8.0f;
    float maxDistance = 356.0f;
    float halfDistance;

    float maxForce = 0.008f;
    float maxDensity = 2;

    public float Force;
    public float Density;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        halfDistance = maxDistance * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        IsActive = Input.GetMouseButton(0);
        if (Input.GetMouseButtonDown(0))
            Init();

        ruler.gameObject.SetActive(IsActive);
        prograde.gameObject.SetActive(IsActive);
        retrograde.gameObject.SetActive(IsActive);
        arrow.gameObject.SetActive(IsActive);

        if (!IsActive)
        {
            Force = 0;
            Density = 0; 
            return;
        } 

        extraPoint = Input.mousePosition;

        float distance = Vector3.Distance(initialPoint, extraPoint);

        if (distance < minDistance)
            ruler.rotation = player.rotation;
        else
        {
            Vector3 direction = (initialPoint - extraPoint).normalized;
            float dotProduct = Vector2.Dot(Vector2.right, direction);
            if (dotProduct > 0.0f)
                ruler.rotation = Quaternion.AngleAxis(-Vector3.Angle(Vector3.up, direction), Vector3.forward);
            else
                ruler.rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, direction), Vector3.forward);

            player.rotation = Quaternion.Lerp(player.rotation, ruler.rotation, .1f);
        }

        if (distance > halfDistance)
        {
            distance = Mathf.Clamp(distance, 0, maxDistance);

            Force = maxForce / halfDistance * (distance - halfDistance);
            Density = maxDensity / halfDistance * (distance - halfDistance);
        }
    }

    void Init()
    {
        initialPoint = Input.mousePosition;
        rectTransform.position = initialPoint;
    }

    public void SetV(Vector2 v)
    {
        float dotProductV = Vector2.Dot(Vector2.right, v);
        var vRot = Quaternion.identity;

        if (dotProductV > 0.0f)
            vRot = Quaternion.AngleAxis(-Vector3.Angle(Vector3.up, v), Vector3.forward);
        else
            vRot = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, v), Vector3.forward);

        prograde.rotation = vRot;
        retrograde.rotation = vRot;
    }

    public void SetM(Vector2 m)
    {
        float dotProductV = Vector2.Dot(Vector2.right, m);
        var vRot = Quaternion.identity;

        if (dotProductV > 0.0f)
            vRot = Quaternion.AngleAxis(-Vector3.Angle(Vector3.up, m), Vector3.forward);
        else
            vRot = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, m), Vector3.forward);

        arrow.rotation = vRot;
    }
}
