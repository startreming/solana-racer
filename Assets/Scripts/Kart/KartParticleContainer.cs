using System.Collections.Generic;
using Effects;
using UnityEngine;

namespace Kart
{
    public class KartParticleContainer
    {
        public List<ParticleSystem> List { get; } = new List<ParticleSystem>();
        public string Identifier { get; }

        public KartParticleContainer(GroundSurfacePreset gsp)
        {
            Identifier = gsp.name;
        }
    }
}