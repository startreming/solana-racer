using UnityEngine;

namespace Race
{
    public class LapCheckpoint : MonoBehaviour
    {
        public int Progress => progress;

        [SerializeField] private int progress;
        [SerializeField] private LapManager lapManager;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                lapManager.VerifyPass(other.transform.parent.gameObject, this);
            }
        }
    } 
}