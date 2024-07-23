using System;
using Car;
using TMPro;
using UnityEngine;

namespace Race
{
    public class RaceManager : MonoBehaviour
    {
        public event Action OnStartedRace;
        public event Action<int> OnUpdatedTimer = newTimer => { };
        public int WaitToStart => waitToStart;
        
        [SerializeField] private int waitToStart = 3;
        [SerializeField] private CarController[] carControllers;
        
        private int _currentCountDown;

        private void Start()
        {
            StartTimer();
        }
        
        private void StartTimer()
        {
            _currentCountDown = waitToStart;
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            OnUpdatedTimer?.Invoke(_currentCountDown);
            
            if (_currentCountDown <= 0)
            {
                StartRace();
                return;
            }

            _currentCountDown--;
            Invoke(nameof(UpdateTimer), 1);
        }

        private void StartRace()
        {
            OnStartedRace?.Invoke();
            foreach (var car in carControllers)
            {
                car.SetCanMove(true);
            }
        }
    }
}
