using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Solana
{
    public class LoginButton : MonoBehaviour
    {
        [SerializeField] private WalletScreen walletScreen;
        [SerializeField] private TextMeshProUGUI messageTxt;
        
        private Button _loginButton;

        private void Start()
        {
            _loginButton = GetComponent<Button>();
            _loginButton.onClick.AddListener(LoginCheckerWalletAdapter);
            messageTxt.gameObject.SetActive(false);

            if (Application.platform is RuntimePlatform.LinuxEditor or RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor)
            {
                _loginButton.onClick.RemoveListener(LoginCheckerWalletAdapter);
                _loginButton.onClick.AddListener(() =>
                    Debug.LogError("Wallet adapter login is not yet supported in the editor"));
            }
        }

        private async void LoginCheckerWalletAdapter()
        {
            if(Web3.Instance == null) return;
            Loading.StartLoading();
            var account = await Web3.Instance.LoginWalletAdapter();
            messageTxt.text = "";
            CheckAccount(account);
        }


        private void CheckAccount(Account account)
        {
            if (account != null)
            {
                walletScreen.gameObject.SetActive(true);
                messageTxt.gameObject.SetActive(false);
            }
            else
            {
                messageTxt.gameObject.SetActive(true);
            }
        }
    }
}
