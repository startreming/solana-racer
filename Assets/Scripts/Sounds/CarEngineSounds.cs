using System;
using System.Collections.Generic;
using Car;
using UnityEngine;

namespace Sounds
{
    public class CarEngineSounds : MonoBehaviour
    {
        [SerializeField] private CarController controller;
        [SerializeField] private EngineSoundData[] sounds;
        private Dictionary<EngineSoundData, EngineSources> _engineSources = new Dictionary<EngineSoundData, EngineSources>();

        private float _timeInAir = 0;

        private void Start()
        {
            foreach (var data in sounds)
            {
                EngineSources engineSources = new EngineSources
                {
                    EngineSource = CreateAudioSource(data.EngineClip)
                };
                if (data.DirtClip != null)
                {
                    engineSources.DirtSource = CreateAudioSource(data.DirtClip);
                }
                _engineSources.Add(data, engineSources);
            }
        }

        private AudioSource CreateAudioSource(AudioClip clip)
        {
            var audioS = gameObject.AddComponent<AudioSource>();
            audioS.Stop();
            audioS.playOnAwake = false;
            audioS.clip = clip;
            audioS.loop = true;
            audioS.priority = 0;
            audioS.spatialBlend = 1;
            audioS.volume = 0;
            return audioS;
        }

        private void Update()
        {
            var speed = !controller.CanMove ? 0 : controller.Rigidbody.velocity.magnitude;
            var speedKPH = speed * 4f;
            foreach (var (data, audioS) in _engineSources)
            {
                var engineSource = audioS.EngineSource;
                var dirtSource = audioS.DirtSource;
                float normalizedSpeed = (speedKPH - data.MinRPM) / (data.MaxRPM - data.MinRPM);
                float finalPitch = Mathf.Lerp(data.MinPitch, data.MaxPitch, normalizedSpeed);
                
                if (!controller.Grounded)
                {
                    _timeInAir += Time.deltaTime;
                    if (_timeInAir > 1f)
                    {
                        finalPitch = 1.3f;
                    }
                }
                else
                {
                    _timeInAir = 0;
                }
                
                if (!controller.CanMove)
                    finalPitch = 1f;

                engineSource.pitch = finalPitch;

                // Calculate volume adjustment
                engineSource.volume = CalculateVolume(data, speedKPH);
                if (dirtSource != null)
                {
                    dirtSource.volume = Mathf.Clamp(engineSource.volume, 0, 0.5f);
                }

                if (dirtSource != null && !dirtSource.isPlaying)
                {
                    audioS.DirtSource.Play();
                }
                if (engineSource != null && !engineSource.isPlaying)
                {
                    audioS.EngineSource.Play();
                }
            }
        }
        
        float CalculateVolume(EngineSoundData data, float speedKPH, float maxVolume = 1f)
        {
            var transitionThreshold = 5;
            if (speedKPH >= data.MinRPM && speedKPH <= data.MaxRPM)
            {
                return maxVolume;
            }
            
            if (speedKPH < data.MinRPM)
            {
                float minTransitionRange = Mathf.Max(0, data.MinRPM - transitionThreshold); 
                if (speedKPH >= minTransitionRange)
                {
                    return Mathf.Lerp(0, maxVolume, (speedKPH - minTransitionRange) / (data.MinRPM - minTransitionRange));
                }

                return 0f;
            }
            
            if (speedKPH > data.MaxRPM)
            {
                float maxTransitionRange = data.MaxRPM + transitionThreshold; 
                if (speedKPH <= maxTransitionRange)
                {
                    return Mathf.Lerp(maxVolume, 0, (speedKPH - data.MaxRPM) / (maxTransitionRange - data.MaxRPM));
                }

                return 0f;
            }

            return 0f;
        }
    }

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

    public class EngineSources
    {
        public AudioSource EngineSource;
        public AudioSource DirtSource;
    }
}