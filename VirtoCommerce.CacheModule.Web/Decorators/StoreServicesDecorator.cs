using System;
using System.Collections.Generic;
using VirtoCommerce.CacheModule.Web.Extensions;
using VirtoCommerce.CacheModule.Web.Services;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CacheModule.Web.Decorators
{
    internal sealed class StoreServicesDecorator : ICachedServiceDecorator, IStoreService
    {
        private readonly CacheManagerAdaptor _cacheManager;
        private readonly IStoreService _storeService;
        public const string RegionName = "Store-Cache-Region";

        public StoreServicesDecorator(IStoreService storeService, CacheManagerAdaptor cacheManager)
        {
            _storeService = storeService;
            _cacheManager = cacheManager;
        }

        #region ICachedServiceDecorator
        public void ClearCache()
        {
            _cacheManager.ClearRegion(RegionName);
        }
        #endregion

        #region IStoreService Members
        public Store Create(Store store)
        {
            var retVal = _storeService.Create(store);
            ClearCache();
            return retVal;
        }

        public void Delete(string[] ids)
        {
            _storeService.Delete(ids);
            ClearCache();
        }

        public Store GetById(string id)
        {
            var cacheKey = GetCacheKey("StoreService.GetById", id);
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _storeService.GetById(id));
            return retVal;
        }

        public Store[] GetByIds(string[] ids)
        {
            var cacheKey = GetCacheKey("StoreService.GetByIds", string.Join(",", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _storeService.GetByIds(ids));
            return retVal;
        }

        public IEnumerable<string> GetUserAllowedStoreIds(ApplicationUserExtended user)
        {
            return _storeService.GetUserAllowedStoreIds(user);
        }

        public SearchResult SearchStores(SearchCriteria criteria)
        {
            var cacheKey = GetCacheKey("StoreService.SearchStores", criteria.ToJson().GetHashCode().ToString());
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _storeService.SearchStores(criteria));
            return retVal;
        }

        public void Update(Store[] stores)
        {
            _storeService.Update(stores);
            ClearCache();
        }
        #endregion


        private static string GetCacheKey(params string[] parameters)
        {
            return "Store-" + string.Join(", ", parameters).GetHashCode();
        }
    }
}
