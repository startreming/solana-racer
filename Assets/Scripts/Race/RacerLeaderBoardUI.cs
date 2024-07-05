using Car;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Race
{
    public class RacerLeaderBoardUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text place;
        [SerializeField] private TMP_Text racerName;
        [SerializeField] private RawImage racerIcon;
        [SerializeField] private TMP_Text raceTime;

        public void UpdatePlayer(Racer racer)
        {
            place.text = racer.Place.ToString();
            racerName.text = racer.Represents.name;
            var timeElapsed = racer.TotalRaceTime.ToString(@"mm\:ss\:fff");
            raceTime.text = timeElapsed;
            
            var picture = racer.Represents.GetComponentInChildren<CarController>().ProfilePicture;
            var canvasRenderer = racerIcon.GetComponent<CanvasRenderer>();
            
            if (picture == null)
            {
                canvasRenderer.SetAlpha(0);
            } else
            {
                racerIcon.texture = picture;
                canvasRenderer.SetAlpha(1);
            }
        }
    }
}