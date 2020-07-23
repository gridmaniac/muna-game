using UnityEngine;
using System.Collections;

public class Pan : MonoBehaviour {

    public bool isActive = false;
    public Vector3 initialPoint;
    public Vector3 extraPoint;
    public Vector3 translation;

    public Transform background;
    Vector3 bamp;

    public Camera cam;
    Vector3 camp;

    Zoom zoom;

    void Start()
    {
        zoom = FindObjectOfType<Zoom>();
    }

    // Update is called once per frame
    void Update()
    {
        isActive = Input.GetMouseButton(2);
        if (Input.GetMouseButtonDown(2))
            Init();

        if (!isActive)
            return;

        extraPoint = Input.mousePosition;
        extraPoint = new Vector3(extraPoint.x, extraPoint.y, -10.0f);
        translation = extraPoint - initialPoint;
        translation.z = 0.0f;

        cam.transform.localPosition = camp - translation * zoom.panStep;
        background.position = bamp + translation * 0.001f;
    }

    void Init()
    {
        initialPoint = Input.mousePosition;

        camp = cam.transform.localPosition;
        bamp = background.position;
    }
}
