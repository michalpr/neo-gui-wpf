﻿using Neo.Gui.Base.Dialogs.Interfaces;
using Neo.Gui.Base.Dialogs.LoadParameters.Contracts;

namespace Neo.Gui.Wpf.Views.Contracts
{
    /// <summary>
    /// Interaction logic for DeployContractView.xaml
    /// </summary>
    public partial class DeployContractView : IDialog<DeployContractLoadParameters>
    {
        public DeployContractView()
        {
            InitializeComponent();
        }
    }
}