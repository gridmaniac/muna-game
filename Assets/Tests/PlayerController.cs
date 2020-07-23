using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;

public class PlayerController : MonoBehaviour {

    private new Rigidbody2D rigidbody;

    private PointEffector2D gravitySource;

    public GameObject periapsisSprite;
    public GameObject apoapsisSprite;

    float adjustRatio = 0.1f; //amount of the difference to rotate by

    private List<Vector2> points;
    private Vector2 lastPos;

    private Vector3 gravityCenter;
    private int counter = 0;

    [Header("General")]
    public GameObject planet;
    public float timeScale;

    [Header("Constants")]
    public float G = 6.674f * Mathf.Pow(10, -11);

    [Header("Planet characteristics")]
    public float massOfPlanet = 0.0f;
    public float gravitationalParameter = 0.0f;

    [Header("Satellite")]
    public float velocityMagnitude = 0.0f;
    public float velocitySquareMagnitude = 0.0f;

    public float distance = 0.0f;
    public float gravityForce = 0.0f;

    public float mass = 0.0f;

    [Header("Longitude (Deg)")]
    public float satLong = 0.0f;

    [Header("Symmetric Longitude (Deg)")]
    public float symSatLong = 0.0f;

    [Header("Longitude Profile")]
    public LongitudeProfile longProfile;

    [Header("Direction of movement")]
    public OrbitProfile profile;

    [Header("Orbit")]
    public float orbitalSpeed = 0.0f;
    public float E = 0.0f;
    public float a = 0.0f;
    public float b = 0.0f;
    public float T = 0.0f;

    public float Rp = 0.0f;
    public float Ra = 0.0f;

    public float time = 0.0f;

    public float eccentricity = 0.0f;

    [Header("Eccentric anomaly (Rad)")]
    public float Ec = 0.0f;

    [Header("True anomaly (Rad)")]
    public float V = 0.0f;

    [Header("Mean anomaly (Rad)")]
    public float M = 0.0f;

    [Header("Eccentric anomaly (Deg)")]
    public float Ecd = 0.0f;

    [Header("True anomaly (Deg)")]
    public float Vd = 0.0f;

    [Header("Mean anomaly (Deg)")]
    public float Md = 0.0f;

    [Header("Mean angular motion (Rad)")]
    public float n = 0.0f;

    [Header("Time to periapsis")]
    public float Tp = 0.0f;

    [Header("Time to apoapsis")]
    public float Ta = 0.0f;

    [Header("Periapsis Longitude (Deg)")]
    public float pLong = 0.0f;

    [Header("Periapsis Symmetric Longitude (Deg)")]
    public float symPlong = 0.0f;

    [Header("Approaching Periapsis")]
    public bool apPe = false;

    [Header("Approaching Apoapsis")]
    public bool apAp = false;


