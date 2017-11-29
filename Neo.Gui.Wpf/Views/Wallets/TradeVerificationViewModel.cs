﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Neo.Core;
using Neo.Gui.Base.Controllers.Interfaces;
using Neo.Gui.Base.Data;
using Neo.Gui.Base.Helpers.Interfaces;
using Neo.Gui.Wpf.MVVM;

namespace Neo.Gui.Wpf.Views.Wallets
{
    public class TradeVerificationViewModel : ViewModelBase
    {
        private readonly IBlockChainController blockChainController;
        private readonly IDispatchHelper dispatchHelper;

        public TradeVerificationViewModel(
            IBlockChainController blockChainController,
            IDispatchHelper dispatchHelper)
        {
            this.blockChainController = blockChainController;
            this.dispatchHelper = dispatchHelper;

            this.Items = new ObservableCollection<TransactionOutputItem>();
        }

        public ObservableCollection<TransactionOutputItem> Items { get; }

        public ICommand AcceptCommand => new RelayCommand(this.Accept);

        public ICommand RefuseCommand => new RelayCommand(this.TryClose);


        public bool TradeAccepted { get; set; }

        private void Accept()
        {
            this.TradeAccepted = true;

            this.TryClose();
        }

        public void SetOutputs(IEnumerable<TransactionOutput> outputs)
        {
            this.dispatchHelper.InvokeOnMainUIThread(() =>
            {
                foreach (var output in outputs)
                {
                    var asset = this.blockChainController.GetAssetState(output.AssetId);

                    this.Items.Add(new TransactionOutputItem
                    {
                        AssetName = $"{asset.GetName()} ({asset.Owner})",
                        AssetId = output.AssetId,
                        Value = new BigDecimal(output.Value.GetData(), 8),
                        ScriptHash = output.ScriptHash
                    });
                }
            });
        }
    }
}