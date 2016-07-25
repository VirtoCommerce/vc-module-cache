using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CacheManager.Core;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.CacheModule.Web.Extensions;

namespace VirtoCommerce.CacheModule.Web.Decorators
{
    internal sealed class StoreServicesDecorator : IStoreService
    {
        private readonly CacheManagerAdaptor _cacheManager;
        private readonly IStoreService _storeService;
        private const string _regionName = "Store-Cache-Region";
        public StoreServicesDecorator(IStoreService storeService, CacheManagerAdaptor cacheManager)
        {
            _storeService = storeService;
            _cacheManager = cacheManager;
        }

        #region IStoreService Members
        public Store Create(Store store)
        {
            var retVal = _storeService.Create(store);
            _cacheManager.ClearRegion(_regionName);
            return retVal;
        }

        public void Delete(string[] ids)
        {
            _storeService.Delete(ids);
            _cacheManager.ClearRegion(_regionName);
        }

        public Store GetById(string id)
        {
            var cacheKey = GetCacheKey(id);
            var retVal = _cacheManager.Get(cacheKey, _regionName, () => {
                return _storeService.GetById(id);
            });
            return retVal;
        }

        public Store[] GetByIds(string[] ids)
        {
            var cacheKey = GetCacheKey(ids);
            var retVal = _cacheManager.Get(cacheKey, _regionName, () => {
                return _storeService.GetByIds(ids);
            });
            return retVal;
        }

        public IEnumerable<string> GetUserAllowedStoreIds(ApplicationUserExtended user)
        {
            return _storeService.GetUserAllowedStoreIds(user);
        }

        public SearchResult SearchStores(SearchCriteria criteria)
        {
            var cacheKey = GetCacheKey(criteria.ToJson().GetHashCode().ToString());
            var retVal = _cacheManager.Get(cacheKey, _regionName, () => {
                return _storeService.SearchStores(criteria);
            });
            return retVal;
        }

        public void Update(Store[] stores)
        {         
            _storeService.Update(stores);
            _cacheManager.ClearRegion(_regionName);
        } 
        #endregion

     
        private static string GetCacheKey(params string[] parameters)
        {
            return string.Format("Store-{0}", string.Join(", ", parameters).GetHashCode());
        }

    }
}