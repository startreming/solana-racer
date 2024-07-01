using TMPro;
using UnityEngine;

namespace Race
{
    public class RacerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text place;
        [SerializeField] private TMP_Text racerName;

        public void UpdatePlayer(Racer racer)
        {
            place.text = racer.Place.ToString();
            racerName.text = racer.Represents.name;
        }
    }
}