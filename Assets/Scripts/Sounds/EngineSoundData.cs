using System;
using UnityEngine;

namespace Sounds
{
    [Serializable]
    public class EngineSoundData
    {
        public float MinRPM;
        public float MaxRPM;
        public AudioClip EngineClip;
        public AudioClip DirtClip;
        public float MinPitch = 0.9f;
        public float MaxPitch = 1.3f;
    }
}