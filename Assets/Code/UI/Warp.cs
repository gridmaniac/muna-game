using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warp : MonoBehaviour
{
    [SerializeField]
    Text label;

    [SerializeField]
    Image knob;

    bool isActive = false;
    Vector3 initialPoint;
    Vector3 extraPoint;

    float amount;
    const float factor = 1.0f / 9.21034f;

    RectTransform rectTransform;

    public int TimeScale { get; private set; } = 1;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        isActive = Input.GetMouseButton(1);
        if (Input.GetMouseButtonDown(1))
            Bake();

        knob.enabled = isActive;
        label.enabled = isActive;

        if (!isActive)
            return;

        extraPoint = Input.mousePosition;

        float distance = Vector3.Distance(initialPoint, extraPoint);
        float sign = Mathf.Sign(extraPoint.y - initialPoint.y);

        knob.fillAmount = amount + distance / 200.0f * sign;
        SetTime(knob.fillAmount);
    }

    public void SetTime(float mul)
    {
        float x = mul / factor;
        TimeScale = Mathf.RoundToInt(Mathf.Clamp(Mathf.Exp(x), 1.0f, 10000.0f));
        label.text = "x" + Mathf.FloorToInt(TimeScale);
    }

    void Bake()
    {
        initialPoint = Input.mousePosition;
        rectTransform.position = initialPoint;

        amount = knob.fillAmount;
    }
}
