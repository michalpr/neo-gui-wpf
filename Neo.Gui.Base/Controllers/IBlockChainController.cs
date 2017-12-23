﻿using System;

using Neo.Core;

namespace Neo.Gui.Base.Controllers
{
    public interface IBlockchainController : IBaseBlockchainController
    {
        uint BlockHeight { get; }

        event EventHandler<Block> PersistCompleted;

        void Initialize();

        BlockchainStatus GetStatus();

        Transaction GetTransaction(UInt256 hash);

        Transaction GetTransaction(UInt256 hash, out int height);

        AccountState GetAccountState(UInt160 scriptHash);

        ContractState GetContractState(UInt160 scriptHash);

        AssetState GetAssetState(UInt256 assetId);

        DateTime GetTimeOfBlock(uint blockHeight);
    }
}