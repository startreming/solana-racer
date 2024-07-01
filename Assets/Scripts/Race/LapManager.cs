﻿using System;
using System.Collections.Generic;
using System.Linq;
using Car;
using UnityEngine;

namespace Race
{
    public class LapManager : MonoBehaviour
    {
        public event Action<int> OnUpdatedLap = newLap => { };
        public event Action<int> OnUpdatedPlace = newPlace => { };
        public event Action<List<Racer>> OnUpdatedRacers = newRacers => { };
        
        [SerializeField] private int maxLaps = 3;
        [SerializeField] private GameObject[] availablePlayers;
        [SerializeField] private List<LapCheckpoint> checkpoints = new List<LapCheckpoint>();
        
        private readonly List<Racer> _racers = new List<Racer>();
        private float _currentRaceTime = 0f;
        private float _leadingCheckTimer = 1f;

        private void Update()
        {
            if (_leadingCheckTimer > 0f)
            {
                _leadingCheckTimer -= Time.deltaTime;
            }
            else
            {
                SetRacersProgress();
                CheckRacerPositions();
                _leadingCheckTimer = 1f;
            }
        }
        
        private void SetRacersProgress()
        {
            foreach (var racer in _racers)
            {
                racer.RaceProgress = GetRaceProgressPercent(racer);
            }
        }
        
        private void CheckRacerPositions()
        {
            var ownRacerEntity = CarController.PlayerGameObject;
            if (ownRacerEntity == null)
                return;
            
            List<Racer> orderedRacers = _racers.OrderByDescending(r => r.RaceProgress).ToList();

            var ownRacer = _racers.FirstOrDefault(x=> x.Represents == ownRacerEntity);
            
            OnUpdatedPlace?.Invoke(orderedRacers.IndexOf(ownRacer)+1);
            OnUpdatedRacers?.Invoke(orderedRacers);
        }

        public void VerifyPass(GameObject playerObject, LapCheckpoint lapCheckpoint)
        {
            for (var index = 0; index < availablePlayers.Length; index++)
            {
                var spawnedObject = availablePlayers[index];
                if (spawnedObject == playerObject)
                {
                    var racer = GetRacer(playerObject);
                    if (racer.Finished)
                        Debug.Log("This racer already finished the race.");
                    else
                    {
                        CheckRacerProgress(lapCheckpoint, racer);
                    }
                }
            }
        }
        
        private void CheckRacerProgress(LapCheckpoint lapCheckpoint, Racer racer)
        {
            if (racer.Progress == checkpoints.Count - 1 && lapCheckpoint.Progress == 0)
            {
                racer.Progress = 0;
                racer.Lap += 1;
                //Debug.Log($"Racer {racer.Represents} just advanced to lap {racer.Lap}!");
            }

            if (racer.Progress == lapCheckpoint.Progress - 1)
            {
                racer.Progress = lapCheckpoint.Progress;
                //Debug.Log($"Advanced racer {racer.Represents} to {racer.Progress}");
            }
            
            // Update all racers
            
            if(racer.Represents == CarController.PlayerGameObject)
                OnUpdatedLap.Invoke(racer.Lap+1);

            if (racer.Lap == maxLaps)
            {
                var playerName = "Player name";

                string timeElapsed = "Too long";
                if (_currentRaceTime > TimeSpan.MaxValue.TotalSeconds)
                {
                    _currentRaceTime = 0;
                }
                else
                {
                    TimeSpan time = TimeSpan.FromSeconds(_currentRaceTime);
                    timeElapsed = time.ToString(@"mm\:ss\:fff");
                }
                
                //UpdateScoreboard(playerName, timeElapsed);
                
                racer.Finished = true;
            }
        }
        
        private float GetRaceProgressPercent(Racer racer)
        {
            float progress = 0f;

            float checkpointContribution = 1f / checkpoints.Count;
            int nextCheckpointIndex = racer.Progress + 1;
            if (nextCheckpointIndex == checkpoints.Count) nextCheckpointIndex = 0;
            
            Vector3 prevCheckpoint = checkpoints[racer.Progress].transform.position;
            Vector3 nextCheckpoint = checkpoints[nextCheckpointIndex].transform.position;
        
            progress += checkpointContribution * racer.Progress;
        
            float distanceCovered = Vector3.Distance(racer.Represents.transform.GetChild(0).position, nextCheckpoint);
            float distanceToCover = Vector3.Distance(prevCheckpoint, nextCheckpoint);
        
            float checkpointProgress = 1f-(distanceCovered/distanceToCover);

            progress += checkpointContribution * checkpointProgress;
            progress = Mathf.Clamp(progress, 0f, 1f);

            progress = (progress / maxLaps);
            progress += (1f/maxLaps) * (racer.Lap);
            
            return progress;
        }

        private Racer GetRacer(GameObject playerObject)
        {
            foreach (var racer in _racers.Where(racer => racer.Represents == playerObject))
            {
                return racer;
            }
            var newRacer = new Racer
            {
                Represents = playerObject
            };
            _racers.Add(newRacer);
            return newRacer;
        }
    }
    
    public class Racer
    {
        public GameObject Represents;
        public int Progress;
        public int Lap;
        public int Place;
        public float RaceProgress;
        public bool Finished;
    }
}