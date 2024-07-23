using Car;
using UnityEngine;

namespace Sounds
{
    public class CarDriftSound : MonoBehaviour
    {
        [SerializeField] private CarController controller;
        [SerializeField] private AudioSource driftSource;

        private void Start()
        {
            driftSource.volume = 0f;
            driftSource.PlayDelayed(0.1f);
        }

        private void Update()
        {
            driftSource.volume = Mathf.Lerp(driftSource.volume, controller.IsDrifting ? 1 : 0, Time.deltaTime*2f);
        }
    }
}