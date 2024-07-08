using System;
using UnityEngine;

namespace Car
{
    public class CarHitListener : MonoBehaviour
    {
        [SerializeField] private CarController controller;
        public event Action<float> OnHit = (magnitude) => { };

        private void OnCollisionEnter(Collision collision)
        {
        }
    }
}