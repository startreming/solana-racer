using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Race
{
    public class RacerLeaderBoardUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text place;
        [SerializeField] private TMP_Text racerName;
        [SerializeField] private Image racerIcon;
        [SerializeField] private TMP_Text raceTime;

        public void UpdatePlayer(Racer racer)
        {
            place.text = racer.Place.ToString();
            racerName.text = racer.Represents.name;
            var timeElapsed = racer.TotalRaceTime.ToString(@"mm\:ss\:fff");
            raceTime.text = timeElapsed;
        }
    }
}