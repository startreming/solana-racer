using Solana;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    
    public void LoadScene()
    {
        Loading.StartLoading();
        SceneManager.LoadScene(sceneName);
    }
}
