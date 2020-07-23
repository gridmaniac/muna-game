using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScienceDisplay : MonoBehaviour
{
    [SerializeField]
    Text log;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {

        log.text = "";

        log.text += "\n man anomaly: " + player.maneuverAnomaly;
        log.text += "\n man time: " + player.timeToManeuver;
        log.text += "\n n: " + player.trajectory[0]?.n;
        log.text += "\n Ez: " + player.trajectory[0]?.Ez;
        log.text += "\n Velocity: " + player.rigidbody.velocity.magnitude;

        //log.text += "\n u: " + orbit.u;
        //log.text += "\n longProfile: " + player.trajectory[0].longProfile;
        //log.text += "\n profile: " + orbit.profile;
        //log.text += "\n eVec: " + orbit.eVec;
        //log.text += "\n dirToPe: " + orbit.dirToPe;
        //log.text += "\n e: " + orbit.e;
        //log.text += "\n E: " + orbit.E;
        //og.text += "\n a: " + orbit.a;
        //log.text += "\n b: " + orbit.b;
        //log.text += "\n Rp: " + orbit.Rp;
        //log.text += "\n Ra: " + orbit.Ra;
        //log.text += "\n T: " + orbit.T;
        //log.text += "\n mod: " + orbit.mod;
        //log.text += "\n redo: " + orbit.redo;
        //log.text += "\n Oz: " + orbit.Oz;
        //log.text += "\n E: " + orbit.Ez;
        //log.text += "\n M: " + orbit.Mz;
        //log.text += "\n n: " + orbit.n;
        //log.text += "\n rvdot: " + orbit.rvdot;
        //log.text += "\n rcdot: " + orbit.rcdot;
        //log.text += "\n rvcross: " + orbit.rvcross;
        //log.text += "\n symSatLong: " + orbit.satLong;
        //log.text += "\n pLong: " + orbit.pLong;
        //log.text += "\n symPlong: " + orbit.symPlong;
        //log.text += "\n orbitIncl: " + orbit.orbitIncl;
        //log.text += "\n peX: " + orbit.peX;
        //log.text += "\n peY: " + orbit.peY;
        //log.text += "\n apX: " + orbit.apX;
        //log.text += "\n apY: " + orbit.apY;

        //log.text += "\n KEPLER";

        //log.text += "\n Pe: " + orbit.Kepler.Pe;
        //log.text += "\n Ap: " + orbit.Kepler.Ap;
        //log.text += "\n PeLong: " + orbit.Kepler.PeLong;
        //log.text += "\n Foci: " + orbit.Kepler.Foci;
        //log.text += "\n a: " + orbit.Kepler.a;
        //log.text += "\n b: " + orbit.Kepler.b;

        //log.text += "\n STATE";

        //log.text += "\n r: " + orbit.State.r;
        //log.text += "\n v: " + orbit.State.v;
        //log.text += "\n v mag: " + orbit.State.v.magnitude;

        // float v = 10.0f * Mathf.Sqrt(co.gravity.ForceMagnitude * -1.0f * (2 / transform.localPosition.magnitude - 1 / orbit.Kepler.a));
        //log.text += "\n v mag c: " + v;
        //log.text += "\n v state real: " + rigidbody.velocity;

        // float vsx = -orbit.a * orbit.e * Mathf.Sin(orbit.Ec);
        //log.text += "\n v state calc ec: " + vsx;
        //log.text += "\n v state calc: " + (rigidbody.velocity.normalized * v);

        //Vector2 tk = orbit.AtTime(-orbit.Ec + test2);
        //transfer.localPosition = tk;

        //Vector2 tk1 = orbit.AtTime(-orbit.Ec - 0.01f + test2);

        //Vector2 d = tk1 - tk;

        // float v2 = 10.0f * Mathf.Sqrt(co.gravity.ForceMagnitude * -1.0f * (2 / tk.magnitude - 1 / orbit.Kepler.a));

        //log.text += "\n v state at circle: " + (d.normalized * v2);


        //Vector2 pos0 = orbit.PosAtAnomaly(test2);

        //Vector2 pos1 = orbit.PosAtAnomaly(test2 + 0.01f);
        //Vector2 dv = (pos1 - pos0).normalized;
        //float v = 10.0f * Mathf.Sqrt(co.gravity.ForceMagnitude * -1.0f * (2.0f / pos0.magnitude - 1.0f / orbit.Kepler.a));
        //log.text += "\n v state at circle: " + (dv * v);

        //log.text += "\n time to circle: " + dt;

        //maneurSprite.localPosition = pos0;

        //log.text += "\n time to circle green: " + orbit.TimeToApproach(test2);



        //log.text += "\n time to circle: " + tryManeuver.time;
        //log.text += "\n encounter: " + hasPossibleEncounter;
        //log.text += "\n phase: " + phase;

        //log.text += "\n time:" + orbit.Time();
        //log.text += "\n velocity: " + rigidbody.velocity + ", " + rigidbody.velocity.magnitude;
        //log.text += "\n est. velocity: " + orbit.GetVelocity() + ", " + orbit.GetVelocity().magnitude;
        //log.text += "\n time from O:" + orbit.TimeFromTrueAnomaly();
    }
}
