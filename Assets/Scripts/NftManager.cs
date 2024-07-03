using System;
using System.Collections.Generic;
using Solana.Unity.SDK.Example;
using Solana.Unity.SDK.Nft;
using UnityEngine;
using UnityEngine.UI;

public class NftManager : MonoBehaviour
{
    [SerializeField] private WalletScreen walletScreen;
    [SerializeField] private RawImage logo;
    
    private Nft _nft;
    private Texture2D _nftTexture;

    public Nft Nft => _nft;
    public Texture2D NftTexture => _nftTexture;

    private void Start()
    {
        walletScreen.OnSelectedNft += SaveNft;
    }

    private void SaveNft(Nft nft)
    {
        _nft = nft;
        _nftTexture = nft.metaplexData?.nftImage?.file;
        logo.texture = _nftTexture;
    }
}
