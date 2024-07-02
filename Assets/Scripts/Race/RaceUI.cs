using System;
using System.Collections.Generic;
using Car;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Race
{
    public class RaceUI : MonoBehaviour
    {
        [SerializeField] private LapManager lapManager;
        [SerializeField] private RaceManager raceManager;
        [SerializeField] private CarController carController;
        
        [SerializeField] private TMP_Text currentLap;
        [SerializeField] private TMP_Text currentPlace;
        [SerializeField] private TMP_Text totalLaps;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject timerContainer;
        [SerializeField] private TextMeshProUGUI currentSpeed;
        [SerializeField] private TextMeshProUGUI currentLapTime;
        [SerializeField] private Image speedometerFillImage;
        [SerializeField] private RacerUI racer;
        [SerializeField] private Transform racersContainer;
        [SerializeField] private RacerLeaderBoardUI racerLeaderBoardUIPrefab;
        [SerializeField] private GameObject leaderBoard;
        [SerializeField] private Transform racersLeaderBoardContainer;

        private float _maxKPHSpeed = 120f; 
        private float _minSpeedometerFill = 0.1f;
        private float _maxSpeedometerFill = 0.7f;
        private float _startedLapTime;
        private bool _raceStarted;

        private void Start()
        {
            lapManager.OnUpdatedPlace += UpdatePlace;
            lapManager.OnUpdatedLap += UpdateLap;
            lapManager.OnUpdatedRacers += UpdateRacers;
            raceManager.OnUpdatedTimer += UpdateTimer;
            raceManager.OnStartedRace += StartRace;
            lapManager.OnFinishedRace += FinishRace;
            lapManager.OnRacerFinishedRace += RacerFinishRace;
            
            totalLaps.text = lapManager.Laps.ToString();
            timerText.text = raceManager.WaitToStart.ToString();
            currentSpeed.text = "0 km/h";
            leaderBoard.SetActive(false);
        }

        private void OnDestroy()
        {
            lapManager.OnUpdatedPlace -= UpdatePlace;
            lapManager.OnUpdatedLap -= UpdateLap;
            lapManager.OnUpdatedRacers -= UpdateRacers;
            raceManager.OnUpdatedTimer -= UpdateTimer;
            raceManager.OnStartedRace -= StartRace;
            lapManager.OnFinishedRace -= FinishRace;
        }

        private void Update()
        {
            if (!_raceStarted) return;
            
            var speed = carController.Rigidbody.velocity.magnitude;
            var speedKPH = speed * 4f;
            
            var normalizedSpeed = Mathf.Clamp01(speedKPH / _maxKPHSpeed);
            var fillAmount = Mathf.Lerp(_minSpeedometerFill, _maxSpeedometerFill, normalizedSpeed);
            
            currentSpeed.text = speedKPH.ToString("F0") + " km/h";
            speedometerFillImage.fillAmount = fillAmount;
            
            var elapsedTime = Time.time - _startedLapTime;
            var timeSpan = TimeSpan.FromSeconds(elapsedTime);
            var formattedTime = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}:{timeSpan.Milliseconds / 10:D2}";
            currentLapTime.text = formattedTime;
        }

        private void UpdateLap(int newLap)
        {
            currentLap.text = newLap.ToString();
            _startedLapTime = Time.time;
        }

        private void UpdatePlace(int newPlace)
        {
            currentPlace.text = newPlace.ToString();
        }

        private void UpdateRacers(List<Racer> racers)
        {
            foreach (Transform child in racersContainer)
            {
                Destroy(child.gameObject);
            }

            for (var place = 0; place < racers.Count; place++)
            {
                var racerData = racers[place];
                var racerUI = Instantiate(racer, racersContainer);
                racerUI.UpdatePlayer(racerData, place+1);
                racerUI.transform.SetSiblingIndex(place);
            }
        }

        private void StartRace()
        {
            _raceStarted = true;
            _startedLapTime = Time.time;
        }

        private void UpdateTimer(int currentCountDown)
        {
            if (currentCountDown <= 0)
            {
                timerText.text = "GO!";
                Invoke(nameof(DisableTimer), 1);
            }
            else
            {
                timerText.text = currentCountDown.ToString();   
            }
        }
        
        private void DisableTimer()
        {
            timerContainer.SetActive(false);
        }
        
        private void FinishRace(List<Racer> racers)
        {
            
        }
        
        private void RacerFinishRace(Racer finishedRacer)
        {
            if (finishedRacer.Represents == CarController.PlayerGameObject)
            {
                _raceStarted = false;
                leaderBoard.SetActive(true);
            }
            
            var racerUI = Instantiate(racerLeaderBoardUIPrefab, racersLeaderBoardContainer);
            racerUI.UpdatePlayer(finishedRacer);
        }
    }
}