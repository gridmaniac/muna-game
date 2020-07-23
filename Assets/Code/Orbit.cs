using UnityEngine;
using System.Collections;
using MUNA.Enums;
using System;

public class Orbit
{

    public struct StateVectors
    {
        public Vector2 r;
        public Vector2 v;
    }

    public struct KeplerElements
    {
        public Vector3 Pe;
        public Vector3 Ap;
        public float PeLong;
        public Vector2 Foci;
        public float a;
        public float b;
    }

    public StateVectors State;
    public KeplerElements Kepler;
    public Vector3 GravityCenter;
    public float u;

    public LongitudeProfile longProfile;
    public OrbitProfile profile;
    public OrbitForm form;
    public Vector2 eVec;
    public Vector3 dirToPe;
    public float e, P, a, b, Rp, Ra, T, l;
    public float dpi, dps;
    public float Oz, Ez, Mz, n;
    public float rvdot, rdot, rvcross;
    public float satLong;
    public float pLong, symPlong;
    public float peX, peY, apX, apY;

    public float escapeTime = 0;
    public bool isEscaped;

    public bool isDubugging;
    Vector2[] cachedPath;

    public bool Calculate()
    {
        rdot = Vector2.Dot(Vector2.right, State.r);
        rvdot = Vector2.Dot(State.r, State.v);
        rvcross = Vector3.Cross(State.r, State.v).z;

        dpi = rvdot < 0.0f ? 2.0f * Mathf.PI : 0.0f;

        longProfile = rdot > 0.0f ? LongitudeProfile.East : LongitudeProfile.West;
        profile = rvcross > 0.0f ? OrbitProfile.Retrograde : OrbitProfile.Prograde;

        eVec = ((State.v.sqrMagnitude - u / State.r.magnitude) * State.r - rvdot * State.v) / u;
        e = eVec.magnitude;

        if (e >= 0.0f && e < 1.0f)
            form = OrbitForm.Ellipse;
        else if (e > 1.0f)
            form = OrbitForm.Hyperbola;
        else
            return false;

        P = State.v.sqrMagnitude / 2.0f - u / State.r.magnitude;
        a = -u / (2.0f * P);

        n = Mathf.Sqrt(u / Mathf.Pow(Mathf.Abs(a), 3.0f));
        l = a * (1 - Mathf.Pow(e, 2.0f));

        Oz = Mathf.Acos(Vector2.Dot(eVec, State.r) / (e * State.r.magnitude));
        
        switch (form)
        {
            case OrbitForm.Ellipse:
                Ez = Mathf.Abs(dpi - Mathf.Acos((e + Mathf.Cos(Oz)) / (1.0f + e * Mathf.Cos(Oz))));
                // Mz = Ez - e * Mathf.Sin(Ez);

                b = a * Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f));
                T = 2.0f * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3.0f) / u);
      
                break;

            case OrbitForm.Hyperbola:
                //Ez = Mathf.Acos((l / (e * State.r.magnitude)) - (1.0f / e));
                Ez = Acosh((e + Mathf.Cos(Oz)) / (1.0f + e * Mathf.Cos(Oz))) * Mathf.Sign(rvdot);

                b = a * Mathf.Sqrt(Mathf.Pow(e, 2.0f) - 1.0f);
                T = 0.0f;

                break;
        }

        Rp = a * (1.0f - e);
        Ra = a * (1.0f + e);

        if (longProfile == LongitudeProfile.West)
            satLong = -Vector2.Angle(Vector2.up, State.r) * Mathf.Deg2Rad;
        else
            satLong = Vector2.Angle(Vector2.up, State.r) * Mathf.Deg2Rad;

        if (profile == OrbitProfile.Prograde)
            pLong = satLong - Mathf.Abs(dpi - Oz);
        else
            pLong = satLong + Mathf.Abs(dpi - Oz);

        peX = Rp * Mathf.Sin(pLong) + GravityCenter.x;
        peY = Rp * Mathf.Cos(pLong) + GravityCenter.y;

        apX = Ra * Mathf.Sin(pLong + Mathf.PI) + GravityCenter.x;
        apY = Ra * Mathf.Cos(pLong + Mathf.PI) + GravityCenter.y;

        Kepler.a = a;
        Kepler.b = b;
        Kepler.Pe = new Vector2(peX, peY);
        Kepler.Ap = new Vector2(apX, apY);

        dirToPe = (GravityCenter - Kepler.Pe).normalized;
        Kepler.Foci = GravityCenter + dirToPe * (a - Rp);

        if (form == OrbitForm.Hyperbola)
            pLong -= Mathf.PI * 0.5f;

        return true;
    }

    public Vector2[] GetCachedPath()
    {
        return
            cachedPath != null ? cachedPath : new Vector2[0];
    }

    public void SetEscape(float time)
    {
        escapeTime = time;
        isEscaped = time < 2.0f * Mathf.PI;
    }

    public Vector2[] GetPath(int n)
    {
        var path = new Vector2[n];
        var st = (2.0f * Mathf.PI) / n;
        float xs = profile == OrbitProfile.Prograde ? -1.0f : 1.0f;
        float t = 0;

        int escapeN = 0;

        switch (form)
        {
            case OrbitForm.Ellipse:
                for (var i = 0; i < n; i++)
                {
                    t = st * i * xs + Mathf.PI * 0.5f;

                    path[i].x = Kepler.Foci.x + b * Mathf.Cos(t) * Mathf.Cos(-pLong) - a * Mathf.Sin(t) * Mathf.Sin(-pLong);
                    path[i].y = Kepler.Foci.y + b * Mathf.Cos(t) * Mathf.Sin(-pLong) + a * Mathf.Sin(t) * Mathf.Cos(-pLong);

                    //if (isEscaped && t > escapeTime + Mathf.PI * 0.5f)
                    //{
                    //    escapeN = i;
                    //    break;
                    //}
                        
                }
                break;

            case OrbitForm.Hyperbola:
                st *= 2.0f;

                for (var i = 0; i < n; i++)
                {
                    t = st * -1.0f * i + 2.0f * Mathf.PI;

                    path[i].x = Kepler.Foci.x + a * Cosh(t) * Mathf.Cos(-pLong) - b * Sinh(t) * Mathf.Sin(-pLong);
                    path[i].y = Kepler.Foci.y + a * Cosh(t) * Mathf.Sin(-pLong) + b * Sinh(t) * Mathf.Cos(-pLong);

                    //if (isEscaped && t > escapeTime * 0.5f  + 2.0f * Mathf.PI)
                    //{
                    //    escapeN = i;
                    //    break;
                    //}
                }
                break;
        }

        if (isEscaped && form != OrbitForm.Hyperbola)
        {
            //var newPath = new Vector2[escapeN];

            //for (int i = 0; i < escapeN; i++)
            //    newPath[i] = path[i];

            //path = newPath;

            //Array.Resize(ref path, escapeN);
        }

        cachedPath = path;
        return path;
    }

    public Vector2 PosAtAnomaly(float t)
    {
        var pos = new Vector3();
        float xs = profile == OrbitProfile.Prograde ? -1.0f : 1.0f;
        float xt = t * xs;

        switch (form)
        {
            case OrbitForm.Ellipse:
                xt += Mathf.PI * 0.5f;

                pos.x = Kepler.Foci.x + b * Mathf.Cos(xt) * Mathf.Cos(-pLong) - a * Mathf.Sin(xt) * Mathf.Sin(-pLong);
                pos.y = Kepler.Foci.y + b * Mathf.Cos(xt) * Mathf.Sin(-pLong) + a * Mathf.Sin(xt) * Mathf.Cos(-pLong);

                break;

            case OrbitForm.Hyperbola:
                xt *= -1.0f;

                pos.x = Kepler.Foci.x + a * Cosh(xt) * Mathf.Cos(-pLong) - b * Sinh(xt) * Mathf.Sin(-pLong);
                pos.y = Kepler.Foci.y + a * Cosh(xt) * Mathf.Sin(-pLong) + b * Sinh(xt) * Mathf.Cos(-pLong);
                break;
        }

        return pos;
    }

    public float AnomalyAtPath(int n, int i)
    {
        var st = (2.0f * Mathf.PI) / n;
        float xs = profile == OrbitProfile.Prograde ? -1.0f : 1.0f;

        //switch (form)
        //{
        //    case OrbitForm.Ellipse:
        //        return st * i + Mathf.PI * 0.5f;

        //    case OrbitForm.Hyperbola:
        //        return st * -1.0f * i + 2.0f * Mathf.PI;
        //}
        float pi = profile == OrbitProfile.Retrograde ? Mathf.PI : 0.0f;

        switch (form)
        {
            case OrbitForm.Ellipse:
                return st * i * xs;

            case OrbitForm.Hyperbola:
                st *= 2.0f;
                //return st * i * xs + Mathf.PI;
                return st * xs * i - xs * 2.0f * Mathf.PI;
        }

        return st * i;
    }

    public float Time()
    {
        float E = Ez;

        float dt0 = 0.0f;

        switch (form)
        {
            case OrbitForm.Ellipse:
                dt0 = Mathf.Sqrt(Mathf.Pow(a, 3.0f) / u) * (E - e * Mathf.Sin(E));

                break;

            case OrbitForm.Hyperbola:
                dt0 = Mathf.Sqrt(Mathf.Pow(-a, 3.0f) / u) * (e * Sinh(E) - E);

                break;
        }

        return dt0;

    }

    public float TimeToApproach(float E1)
    {
        float E = Ez;

        float dt0 = 0.0f, dt1 = 0.0f;

        switch (form)
        {
            case OrbitForm.Ellipse:
                dt0 = Mathf.Sqrt(Mathf.Pow(a, 3.0f) / u) * (E - e * Mathf.Sin(E));
                dt1 = Mathf.Sqrt(Mathf.Pow(a, 3.0f) / u) * (E1 - e * Mathf.Sin(E1));

                break;

            case OrbitForm.Hyperbola:
                dt0 = Mathf.Sqrt(Mathf.Pow(-a, 3.0f) / u) * (e * Sinh(E) - E);
                dt1 = Mathf.Sqrt(Mathf.Pow(-a, 3.0f) / u) * (e * Sinh(E1) - E1);

                break;
        }

        float dt = dt1 - dt0;

        if (dt < 0.0f)
            dt = T + dt;

        return dt;
    }

    public Vector2 PosAtTime(float t)
    {
        float M = n * t;
        float E = M;

        if (float.IsNaN(M) || M == 0.0f)
            //return Vector2.zero;
            return PosAtAnomaly(0);

        int exit = 0;

        switch (form)
        {
            case OrbitForm.Ellipse:
                while (true)
                {
                    var dE = (E - e * Mathf.Sin(E) - M) / (1.0f - e * Mathf.Cos(E));
                    E -= dE;
                    if (Mathf.Abs(dE) < 1e-6) break;

                    exit++;

                    //if (exit > 20)
                    //    return Vector2.zero;

                    if (exit > 50)
                        break;
                }

                break;

            case OrbitForm.Hyperbola:

                while (true)
                {
                    var dE = (e * Sinh(E) - E - M) / (e * Cosh(E) - 1.0f);
                    E -= dE;
                    if (Mathf.Abs(dE) < 1e-6) break;

                    exit++;
                    if (exit > 50)
                        break;
                }

                break;
        }

        //float O = 2.0f * Mathf.Atan(Mathf.Sqrt((1.0f + e) / (1.0f - e)) * Mathf.Tan(E / 2.0f));
        //Oz = O;
        Ez = E;

        return PosAtAnomaly(E);
    }

    public Vector2 GetVelocity()
    {
        return GetVelocityAt(Ez);
    }

    public Vector2 GetVelocityAt(float E)
    {
        float vX = 0.0f, vY = 0.0f;

        float xs = profile == OrbitProfile.Prograde ? -1.0f : 1.0f;

        switch (form)
        {
            case OrbitForm.Ellipse:
                vX = -Mathf.Sqrt(u / a) * (Mathf.Sin(E) / (1.0f - e * Mathf.Cos(E))) * xs;
                vY = Mathf.Sqrt(u / a) * ((Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f)) * Mathf.Cos(E)) / (1.0f - e * Mathf.Cos(E)));

                break;
            case OrbitForm.Hyperbola:
                vX = -Mathf.Sqrt(u / -a) * (Sinh(E) / (1.0f - e * Cosh(E))) * xs;
                vY = Mathf.Sqrt(u / -a) * ((Mathf.Sqrt(Mathf.Pow(e, 2.0f) - 1.0f) * Cosh(E)) / (1.0f - e * Cosh(E)));

                break;
        }

        var raw = new Vector2(vX, vY);

        if (form == OrbitForm.Ellipse)
            return Rotate(raw, -pLong + Mathf.PI * 0.5f * xs);

        float pi = profile == OrbitProfile.Retrograde ? Mathf.PI : 0.0f;
        return Rotate(raw, -pLong - pi);
    }

    public Vector2 Rotate(Vector2 v, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    float Sinh(float x)
    {
        return (Mathf.Exp(x) - Mathf.Exp(-x)) / 2.0f;
    }

    float Cosh(float x)
    {
        return (Mathf.Exp(x) + Mathf.Exp(-x)) / 2.0f;
    }

    float Acosh(float x)
    {
        return Mathf.Log(x + Mathf.Sqrt(Mathf.Pow(x, 2.0f) - 1.0f));
    }
}
