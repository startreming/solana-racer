using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadGameButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(SceneLoader.LoadGameScene);
        }
    }
}
