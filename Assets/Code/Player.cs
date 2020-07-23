using MUNA.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{    
    public CelestialObject co;

    [SerializeField]
    Throttle throttle;

    [SerializeField]
    Maneuver maneuver;

    public Transform ts;

    public CelestialObject[] cos;

    public new Rigidbody2D rigidbody;
    ParticleSystem particles;
    Camera cam;
    Zoom zoom;
    Warp warp;

    float G = 6.674f * Mathf.Pow(10, -11);
    float u;

    Orbit orbit;
    Orbit maneuverOrbit;

    public List<Orbit> trajectory;

    ManeuverSet tryManeuver;

    float time;

    BodyState state;
    PhysicsMode physics;

    struct ManeuverSet
    {
        public Vector2 pos;
        public float anomaly;
        public bool isPossible;
        public float time;
    }

    Vector2 maneuverPos;

    int orbitResolution = 2000;

    public float timeToManeuver;
    public float maneuverAnomaly;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        particles = GetComponentInChildren<ParticleSystem>();
        orbit = new Orbit();
        maneuverOrbit = new Orbit();

        cam = Camera.main;
        zoom = FindObjectOfType<Zoom>();
        warp = FindObjectOfType<Warp>();

        state = BodyState.AtRest;
        physics = PhysicsMode.Simulation;

        DrawTrajectory();
    }

    //public void Test()
    //{
    //    float degrees = 270.0f;
    //    float radians = degrees * Mathf.Deg2Rad;
    //    float x = Mathf.Cos(radians);
    //    float y = Mathf.Sin(radians);
    //    Vector3 pos = new Vector2(x, y) * 150.0f;

    //    transform.position = pos;
    //    rigidbody.velocity = new Vector2(2.5f, 0.3f);
    //}

    void FixedUpdate()
    {
        Insertion();

        if (co == null)
            return;

        if (Escape())
            return;

        SetState();

        RotateOnLanding();
        SetApsis();

        if (throttle.IsActive && physics == PhysicsMode.Approximation || state == BodyState.AtRest)
            SetPhysicsMode(PhysicsMode.Simulation);

        if (!throttle.IsActive && physics == PhysicsMode.Simulation && state == BodyState.AtMotion)
            SetPhysicsMode(PhysicsMode.Approximation);

        switch (physics)
        {
            case PhysicsMode.Simulation:
                DrawTrajectory();
                break;

            case PhysicsMode.Approximation:
                time += Time.fixedDeltaTime * warp.TimeScale;

                if (trajectory[q].form == OrbitForm.Ellipse && time > trajectory[q].T)
                    time = 0;

                transform.localPosition = trajectory[q].PosAtTime(time);

                break;
        }

        throttle.SetV(rigidbody.velocity.normalized);

        SetDF(throttle.Density, throttle.Force);

        if (!maneuver.IsActive && !throttle.IsActive)
        {
            tryManeuver = GetManeuver();

            if (tryManeuver.isPossible)
                co.po.manSprite.localPosition = tryManeuver.pos;
            else
                co.po.manSprite.localPosition = maneuverPos;

            throttle.gameObject.SetActive(!tryManeuver.isPossible);
            maneuver.gameObject.SetActive(tryManeuver.isPossible);

            maneuver.SetPos(co.transform.TransformPoint(tryManeuver.pos));
        }

        if (maneuver.IsActive)
            DrawManeuver();
    }

    void SetPhysicsMode(PhysicsMode mode)
    {
        switch(mode)
        {
            case PhysicsMode.Simulation:
                rigidbody.isKinematic = false;

                if (state == BodyState.AtMotion)
                    rigidbody.velocity = trajectory[q].GetVelocity();

                time = 0;

                break;

            case PhysicsMode.Approximation:
                rigidbody.isKinematic = true;
                time = trajectory[q].Time();

                break;
        }

        physics = mode;
    }

    void Insertion()
    {
        var x = GetSOICO(transform.position);

        if (co == null || x.scope > co.scope)
        {
            transform.SetParent(x.socket);
            cam.transform.SetParent(x.socket);

            if (co != null)
                co.gravity.Disable();

            co = x;
            co.gravity.Enable();

            u = (G * 100.0f * Mathf.Pow(co.mass, 3.0f)) / Mathf.Pow((co.mass + rigidbody.mass), 2.0f);

            SetPhysicsMode(PhysicsMode.Simulation);
            DrawTrajectory();
        }

    }

    CelestialObject GetSOICO(Vector2 pos)
    {
        foreach (var x in cos)
        {
            float distance = Vector3.Distance(pos, x.transform.position);
            if (distance < x.gravity.Radius - 200.0f)
                return x;
        }

        return null;
    }

    bool Escape()
    {
        if (transform.localPosition.magnitude <= co.gravity.Radius)
            return false;

        co.gravity.Disable();
        co = null;
        return true;
    }

    void SetState()
    {
        //if (transform.localPosition.magnitude < co.Radius + 20.0f)
        if (rigidbody.velocity.magnitude == 0)
            state = BodyState.AtRest;
        else
            state = BodyState.AtMotion;
    }

    void RotateOnLanding()
    {
        if (state == BodyState.AtMotion)
            return;

        var dir = transform.localPosition;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90.0f, Vector3.forward), 0.2f);
    }

    void SetApsis()
    {
        //if (state == BodyState.AtRest)
        //{
        //    periapsisSprite.localPosition = Vector2.zero;
        //    apoapsisSprite.localPosition = Vector2.zero;
        //    return;
        //}

        //if (!float.IsNaN(orbit.Kepler.Pe.x))
        co.po.peSprite.localPosition = trajectory[q].Kepler.Pe;
        //if (!float.IsNaN(orbit.Kepler.Ap.x))
        co.po.apSprite.localPosition = trajectory[q].Kepler.Ap;
    }

    void SetDF(float density, float force)
    {
        particles.startLifetime = density;
        rigidbody.AddForce(transform.up * force);
    }

    ManeuverSet GetManeuver()
    {
        var set = new ManeuverSet();

        Vector2 cursor = cam.ScreenToWorldPoint(Input.mousePosition);
        cursor = co.transform.InverseTransformPoint(cursor);

        if (state == BodyState.AtRest)
        {
            if (Vector2.Distance(cursor, transform.localPosition) < 5.0f)
            {
                set.pos = transform.localPosition;
                set.isPossible = true;
            }

            return set;
        }
        
        float distance = 5;
        var rough = trajectory[q].GetPath(orbitResolution);

        for (int i = 0; i < rough.Length; i++)
        {
            if (Vector2.Distance(rough[i], cursor) < distance)
            {
                set.pos = rough[i];
                distance = Vector2.Distance(rough[i], cursor);

                // use cached paths
                set.anomaly = trajectory[q].AnomalyAtPath(orbitResolution, i);
                set.time = trajectory[q].TimeToApproach(set.anomaly);

                timeToManeuver = set.time;
                maneuverAnomaly = set.anomaly;

                set.isPossible = true;
            }
        }

        return set;
    }

    int q;

    void DrawTrajectory()
    {
        q = 0;
        trajectory = new List<Orbit>();

        foreach (var x in cos)
            x.po.orbitRenderer.positionCount = 0;

        var o0 = new Orbit();

        o0.State.r = transform.localPosition;
        o0.State.v = rigidbody.velocity;
        o0.GravityCenter = Vector2.zero;
        o0.u = u;
        o0.isDubugging = true;

        trajectory.Add(o0);

        if (!o0.Calculate())
            return;

        float t = o0.Ez;
        float revs = 2.0f * Mathf.PI;
        //float step = 2.0f* Mathf.PI / o0.T;
        float step = 0.02f;
        float e = 0.00001f;

        while (t < o0.Ez + revs)
        {
            var pos = o0.PosAtAnomaly(t);
            if (pos.magnitude > co.gravity.Radius)
            {
                //while (step > e)
                //{
                //    step *= 0.5f;

                //    pos = o0.PosAtAnomaly(t);
                //    if (pos.magnitude > co.gravity.Radius)
                //}

                co.po.escSprite.localPosition = pos; //co.transform.TransformPoint(pos);
                break;
            }

            t += step;
        }

        o0.SetEscape(t);

        var orbitPath = o0.GetPath(orbitResolution);
        int points = orbitPath.Length;
        co.po.orbitRenderer.positionCount = points;

        for (int i = 0; i < points; i++)
        {
            //if (Vector2.Distance(Vector2.zero, orbitPath[i]) > 2000)
            //    continue;

            co.po.orbitRenderer.SetPosition(i, orbitPath[i]);
        }
    }

    void SearchTransfer()
    {
        if (co == null)
            return;

        var target = GameObject.Find("Muna").GetComponent<CelestialObject>();

        float soi = 0.9431f * target.orbit.a * Mathf.Pow((target.mass / target.co.mass), 2.0f / 5.0f);

        float radius = maneuverOrbit.Ra + soi;

        float angle = Mathf.Acos((target.orbit.l / radius - 1.0f) / target.orbit.e);

        hasPossibleEncounter = !float.IsNaN(angle) && angle != 0.0f;
    }
    bool hasPossibleEncounter;

    //void SearchTransfer()
    //{
    //    if (co == null)
    //        return;

    //    var tco = GameObject.Find("Muna").GetComponent<CelestialObject>();

    //    var rough1 = tco.orbit.GetPath(250);
    //    var rough2 = maneur.GetPath(250);

    //    bool isFound = false;

    //    for (int i = 0; i < 250; i++)
    //    {
    //        for (int j = 0; j < 250; j++)
    //        {
    //            if (Vector2.Distance(rough1[i], rough2[j]) < tco.gravity.Radius)
    //            {
    //                transfer1.localPosition = rough2[j];
    //                float an = maneur.AnomalyAtPath(250, j);
    //                dt = maneur.TimeToApproach(an - Mathf.PI * 0.5f);
    //                tco.ShowGhost(dt);

    //                isFound = true;
    //                break;
    //            }
    //        }

    //        if (isFound)
    //            break;
    //    }
    //}

    void DrawManeuver()
    {
        if (!tryManeuver.isPossible)
            return;

        var dv = trajectory[q].GetVelocityAt(tryManeuver.anomaly);

        throttle.SetM(maneuver.V);

        maneuverPos = tryManeuver.pos;

        maneuverOrbit.State.r = tryManeuver.pos;
        maneuverOrbit.State.v = dv + maneuver.V;
        maneuverOrbit.GravityCenter = Vector2.zero;
        maneuverOrbit.u = u;

        if (!maneuverOrbit.Calculate())
        {
            co.po.maneuverRenderer.positionCount = 0;
            return;
        }

        var maneuverPath = maneuverOrbit.GetPath(2000);

        co.po.maneuverRenderer.positionCount = 2000;
        for (int i = 0; i < 2000; i++)
        {
            co.po.maneuverRenderer.SetPosition(i, maneuverPath[i]);
        }
    }
}
