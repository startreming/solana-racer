using System;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    public class CarEngineSounds : MonoBehaviour
    {
        [SerializeField] private CarController controller;
        [SerializeField] private EngineSoundData[] sounds;
        private Dictionary<EngineSoundData, EngineSources> _engineSources = new Dictionary<EngineSoundData, EngineSources>();

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
                engineSource.pitch = Mathf.Lerp(data.MinPitch, data.MaxPitch, normalizedSpeed);

                // Calculate volume adjustment
                engineSource.volume = CalculateVolume(data, speedKPH);
                if (dirtSource != null)
                {
                    dirtSource.volume = engineSource.volume;
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
        
        float CalculateVolume(EngineSoundData data, float speedKPH)
        {
            var transitionThreshold = 5;
            if (speedKPH >= data.MinRPM && speedKPH <= data.MaxRPM)
            {
                return 1f;
            }
            
            if (speedKPH < data.MinRPM)
            {
                float minTransitionRange = Mathf.Max(0, data.MinRPM - transitionThreshold); 
                if (speedKPH >= minTransitionRange)
                {
                    return Mathf.Lerp(0, 1, (speedKPH - minTransitionRange) / (data.MinRPM - minTransitionRange));
                }

                return 0f;
            }
            
            if (speedKPH > data.MaxRPM)
            {
                float maxTransitionRange = data.MaxRPM + transitionThreshold; 
                if (speedKPH <= maxTransitionRange)
                {
                    return Mathf.Lerp(1, 0, (speedKPH - data.MaxRPM) / (maxTransitionRange - data.MaxRPM));
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