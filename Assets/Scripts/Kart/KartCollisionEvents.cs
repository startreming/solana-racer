using System;
using UnityEngine;

namespace Kart
{
    public class KartCollisionEvents : MonoBehaviour
    {
        public event Action<GameObject, Collision> OnCollided = (go, col) => { };
        public event Action<GameObject, Collider> OnTriggered = (go, col) => { };
        
        private void OnCollisionEnter(Collision collision)
        {
            OnCollided?.Invoke(gameObject, collision);
        }
        
        private void OnTriggerEnter(Collider collider)
        {
            OnTriggered?.Invoke(gameObject, collider);
            Debug.Log("Trigger");
        }
    }
    
}