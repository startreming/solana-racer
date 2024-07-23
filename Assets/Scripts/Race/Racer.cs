using System;
using UnityEngine;

namespace Race
{
    public class Racer
    {
        public GameObject Represents;
        public int Progress;
        public int Lap;
        public int Place;
        public int PlaceInverted;
        public float RaceProgress;
        public bool Finished;
        public TimeSpan TotalRaceTime;
    }
}