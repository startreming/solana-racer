using System;
using System.Collections.Generic;
using System.Linq;
using Effects;
using UnityEngine;

namespace Kart
{
    public class KartParticles : MonoBehaviour
    {
        [Header("Particle Prefabs")]
        [SerializeField] private ParticleSystem smokeParticle;
        [SerializeField] private ParticleSystem[] wheelDirtParticles;

        private KartController _kartController;
        private KartModel _kartModel;
        
        private float _wheelDirtParticleMultiplier = 10f;
        private float _smockeParticleMultiplier = 1f;
        
        private void Awake()
        { 
            _kartController = GetComponent<KartController>();
            _kartModel = _kartController.Model;
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
            float rateOverTimeValue;
            
            if (!_kartController.Grounded) rateOverTimeValue = 0f;
            else rateOverTimeValue = _kartController.CurrentSpeed * _smockeParticleMultiplier;
            
            var emission = smokeParticle.emission;
            emission.rateOverTime = rateOverTimeValue;
        }
    }
}

