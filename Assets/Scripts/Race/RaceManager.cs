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
        
        private int currentCountDown;

        private void Start()
        {
            StartTimer();
        }
        
        private void StartTimer()
        {
            currentCountDown = waitToStart;
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            OnUpdatedTimer?.Invoke(currentCountDown);
            
            if (currentCountDown <= 0)
            {
                StartRace();
                return;
            }

            currentCountDown--;
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
