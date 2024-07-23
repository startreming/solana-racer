using System.Collections;
using UnityEngine;

namespace Sounds
{
    public class AmbienceSoundSystem : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip[] sounds;
        
        private int _currentIndex;
        
        private void Start()
        {
            StartCoroutine(PlaySounds());
        }

        private IEnumerator PlaySounds()
        {
            source.clip = sounds[_currentIndex];
            source.Play();
            yield return new WaitForSeconds(source.clip.length-0.5f);
            _currentIndex = Random.Range(0, sounds.Length - 1);
            StartCoroutine(PlaySounds());
        }
    }
}