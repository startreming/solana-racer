using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private PauseManager pauseManager;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;
        
        private CanvasGroup canvasGroup;
        
        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            
            pauseManager.OnGamePaused += GamePaused;
            resumeButton.onClick.AddListener(pauseManager.ResumeGame);
            quitButton.onClick.AddListener(pauseManager.GoToMainMenu);
        }
        

        private void GamePaused(bool isPaused)
        {
            canvasGroup.alpha = isPaused ? 1 : 0;
        }
    }
}
