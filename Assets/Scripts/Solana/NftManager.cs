using Cysharp.Threading.Tasks;
using Solana.Unity.SDK.Nft;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Solana
{
    public class NftManager : MonoBehaviour
    {
        public static NftManager Instance;
    
        [SerializeField] private WalletScreen walletScreen;
        [SerializeField] private RawImage logo;
    
        private Nft _nft;
        private Texture2D _nftTexture;

        public Nft Nft => _nft;
        public Texture2D NftTexture => _nftTexture;

        private void Awake()
        {
            if (Instance != null) Destroy(this);
            Instance = this;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            if (walletScreen != null) walletScreen.OnSelectedNft += SaveNft;
        }

        private void SaveNft(Nft nft)
        {
            _nft = nft;
            LoadNft(nft).AsAsyncUnitUniTask().Forget();
        }
        
        private async UniTask LoadNft(Nft nft)
        {
            _nft = nft;
            var imgUrl = nft.metaplexData?.nftImage?.externalUrl;

            if (string.IsNullOrEmpty(imgUrl))
            {
                Debug.LogError("NFT image URL not found.");
                return;
            }

            var encodedImgUrl = UnityWebRequest.EscapeURL(imgUrl);
            var symbol = nft.metaplexData.data.metadata.symbol;
            var placement = "logo";
            var requestUrl = $"https://solanaracer-web.vercel.app/api/asset?symbol={symbol}&placement={placement}&imgUrl={encodedImgUrl}";
            Debug.Log("Request url: "+requestUrl);

            using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
            {
                var response = await request.SendWebRequest();

                if (response.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error obtaining asset: {response.error}");
                    return;
                }
                
                Texture2D texture = new Texture2D(75, 75);
                texture.LoadImage(response.downloadHandler.data);
                
                _nftTexture = texture;
                logo.texture = _nftTexture;
                Debug.Log("Nft image loaded successfully");
            }
        }

    }
}
