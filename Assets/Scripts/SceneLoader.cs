using UI;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private const string gameSceneName = "Track1";
    private const string menuSceneName = "WalletSolana";

    public static void LoadGameScene()
    {
        Loading.StartLoading();
        SceneManager.LoadScene(gameSceneName);
    }

    public static void LoadMenuScene()
    {
        Loading.StartLoading();
        SceneManager.LoadScene(menuSceneName);
    }
}
