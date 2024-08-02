using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Solana.Unity.Extensions;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using Solana.Unity.SDK.Example;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Solana
{
    public class WalletScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lamports;
        [SerializeField] private Button refreshBtn;
        [SerializeField] private Button logoutBtn;
        [SerializeField] private Button beginRaceBtn;
        
        [SerializeField] private GameObject tokenItem;
        [SerializeField] private Transform tokenContainer;
        
        public event Action<Unity.SDK.Nft.Nft> OnSelectedNft = token => {};

        private CancellationTokenSource _stopTask;
        private List<TokenItem> _instantiatedTokens = new();
        private static TokenMintResolver _tokenResolver;
        private bool _isLoadingTokens = false;

        public void Start()
        {
            beginRaceBtn.gameObject.SetActive(false);
            refreshBtn.onClick.AddListener(RefreshWallet);
            
            logoutBtn.onClick.AddListener(() =>
            {
                Web3.Instance.Logout();
                gameObject.SetActive(false);
            });

            _stopTask = new CancellationTokenSource();
        }

        private void RefreshWallet()
        {
            Web3.UpdateBalance().Forget();
            GetOwnedTokenAccounts().AsAsyncUnitUniTask().Forget();
        }

        private void OnEnable()
        {
            //Loading.StopLoading();
            Web3.OnBalanceChange += OnBalanceChange;
        }

        private void OnBalanceChange(double sol)
        {
            MainThreadDispatcher.Instance().Enqueue(() =>
            {
                lamports.text = $"{sol}";
            });
            GetOwnedTokenAccounts().AsAsyncUnitUniTask().Forget();
        }

        private void OnDisable()
        {
            Web3.OnBalanceChange -= OnBalanceChange;
        }
        
        private async UniTask GetOwnedTokenAccounts()
        {
            if(_isLoadingTokens) return;
            _isLoadingTokens = true;
            
            // Get compatible nft symbols
            var compatibleSymbols = await GetCompatibleSymbols("logo");
            
            var tokens = await Web3.Wallet.GetTokenAccounts(Commitment.Confirmed);
            if(tokens == null) return;
            // Remove tokens not owned anymore and update amounts
            var tkToRemove = new List<TokenItem>();
            _instantiatedTokens.ForEach(tk =>
            {
                var tokenInfo = tk.TokenAccount.Account.Data.Parsed.Info;
                var match = tokens.Where(t => t.Account.Data.Parsed.Info.Mint == tokenInfo.Mint).ToArray();
                if (match.Length == 0 || match.Any(m => m.Account.Data.Parsed.Info.TokenAmount.AmountUlong == 0))
                {
                    tkToRemove.Add(tk);
                }
                else
                {
                    var newAmount = match[0].Account.Data.Parsed.Info.TokenAmount.UiAmountString;
                    tk.UpdateAmount(newAmount);
                }
            });

            tkToRemove.ForEach(tk =>
            {
                _instantiatedTokens.Remove(tk);
                Destroy(tk.gameObject);
            });
            // Add new tokens
            List<UniTask> loadingTasks = new List<UniTask>();
            if (tokens is {Length: > 0})
            {
                var tokenAccounts = tokens.OrderByDescending(
                    tk => tk.Account.Data.Parsed.Info.TokenAmount.AmountUlong);
                foreach (var item in tokenAccounts)
                {
                    if (!(item.Account.Data.Parsed.Info.TokenAmount.AmountUlong > 0)) break;
                    if (_instantiatedTokens.All(t => t.TokenAccount.Account.Data.Parsed.Info.Mint != item.Account.Data.Parsed.Info.Mint))
                    {
                        var loadTask = Unity.SDK.Nft.Nft.TryGetNftData(item.Account.Data.Parsed.Info.Mint,
                            Web3.Instance.WalletBase.ActiveRpcClient).AsUniTask();
                        loadingTasks.Add(loadTask);
                        loadTask.ContinueWith(nft =>
                        {
                            // Filter nft by symbol
                            Debug.Log(nft.metaplexData.data.metadata.name+ " - Symbol: "+nft.metaplexData.data.metadata.symbol);
                            if (nft.metaplexData.data.metadata.symbol != null 
                                /* TODO: Uncomment
                                 && compatibleSymbols.Contains(nft.metaplexData.data.metadata.symbol)*/
                                )
                            {
                                var tk = Instantiate(tokenItem, tokenContainer, true);
                                tk.transform.localScale = Vector3.one;
                                
                                Debug.Log(nft.metaplexData.data.metadata.name+ " compatible");
                                TokenItem tkInstance = tk.GetComponent<TokenItem>();
                                tkInstance.SetWalletScreen(this);
                                _instantiatedTokens.Add(tkInstance);
                                tk.SetActive(true);
                                if (tkInstance)
                                {
                                    tkInstance.InitializeData(item, nft).Forget();
                                }
                            }
                        }).Forget();
                    }
                }
            }
            await UniTask.WhenAll(loadingTasks);
            _isLoadingTokens = false;
            Loading.StopLoading();
        }
        
        private async UniTask<List<string>> GetCompatibleSymbols(string placement)
        {
            string url = $"https://solanaracer-web.vercel.app/api/supported?placement={placement}";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var response = await request.SendWebRequest();
                if (response.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error fetching compatible symbols: {response.error}");
                    return new List<string>();
                }
                else
                {
                    string json = response.downloadHandler.text;
                    var data = JsonUtility.FromJson<SupportedSymbolsResponse>(json);
                    
                    Debug.Log("Nft symbols loaded successfully");
                    var log = "Symbols: ";
                    foreach (var symbol in data.symbols)
                    {
                        log += symbol + " ";
                    }
                    Debug.Log(log);
                    return data.symbols;
                }
            }
        }

        [System.Serializable]
        private class SupportedSymbolsResponse
        {
            public List<string> symbols;
        }

        public void InvokeOnSelectedToken(Unity.SDK.Nft.Nft nft)
        {
            OnSelectedNft?.Invoke(nft);
            beginRaceBtn.gameObject.SetActive(true);
        }
        
        public static async UniTask<TokenMintResolver> GetTokenMintResolver()
        {
            if(_tokenResolver != null) return _tokenResolver;
            var tokenResolver = await TokenMintResolver.LoadAsync();
            if(tokenResolver != null) _tokenResolver = tokenResolver;
            return _tokenResolver;
        }

        private void OnDestroy()
        {
            if (_stopTask is null) return;
            _stopTask.Cancel();
        }

    }
}
