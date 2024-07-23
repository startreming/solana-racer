using System;
using UnityEngine;

namespace Domain
{
    [Serializable]
    public class VehicleStats
    {
        [SerializeField] [Range(0, 24)] private int speedStat;
        [SerializeField] [Range(0, 24)] private int accelerationStat;
        [SerializeField] [Range(0, 24)] private int weightStat;
        [SerializeField] [Range(0, 24)] private int handlingStat;
        [SerializeField] [Range(0, 24)] private int tractionStat;
        [SerializeField] [Range(0, 24)] private int miniNitroStat;

        public VehicleStats(VehicleStats initializer)
        {
            speedStat = initializer.speedStat;
            accelerationStat = initializer.accelerationStat;
            handlingStat = initializer.handlingStat;
            tractionStat = initializer.tractionStat;
            weightStat = initializer.weightStat;
            miniNitroStat = initializer.miniNitroStat;
        }
        
        public float GetAccelerationConstant()
        {
            return LinearConversion(accelerationStat, 0.2f, 1.2f);
        }

        public float GetMaxSpeed()
        {
            return LinearConversion(speedStat, 50f, 90f);
        }
        
        public float GetCurrentSpeed(float timePressed)
        {
            var k = GetAccelerationConstant();
            float e = (float) System.Math.E;
            float t = timePressed;
            float maxSpeed = GetMaxSpeed();

            float currentSpeed = maxSpeed * ( 1 - Mathf.Pow(e, (-k * t)) ) ;
            
            return currentSpeed;
        }
        
        public float GetReverseSpeed(float timePressed)
        {
            var k = GetAccelerationConstant() * 2/3;
            float e = (float) System.Math.E;
            float t = timePressed;
            float maxSpeed = GetMaxSpeed() / 2f;

            float currentSpeed = maxSpeed * ( 1 - Mathf.Pow(e, (-k * t)) ) ;
            
            return -currentSpeed;
        }

        public float GetAnglesPerSecond()
        {
            return LinearConversion(handlingStat, 10f, 20f);
        }

        public float GetOffroadTraction()
        {
            return LinearConversion(tractionStat, 0.5f, 0.9f);
        }
        
        public float GetNitroCollectionRate()
        {
            return LinearConversion(miniNitroStat, 150f, 250f);
        }
        
        public float GetWeight()
        {
            return LinearConversion(weightStat, 10f, 100f);
        }
        
        public float GetCollisionForceMagnitude(float speed, float otherCarSpeed, float carWeight, float otherCarWeight)
        {
            var vehicle1weight = carWeight;
            var vehicle2weight = otherCarWeight;

            var vehicle1speed = speed;
            var vehicle2speed = otherCarSpeed;

            var k = 10f;

            return k * ((vehicle1weight * vehicle2weight) / (vehicle1weight + vehicle2weight)) *
                   (Mathf.Abs(vehicle1speed - vehicle2speed)); //* (1 / vehicle1weight);
        }
        
        private static float LinearConversion(float stat, float minValue, float maxValue, float statMinValue = 0f, float statMaxValue = 24f)
        {
            var k = (((maxValue - minValue) / (statMaxValue - statMinValue)) * (stat - statMinValue)) + minValue;
            return k;
        }
        
        public static VehicleStats operator +(VehicleStats a, VehicleStats b)
        {
            var stats = new VehicleStats(a);

            stats.accelerationStat += b.accelerationStat;
            stats.speedStat += b.speedStat;
            stats.handlingStat += b.handlingStat;
            stats.tractionStat += b.tractionStat;
            stats.weightStat += b.weightStat;
            stats.miniNitroStat += b.miniNitroStat;
            
            return stats;
        }

    }
}
