using System;
using UnityEngine;

namespace UI
{
    public class PauseManager : MonoBehaviour
    {
        private bool _isPaused = false;

        public event Action<bool> OnGamePaused = isPaused => { };

        private void Update()
        {
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
        }
    
        public void ResumeGame()
        {
            Time.timeScale = 1f;
            _isPaused = false;
            OnGamePaused?.Invoke(_isPaused);
        }
    
        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneLoader.LoadMenuScene();
        }
    }
}