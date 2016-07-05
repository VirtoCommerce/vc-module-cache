using System;
using Common.Logging;
using Microsoft.Practices.Unity;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CacheModule.Web
{
    public class Module : ModuleBase
    {     
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members

        public override void Initialize()
        {
      
        }

        public override void PostInitialize()
        {
          
        }
        #endregion
    }
}