    // Use this for initialization
    void Start () {
        this.rigidbody = this.GetComponent<Rigidbody2D>();
        this.gravitySource = this.planet.GetComponent<PointEffector2D>();
        this.gravityCenter = this.gravitySource.transform.position;

        this.massOfPlanet = this.gravitySource.forceMagnitude / G * -1.0f;


        this.gravitationalParameter = (G * 100.0f * Mathf.Pow(massOfPlanet, 3.0f)) / Mathf.Pow((massOfPlanet + mass), 2.0f);

        points = new List<Vector2>();
        points.Add(transform.position);

        this.mass = this.rigidbody.mass;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Time.timeScale -= 10.0f;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Time.timeScale += 10.0f;
        }
        this.timeScale = Time.timeScale;
    }

    void FixedUpdate()
    {
       
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 gravity = (gravityCenter - transform.position).normalized;

            this.rigidbody.AddForce(-gravity * 0.002f, ForceMode2D.Impulse);
        }

        if (Input.GetAxis("Horizontal") != 0.0f)
        {
            this.rigidbody.AddForce(this.transform.right * Mathf.Sign(Input.GetAxis("Horizontal")) * 0.02f);
        }

        /*counter++;

        for (var i = 1; i < points.Count; i ++)
        {
            //Debug.DrawLine(points[i], points[i-1], Color.red);
        }
        if (counter > 1000)
        {
            points.Add(transform.position);
            counter = 0;
        }
        
        if (points.Count > 3000)
        {
            points.RemoveAt(0);
        }*/

        this.velocityMagnitude = this.rigidbody.velocity.magnitude;
        this.velocitySquareMagnitude = this.rigidbody.velocity.sqrMagnitude;

        this.distance = Vector2.Distance(this.rigidbody.worldCenterOfMass, gravityCenter);
        

        this.gravityForce = (this.G * this.massOfPlanet * this.mass) / Mathf.Pow((float)this.distance, 2.0f);


        //pseudo

        Vector2 planetCenter = this.planet.transform.position;
        Vector2 satelliteCenter = this.transform.position;

        r = satelliteCenter - planetCenter;
        v = this.rigidbody.velocity;

    

        e = ((v.sqrMagnitude - gravitationalParameter / r.magnitude) * r - Vector2.Dot(r, v) * v) / gravitationalParameter;
      

        eccentricity = e.magnitude;
        E = v.sqrMagnitude / 2 - gravitationalParameter / r.magnitude;
        a = -gravitationalParameter / (2 * E);
        b = a * Mathf.Sqrt(1 - Mathf.Pow(eccentricity, 2.0f));

        Rp = a * (1 - Mathf.Abs(eccentricity));
        Ra = a * (1 + Mathf.Abs(eccentricity));

        T = 2.0f * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3.0f) / gravitationalParameter);



        if (Vector2.Dot(r, v) > 0.0f)
        {
            V = (360.0f * Mathf.Deg2Rad) - Mathf.Acos(Vector2.Dot(e, r) / (e.magnitude * r.magnitude));
        }
        else
        {
            V = Mathf.Acos(Vector2.Dot(e, r) / (e.magnitude * r.magnitude));
        }


        if (Vector2.Dot(r, v) > 0.0f)
        {
            Ec = (360.0f * Mathf.Deg2Rad) - Mathf.Acos((eccentricity + Mathf.Cos(V)) / (1 + eccentricity * Mathf.Cos(V)));
        }
        else
        {
            Ec = Mathf.Acos((eccentricity + Mathf.Cos(V)) / (1 + eccentricity * Mathf.Cos(V)));
        }
        

        M = Ec - eccentricity * Mathf.Sin(Ec);


        Ecd = Ec * Mathf.Rad2Deg;
        Vd = V * Mathf.Rad2Deg;
        Md = M * Mathf.Rad2Deg;


        n = Mathf.Sqrt(gravitationalParameter / Mathf.Pow(a, 3.0f));
        //this.n = 2 * Mathf.PI / this.T;

        Tp = M / n;
        Ta = T - Tp;

        dotProductV = Vector2.Dot(v.normalized, Vector2.up.normalized);
        dotProductVr = Vector2.Dot(v.normalized, Vector2.right.normalized);
        dotProduct = Vector2.Dot(Vector2.right.normalized, r.normalized);

        dotProductR = Vector2.Dot(Vector2.up.normalized, r.normalized);

        if (dotProduct < 0.0f)
        {
            satLong = 360.0f - Vector2.Angle(Vector2.up, r);
            longProfile = LongitudeProfile.West;
        }
        else
        {
            satLong = Vector2.Angle(Vector2.up, r);
            longProfile = LongitudeProfile.East;
        }

        
        /*if (profile == OrbitProfile.Prograde)
        {
            symSatLong = Vector2.Angle(Vector2.up, r) * Mathf.Sign(dotProduct) * -1.0f;
            //pLong = dotProduct > 0.0f ? Vd - symSatLong : Vd - symSatLong;
        }
        else
        {
            symSatLong = Vector2.Angle(Vector2.up, r) * Mathf.Sign(dotProduct);
            //pLong = symSatLong - Vd;
        }*/


        if (longProfile == LongitudeProfile.West)
        {
            symSatLong = - Vector2.Angle(Vector2.up, r);
        }
        else
        {
            symSatLong = Vector2.Angle(Vector2.up, r);
        }

        profile = lastSatLong > satLong ? OrbitProfile.Retrograde : OrbitProfile.Prograde;

        if (profile == OrbitProfile.Prograde)
        {
            pLong = -Vd - symSatLong;
        }
        else
        {
            pLong = symSatLong - Vd;
        }


        if (Mathf.Abs(pLong) > 180.0f)
            symPlong = 360.0f - Mathf.Abs(pLong);
        else
            symPlong = pLong;

        /*if (Vector2.Dot(r, v) > 0.0f)
            symPlong = pLong + 360.0f;
        else
            symPlong = pLong;*/



        /*symPlong = Mathf.Abs(symPlong);
        if (lastSatLong > satLong)
        {
            if (dotProductV > 0.0f)
            {
                symPlong *= -1;
            }
        }
        else
        {
            if (dotProductV < 0.0f)
            {
                //symPlong *= -1;
            }
        }

        */
        if (profile == OrbitProfile.Prograde)
        {
            orbitIncl = symPlong;
            pLong *= -1.0f;
        }
           
        else
        {
            orbitIncl = -symPlong;
        }
            



        lastSatLong = satLong;

        

        float peX = Rp * Mathf.Sin(pLong * Mathf.Deg2Rad) + planetCenter.x;
        float peY = Rp * Mathf.Cos(pLong * Mathf.Deg2Rad) + planetCenter.y;

        float apX = Ra * Mathf.Sin((pLong + 180.0f) * Mathf.Deg2Rad) + planetCenter.x;
        float apY = Ra * Mathf.Cos((pLong + 180.0f) * Mathf.Deg2Rad) + planetCenter.y;

        Vector2 pe = new Vector2(peX, peY);
        Vector2 ap = new Vector2(apX, apY);

        Vector2 dirToAp = (ap - pe).normalized;
        

        periapsisSprite.transform.position = pe;
        apoapsisSprite.transform.position = ap;

        float peXoff = peX - planetCenter.x;
        float peYoff = peY - planetCenter.y;

        Vector2 orbitFoci = planetCenter + dirToAp * (a - Rp);
        positions = CreateEllipse(a, b, orbitFoci, orbitIncl + orbitInclOffset, 1000);

        LineRenderer lr = GetComponent<LineRenderer>();
        
        lr.SetVertexCount(1000 + 1);
        for (int i = 0; i <= 1000; i++)
        {
            lr.SetPosition(i, positions[i]);
        }

    }

    [Space(10)]
    public float orbitIncl = 0.0f;
    public float orbitInclOffset = 0.0f;
    public float xOff = 0.0f;
    public float yOff = 0.0f;
    public float dotProduct = 0.0f;
    public float dotProductV = 0.0f;
    public float dotProductVr = 0.0f;
    public float dotProductR = 0.0f;

    private Vector2 v;
    private Vector2 r;
    private Vector2 h;
    private Vector2 e;

    private float lastSatLong = 0.0f;

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this.planet.transform.position, r);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(this.transform.position, v);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, e);

        
    }

    Vector3[] CreateEllipse(float a, float b, Vector2 orbitFoci, float theta, int resolution)
    {

        positions = new Vector3[resolution + 1];
        Quaternion q = Quaternion.AngleAxis(theta + 90.0f, Vector3.forward);
        Vector3 center = orbitFoci;

        for (int i = 0; i <= resolution; i++)
        {
            float angle = (float)i / (float)resolution * 2.0f * Mathf.PI;
            positions[i] = new Vector3(a * Mathf.Cos(angle), b * Mathf.Sin(angle), 0.0f);
            positions[i] = q * positions[i] + center;
        }

        return positions;
    }

    private Vector3[] positions;
}
