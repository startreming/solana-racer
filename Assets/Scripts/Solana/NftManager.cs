using Cysharp.Threading.Tasks;
using Solana.Unity.Extensions.Models.TokenMint;
using Solana.Unity.Rpc.Models;
using Solana.Unity.SDK.Nft;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebSocketSharp;

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

        public async UniTask<Texture2D> GetNftImageUrl(Nft nft, TokenAccount tokenAccount = null)
        {
            var onChainSymbol = nft.metaplexData.data.metadata.symbol;
            var offChainSymbol = nft.metaplexData.data.offchainData.symbol;
            var usedSymbol = !string.IsNullOrEmpty(onChainSymbol) ? onChainSymbol : offChainSymbol;

            var imgUrl = "";
            if (!string.IsNullOrEmpty(nft.metaplexData.data.metadata.uri))
                imgUrl = nft.metaplexData.data.metadata.uri;
            
            if (nft.metaplexData?.data?.offchainData?.default_image != null)
            {
                imgUrl = nft.metaplexData?.data?.offchainData?.default_image;
            }
            else
            {
                if (tokenAccount != null)
                {
                    var tokenMintResolver = await WalletScreen.GetTokenMintResolver();
                    TokenDef tokenDef = tokenMintResolver.Resolve(tokenAccount.Account.Data.Parsed.Info.Mint);
                    if(tokenDef.TokenName.IsNullOrEmpty() 
                       || tokenDef.Symbol.IsNullOrEmpty()) 
                        return Texture2D.blackTexture;
                    imgUrl = tokenDef.TokenLogoUrl;
                }
            }
            
            
            var encodedImgUrl = UnityWebRequest.EscapeURL(imgUrl);
            var placement = "logo";
            var requestUrl = 
                $"https://solanaracer-web.vercel.app/api/asset?symbol={usedSymbol}&placement={placement}&imgUrl={encodedImgUrl}";

            using UnityWebRequest request = UnityWebRequest.Get(requestUrl);
            
            var response = await request.SendWebRequest();

            if (response.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error obtaining asset: {response.error}");
                return Texture2D.blackTexture;
            }
                
            Texture2D texture = new Texture2D(75, 75);
            texture.LoadImage(response.downloadHandler.data);
                
            Debug.Log("Nft image loaded successfully");
            return texture;
        }

        private void SaveNft(Nft nft)
        {
            _nft = nft;
            LoadNft(nft).AsAsyncUnitUniTask().Forget();
        }

        private async UniTask LoadNft(Nft nft)
        {
            _nft = nft;
            
            var texture = await GetNftImageUrl(nft);
            
            _nftTexture = texture;
            logo.texture = _nftTexture;
        }
    }
}
