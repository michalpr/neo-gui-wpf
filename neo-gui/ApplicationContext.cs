﻿using System;
using Autofac;
using Neo.Implementations.Wallets.EntityFramework;

namespace Neo
{
    public class ApplicationContext : IApplicationContext
    {
        #region Singleton Pattern
        private static Lazy<ApplicationContext> _lazyInstance = new Lazy<ApplicationContext>(() => new ApplicationContext());

        public static ApplicationContext Instance
        {
            get
            {
                return _lazyInstance.Value;
            }
        }
        #endregion

        public ILifetimeScope ContainerLifetimeScope { get; set; }

        public UserWallet CurrentWallet { get; set; }
    }
}