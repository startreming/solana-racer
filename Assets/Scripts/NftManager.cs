using System;
using Solana;
using Solana.Unity.SDK.Example;
using Solana.Unity.SDK.Nft;
using UnityEngine;
using UnityEngine.UI;

public class NftManager : MonoBehaviour
{
    public static NftManager Instance;
    
    [SerializeField] private WalletScreen walletScreen;
    [SerializeField] private RawImage logo;
    
    private Nft _nft;
    [SerializeField] private Texture2D _nftTexture;

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
        _nftTexture = nft.metaplexData?.nftImage?.file;
        logo.texture = _nftTexture;
    }
}
