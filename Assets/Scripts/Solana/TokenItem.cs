using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Solana.Unity.Extensions.Models.TokenMint;
using Solana.Unity.Rpc.Models;
using Solana.Unity.SDK.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace Solana
{
    public class TokenItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pub_txt;
        [SerializeField] private TextMeshProUGUI amount_txt;

        public RawImage logo;

        public Button transferButton;
        public Button selectButton;

        public TokenAccount TokenAccount;
        private Unity.SDK.Nft.Nft _nft;
        private Texture2D _texture;
        
        private WalletScreen _walletScreen;

        public Unity.SDK.Nft.Nft Nft => _nft;

        private void Awake()
        {
            logo = GetComponentInChildren<RawImage>();
        }

        private void Start()
        {
            selectButton.onClick.AddListener(SelectNft);
        }

        public async UniTask InitializeData(TokenAccount tokenAccount, Unity.SDK.Nft.Nft nftData = null)
        {
            TokenAccount = tokenAccount;
            if (nftData != null && ulong.Parse(tokenAccount.Account.Data.Parsed.Info.TokenAmount.Amount) == 1)
            {
                await UniTask.SwitchToMainThread();
                _nft = nftData;
                amount_txt.text = "";
                pub_txt.text = nftData.metaplexData?.data?.offchainData?.name;

                if (logo != null)
                {
                    logo.texture = await NftManager.Instance.GetNftImageUrl(nftData);
                }
            }
            else
            {
                amount_txt.text =
                    tokenAccount.Account.Data.Parsed.Info.TokenAmount.AmountDecimal.ToString(CultureInfo
                        .CurrentCulture);
                pub_txt.text = nftData?.metaplexData?.data?.offchainData?.name ?? tokenAccount.Account.Data.Parsed.Info.Mint;
                if (nftData?.metaplexData?.data?.offchainData?.symbol != null)
                {
                    pub_txt.text += $" ({nftData?.metaplexData?.data?.offchainData?.symbol})";
                }

                var texture = await NftManager.Instance.GetNftImageUrl(nftData, tokenAccount);
                LoadAndCacheTokenLogo(texture, tokenAccount.Account.Data.Parsed.Info.Mint);
            }
        }

        private void LoadAndCacheTokenLogo(Texture2D texture, string tokenMint)
        {
            if(tokenMint.IsNullOrEmpty() || texture is null) 
                return;
            
            _texture = FileLoader.Resize(texture, 75, 75);
            FileLoader.SaveToPersistentDataPath(Path.Combine(Application.persistentDataPath, $"{tokenMint}.png"), _texture);
            logo.texture = _texture;
        }

        public void SetWalletScreen(WalletScreen walletScreen)
        {
            _walletScreen = walletScreen;
        }

        public void UpdateAmount(string newAmount)
        {
            MainThreadDispatcher.Instance().Enqueue(() => { amount_txt.text = newAmount; });
        }

        private void SelectNft()
        {
            if (_nft != null && _walletScreen != null)
            {
                _walletScreen.InvokeOnSelectedToken(_nft);
            }
        }
    }
}
