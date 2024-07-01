using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Race
{
    public class RaceUI : MonoBehaviour
    {
        [SerializeField] private LapManager lapManager;
        [SerializeField] private TMP_Text currentLap;
        [SerializeField] private TMP_Text currentPlace;
        [SerializeField] private RacerUI racer;
        [SerializeField] private Transform racersContainer;

        private void Start()
        {
            lapManager.OnUpdatedPlace += UpdatePlace;
            lapManager.OnUpdatedLap += UpdateLap;
            lapManager.OnUpdatedRacers += UpdateRacers;
        }

        private void OnDestroy()
        {
            lapManager.OnUpdatedPlace -= UpdatePlace;
            lapManager.OnUpdatedLap -= UpdateLap;
            lapManager.OnUpdatedRacers -= UpdateRacers;
        }

        private void UpdateLap(int newLap)
        {
            currentLap.text = newLap.ToString();
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
                racerData.Place = place+1;
                var racerUI = Instantiate(racer, racersContainer);
                racerUI.UpdatePlayer(racerData);
                racerUI.transform.SetSiblingIndex(place);
            }
        }
    }
}