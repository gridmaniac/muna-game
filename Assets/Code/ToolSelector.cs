using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelector : MonoBehaviour
{
    public bool IsActive { get; private set; }

    Vector3 initialPoint;
    Vector3 extraPoint;

    RectTransform rectTransform;

    [SerializeField]
    RectTransform gizmo;

    [SerializeField]
    RectTransform arrow;

    [SerializeField]
    RectTransform throttleText;
    float throttleTextScale;

    [SerializeField]
    RectTransform warpText;
    float warpTextScale;

    [SerializeField]
    RectTransform maneurText;
    float maneurTextScale;

    [SerializeField]
    Throttle throttle;

    [SerializeField]
    Warp warp;

    //GameObject maneur;

    float minDistance = 1.0f;
    int selected = 0;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        IsActive = Input.GetMouseButton(1);
        if (Input.GetMouseButtonDown(1))
            Init();

        if (Input.GetMouseButtonUp(1))
            Select();

        gizmo.gameObject.SetActive(IsActive);

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

            throttleTextScale = 1;
            warpTextScale = 1;
            maneurTextScale = 1;

            float angle = arrow.rotation.eulerAngles.magnitude;
            if (angle <= 315 && angle > 225)
            {
                throttleTextScale = 1.5f;
                selected = 1;
            } else if (angle <= 225 && angle > 135)
            {
                warpTextScale = 1.5f;
                selected = 2;
            } else if (angle <= 135 && angle > 45)
            {
                maneurTextScale = 1.5f;
                selected = 3;
            } else
            {
                selected = 0;
            }

            throttleText.localScale = new Vector2(throttleTextScale, throttleTextScale);
            warpText.localScale = new Vector2(warpTextScale, warpTextScale);
            maneurText.localScale = new Vector2(maneurTextScale, maneurTextScale);
        }
    }

    void Select()
    {
        warp.SetTime(0.01f);

        throttle.gameObject.SetActive(false);
        warp.gameObject.SetActive(false);

        switch (selected)
        {
            case 1:
                throttle.gameObject.SetActive(true);
                break;

            case 2:
                warp.gameObject.SetActive(true);
                break;
        }
    }

    void Init()
    {
        initialPoint = Input.mousePosition;
        rectTransform.position = initialPoint;
    }
}
