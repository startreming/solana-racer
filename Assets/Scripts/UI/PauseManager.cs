using System;
using Solana;
using UnityEngine;

namespace UI
{
    public class PauseManager : MonoBehaviour
    {
        private bool _isPaused = false;
        private bool _canPause = true;

        public event Action<bool> OnGamePaused = isPaused => { };

        public void SetCanPause(bool canPause)
        {
            _canPause = canPause;
        }

        private void Start()
        {
            _isPaused = false;
            AudioListener.pause = false;
        }

        private void Update()
        {
            if (!_canPause) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
            _isPaused = true;
            OnGamePaused?.Invoke(_isPaused);
            AudioListener.pause = true;
        }
    
        public void ResumeGame()
        {
            Time.timeScale = 1f;
            _isPaused = false;
            OnGamePaused?.Invoke(_isPaused);
            AudioListener.pause = false;
        }
    
        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            var nftManager = FindObjectOfType<NftManager>();
            if (nftManager != null)
            {
                Destroy(nftManager.gameObject);
            }
            SceneLoader.LoadMenuScene();
        }
    }
}