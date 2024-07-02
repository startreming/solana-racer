using System;
using UnityEngine;

namespace Car
{
    public class CarParticles : MonoBehaviour
    {
        [Header("Particle Prefabs")]
        [SerializeField] private ParticleSystem[] smokeParticles;
        [SerializeField] private ParticleSystem[] wheelDirtParticles;
        [SerializeField] private ParticleSystem boostParticle;

        private CarController _kartController;
        private float _wheelDirtParticleMultiplier = 10f;
        private float _smockeParticleMultiplier = 1f;
        
        private void Awake()
        { 
            _kartController = GetComponent<CarController>();
            _kartController.OnBoostStart += BoostStart;
        }

        private void OnDestroy()
        {
            _kartController.OnBoostStart -= BoostStart;
        }

        private void BoostStart()
        {
            boostParticle.Play();
        }

        private void Update()
        {
            AdjustWheelDirtParticles();
            AdjustSmokeParticle();
        }

        private void AdjustWheelDirtParticles()
        {
            foreach (var wheelDirtParticle in wheelDirtParticles)
            {
                float rateOverTimeValue;
                
                if (!_kartController.Grounded) rateOverTimeValue = 0f;
                else rateOverTimeValue = _kartController.CurrentSpeed * _wheelDirtParticleMultiplier;

                var emission = wheelDirtParticle.emission;
                emission.rateOverTime = rateOverTimeValue;
            }
        }
        
        private void AdjustSmokeParticle()
        {
            foreach (var smokeParticle in smokeParticles)
            {
                float rateOverTimeValue;
            
                if (!_kartController.Grounded) rateOverTimeValue = 0f;
                else rateOverTimeValue = _kartController.CurrentSpeed * _smockeParticleMultiplier;
            
                var emission = smokeParticle.emission;
                emission.rateOverTime = rateOverTimeValue;
            }
        }
    }
}

