using System;
using CacheManager.Core;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Common;

namespace VirtoCommerce.CacheModule.Web.Decorators
{
    internal sealed class CacheManagerAdaptor
    {
        private readonly ICacheManager<object> _cacheManager;
        private readonly ISettingsManager _settingManager;

        public CacheManagerAdaptor(ISettingsManager settingManager, ICacheManager<object> cacheManager)
        {
            _settingManager = settingManager;
            _cacheManager = cacheManager;
        }

        public T Get<T>(string cacheKey, string region, Func<T> getValueFunction)
        {
            if (_settingManager.GetValue("Cache.Enable", true))
            {
                var expirationTimeout = _settingManager.GetValue("Cache.ExpirationTimeout", 60);
                var result = _cacheManager.Get(cacheKey, region, TimeSpan.FromSeconds(expirationTimeout), getValueFunction);
                return result;
            }
            return getValueFunction();
        }

        public void ClearRegion(string region)
        {
            if (_settingManager.GetValue("Cache.Enable", true))
            {
                _cacheManager.ClearRegion(region);
            }
        }
    }
}
