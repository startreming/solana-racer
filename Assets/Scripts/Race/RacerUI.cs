using Car;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Race
{
    public class RacerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text place;
        [SerializeField] private TMP_Text racerName;
        [SerializeField] private RawImage racerIcon;

        public void UpdatePlayer(Racer racer, int placeIndex)
        {
            place.text = placeIndex.ToString();
            racerName.text = racer.Represents.name+$" {Mathf.Round(racer.RaceProgress*100)}%";
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