using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour {
    Camera cam;

    float targetZoom = 10000.0f;
    float maxZoom = 20000.0f;
    float minZoom = 50.0f;

    public float lineWidth = 1.0f;
    public float panStep;
    public Vector2 pointScale = new Vector2(2.0f, 2.0f);

    // Use this for initialization
    void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f) // forward
        {
            targetZoom -= targetZoom / 20.0f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            targetZoom += targetZoom / 20.0f;
        }

        if (targetZoom > maxZoom)
            targetZoom = maxZoom;

        if (targetZoom < minZoom)
            targetZoom = minZoom;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.unscaledDeltaTime * 20.0f);

        lineWidth = cam.orthographicSize / 400.0f;
        panStep = minZoom / maxZoom * cam.orthographicSize;
        pointScale = new Vector2(cam.orthographicSize / 150.0f, cam.orthographicSize / 150.0f);

        //cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 10.0f, 1000.0f);
    }
}
