using UnityEngine;
using System.Collections;

public class CelestialObject : MonoBehaviour {
    public CelestialObject co;
    public LineRenderer orbitRenderer;

    public GameObject playerOrbit;
    public PlayerOrbit po;

    public Gravity gravity;
    public Vector2 impulse;
    public Transform socket;
    public Transform launchSide;
    public int scope;
    public float density;

    public Orbit orbit;

    public GameObject body;
    new CircleCollider2D collider;
    new Rigidbody2D rigidbody;
    Zoom zoom;
    Warp warp;

    [SerializeField]
    Transform ghost;

    public float M;

    public float Radius {
        get { return collider.radius; }
    }

    float G = 6.674f * Mathf.Pow(10, -11);
    float u, velocity, rot;

    public float mass;
    public float phase;
    public float time;

    void Awake()
    {
        collider = body.GetComponent<CircleCollider2D>();
        rigidbody = body.GetComponent<Rigidbody2D>();
        mass = gravity.ForceMagnitude / G * -1.0f;
        //collider.radius = Mathf.Pow((3.0f * mass) / (4.0f * Mathf.PI * density), 1.0f / 3.0f);

        var go = Instantiate(playerOrbit);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector2.zero;

        po = go.GetComponent<PlayerOrbit>();
    }

    void Start()
    {
        zoom = FindObjectOfType<Zoom>();
        warp = FindObjectOfType<Warp>();

        DrawOrbit();

        float soi = 1000000.0f;
        if (co != null)
            soi = 0.9431f * orbit.a * Mathf.Pow((mass / co.mass), 2.0f / 5.0f);

        gravity.SetSOI(soi);
    }

    void DrawOrbit()
    {
        if (co == null)
            return;

        u = (G * 100.0f * Mathf.Pow(co.mass, 3.0f)) / Mathf.Pow((co.mass + mass), 2.0f);

        orbit = new Orbit();
        orbit.State.r = transform.localPosition;
        orbit.State.v = impulse;
        orbit.GravityCenter = Vector2.zero;
        orbit.u = u;

        if (!orbit.Calculate())
            return;

        var orbitPath = orbit.GetPath(500);

        orbitRenderer.positionCount = 500;
        for (int i = 0; i < 500; i++)
            orbitRenderer.SetPosition(i, orbitPath[i]);

        phase = Random.Range(0.0f, orbit.T);
    }

    void Update()
    {
        if (orbitRenderer)
            orbitRenderer.widthMultiplier = zoom.lineWidth;

        po.orbitRenderer.widthMultiplier = zoom.lineWidth;
        po.maneuverRenderer.widthMultiplier = zoom.lineWidth;
        po.peSprite.localScale = zoom.pointScale;
        po.apSprite.localScale = zoom.pointScale;
        po.manSprite.localScale = zoom.pointScale;

        //DrawOrbit();
    }

    public void ShowGhost(float t)
    {
        if (co == null)
            return;

        float next = phase + t;
        float k = Mathf.Floor(next / orbit.T);

        Vector3 pos = orbit.PosAtTime(next - orbit.T * k);
        ghost.localPosition = pos;
    }

    void FixedUpdate()
    {
        if (co != null)
        {
            // TODO: PERIOD OR ACTUAL VELOCITY!!!
            // velocity = Mathf.Sqrt((G * co.Mass) / (co.radius + transform.localPosition.magnitude)) * 0.0005f;
            // velocity = 10.0f * Mathf.Sqrt(co.gravity.ForceMagnitude2 * -1.0f * (2.0f / transform.localPosition.magnitude - 1.0f / orbit.Kepler.a));
            
            phase += Time.fixedDeltaTime * warp.TimeScale;

            //float s = 2.0f * Mathf.PI / orbit.T;

            if (phase > orbit.T)
                phase = 0.0f;

            //Vector3 pos = orbit.AtAnoma(phase * s);
            //Vector3 pos = orbit.AtAnoma(orbit.O);
            transform.localPosition = orbit.PosAtTime(phase);

            time = orbit.Time();


            //if (phase > 2.0f * Mathf.PI)
            //    phase = 0.0f;

            //DrawOrbit();
        }

        //rot += 0.01f;
        //if (rot > 360) rot = 0.0f;
        //rigidbody.MoveRotation(rot);

    }
}
