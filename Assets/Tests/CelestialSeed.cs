using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Celestial
{
    public class CelestialSeed : ICloneable, IEquatable<CelestialSeed>
    {
        public float radius;
        public int resolution;
        public float topoFreq;
        public float topoAmp;
        public float details;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool Equals(CelestialSeed seed)
        {
            if (seed == null) return false;
            if (this.radius == seed.radius &&
                this.resolution == seed.resolution &&
                this.topoFreq == seed.topoFreq &&
                this.topoAmp == seed.topoAmp &&
                this.details == seed.details)
                return true;
            else
                return false;
        }

        public CelestialSeed(int resolution, float radius, float topoFreq, float topoAmp, float details)
        {
            this.radius = radius;
            this.resolution = resolution;
            this.topoFreq = topoFreq;
            this.topoAmp = topoAmp;
            this.details = details;
        }
    }
}
