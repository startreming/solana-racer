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
        private GameObject _particlePool;
        private float _wheelDirtParticleMultiplier = 10f;
        private float _smockeParticleMultiplier = 1f;
        
        private List<ParticleSystem> _boostParticles = new List<ParticleSystem>();
        private List<ParticleSystem> _driftParticles = new List<ParticleSystem>();
        private List<ParticleSystem> _driftSmokeParticles = new List<ParticleSystem>();
        private List<ParticleSystem> _hoverSmokeParticles = new List<ParticleSystem>();
        
        private KartParticleContainer _lastSelectedContainer;
        private List<KartParticleContainer> _trackParticleContainer = new List<KartParticleContainer>();
        
        
        private void Awake()
        { 
            _kartController = GetComponent<KartController>();
            _kartModel = _kartController.Model;
            
            /*_particlePool = new GameObject("Particle Pool");
            _particlePool.transform.SetParent(transform);*/
        }
        
        private void OnEnable()
        {
            _kartController.OnDriftStart += DriftStart;
            _kartController.OnDriftEnd += DriftEnd;
            _kartController.OnDriftUpgrade += DriftUpgrade;
            _kartController.OnBoostStart += BoostParticle;
            
            _kartController.OnGroundTypeChange += GroundChange;
            
            _kartController.OnGroundEnter += GroundEnter;
            _kartController.OnGroundLeave += GroundLeave;
        }

        private void OnDisable()
        {
            _kartController.OnDriftStart -= DriftStart;
            _kartController.OnDriftEnd -= DriftEnd;
            _kartController.OnDriftUpgrade -= DriftUpgrade;
            _kartController.OnBoostStart -= BoostParticle;
            _kartController.OnGroundTypeChange -= GroundChange;
            
            _kartController.OnGroundTypeChange -= GroundChange;
            
            _kartController.OnGroundEnter -= GroundEnter;
            _kartController.OnGroundLeave -= GroundLeave;
        }

        private void GroundEnter()
        {
            StartParticles(_hoverSmokeParticles);
        }
        private void GroundLeave()
        {
            Stop(_hoverSmokeParticles);
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

        private void Attach()
        {
            //AttachParticlesToList(kartModel.WheelParticlePoints, smokeParticlePrefab, _driftSmokeParticles);
        }
        
        public void ReturnToPool()
        {
            var allParticles = new List<ParticleSystem>();
            
            _boostParticles.ForEach(particle => allParticles.Add(particle));
            _driftParticles.ForEach(particle => allParticles.Add(particle));
            _driftSmokeParticles.ForEach(particle => allParticles.Add(particle));
            _hoverSmokeParticles.ForEach(particle => allParticles.Add(particle));

            _trackParticleContainer.ForEach(container => container.List.ForEach(p => allParticles.Add(p)) );

            // Agony
            foreach (var p in allParticles)
            {
                p.transform.SetParent(_particlePool.transform);
                p.gameObject.SetActive(false);
            }
        }

        private void GroundChange(GroundSurfacePreset gsp)
        {
            if (gsp != null)
            {
                if (_lastSelectedContainer != null)
                    Stop(_lastSelectedContainer.List);
                
                KartParticleContainer container = FindContainerWithGsp(gsp);
                
                if (container == null)
                    container = CreateParticleContainer(gsp);

                AttachParticlesToList(_kartModel.WheelParticlePoints, gsp.tractionParticle, container.List);
                
                StartParticles(container.List);
                _lastSelectedContainer = container;
            }
            else
            {
                if (_lastSelectedContainer != null)
                {
                    Stop(_lastSelectedContainer.List);
                }
            }
        }

        private KartParticleContainer CreateParticleContainer(GroundSurfacePreset gsp)
        {
            var newContainer = new KartParticleContainer(gsp);
            _trackParticleContainer.Add(newContainer);
            return newContainer;
        }

        private KartParticleContainer FindContainerWithGsp(GroundSurfacePreset gsp)
        {
            return _trackParticleContainer.FirstOrDefault(container => container.Identifier == gsp.name);
        }
        
        private void DriftStart()
        {
            StartParticles(_driftSmokeParticles);
        }
        
        private void DriftEnd()
        {
            Stop(_driftSmokeParticles);
        }

        private void DriftUpgrade(int driftLevel)
        {
            Stop(_driftParticles);
            
            StartParticles(_driftParticles);
        }

        private void BoostParticle()
        {
            StartParticles(_boostParticles);
            Stop(_driftParticles);
        }
        
        private void StartParticles(List<ParticleSystem> list)
        {
            foreach (var particle in list)
            {
                particle.Play();
            }
        }
        
        private void ColorParticles(List<ParticleSystem> list, Color c)
        {
            foreach (var particle in list)
            {
                var main = particle.main;
                main.startColor = c;
            }
        }
        
        private void Stop(List<ParticleSystem> list)
        {
            foreach (var particle in list)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        private void AttachParticlesToList(Transform[] particlePoints, GameObject particlePrefab, List<ParticleSystem> particleSystemList)
        {
            foreach (var particlePoint in particlePoints)
            {
                // Check if it needs the particle
                ParticleSystem containedPS = particlePoint.GetComponentsInChildren<ParticleSystem>().FirstOrDefault(p => p.name.Contains(particlePrefab.name));

                if (containedPS == null)
                {
                    // Assume it is pooled
                    ParticleSystem pooledParticleSystem = _particlePool.GetComponentsInChildren<ParticleSystem>(true)
                        .FirstOrDefault(p => p.name.Contains(particlePrefab.name));    
                    
                    if (pooledParticleSystem != null) // Reattach if so
                    {
                        pooledParticleSystem.gameObject.SetActive(true);
                        pooledParticleSystem.transform.position = particlePoint.transform.position;
                        pooledParticleSystem.transform.rotation = particlePoint.transform.rotation;
                        pooledParticleSystem.transform.SetParent(particlePoint);
                    }
                    else // Create the particle if not
                    {
                        var newParticle = Instantiate(particlePrefab, particlePoint.transform.position, particlePoint.rotation);
                        newParticle.transform.SetParent(particlePoint);
                        particleSystemList.Add(newParticle.GetComponent<ParticleSystem>());
                    }
                }
            }
        }
    }
}

