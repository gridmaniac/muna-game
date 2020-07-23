using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maneuver : MonoBehaviour
{
    public bool IsActive { get; private set; }

    Vector3 initialPoint;
    Vector3 extraPoint;

    RectTransform rectTransform;
    Camera cam;

    [SerializeField]
    RectTransform arrow;

    float minDistance = 0.0f;
    float maxDistance = 356.0f;

    float maxV = 3;

    Vector2 centerPos;
    public Vector2 V;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        IsActive = Input.GetMouseButton(0);
        if (Input.GetMouseButtonDown(0))
            Init();

        arrow.gameObject.SetActive(false);

        if (!IsActive)
            return;

        extraPoint = Input.mousePosition;

        float distance = Vector3.Distance(initialPoint, extraPoint);
        if (distance > minDistance)
        {
            Vector3 direction = (initialPoint - extraPoint).normalized;
            float dotProduct = Vector2.Dot(Vector2.right, direction);
            if (dotProduct > 0.0f)
                arrow.rotation = Quaternion.AngleAxis(-Vector3.Angle(Vector3.up, direction), Vector3.forward);
            else
                arrow.rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, direction), Vector3.forward);

            V = direction * (maxV / maxDistance * distance);
        }
    }

    void Init()
    {
        // initialPoint = Input.mousePosition;
        rectTransform.position = centerPos;
        initialPoint = cam.WorldToScreenPoint(centerPos);
    }

    public void SetPos(Vector2 pos)
    {
        centerPos = pos;
    }
}
