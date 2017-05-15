using System.Collections.Generic;
using VirtoCommerce.Domain.Inventory.Model;
using VirtoCommerce.Domain.Inventory.Services;

namespace VirtoCommerce.CacheModule.Data.Decorators
{
    public sealed class InventoryServicesDecorator : ICachedServiceDecorator, IInventoryService
    {
        private readonly CacheManagerAdaptor _cacheManager;
        private readonly IInventoryService _inventoryService;
        public const string RegionName = "Inventory-Cache-Region";

        public InventoryServicesDecorator(IInventoryService inventoryService, CacheManagerAdaptor cacheManager)
        {
            _inventoryService = inventoryService;
            _cacheManager = cacheManager;
        }

        #region ICachedServiceDecorator
        public void ClearCache()
        {
            _cacheManager.ClearRegion(RegionName);
        }
        #endregion

        #region IStoreService Members
        public IEnumerable<InventoryInfo> GetAllInventoryInfos()
        {
            var cacheKey = GetCacheKey("InventoryService.GetAllInventoryInfos");
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _inventoryService.GetAllInventoryInfos());
            return retVal;
        }

        public IEnumerable<InventoryInfo> GetProductsInventoryInfos(IEnumerable<string> productIds)
        {
            var cacheKey = GetCacheKey("InventoryService.GetProductsInventoryInfos", string.Join(",", productIds));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _inventoryService.GetProductsInventoryInfos(productIds));
            return retVal;
        }

        public void UpsertInventories(IEnumerable<InventoryInfo> inventoryInfos)
        {
            _inventoryService.UpsertInventories(inventoryInfos);
            ClearCache();
        }

        public InventoryInfo UpsertInventory(InventoryInfo inventoryInfo)
        {
            var retVal = _inventoryService.UpsertInventory(inventoryInfo);
            ClearCache();
            return retVal;
        }
        #endregion

        private static string GetCacheKey(params string[] parameters)
        {
            return "Inventory-" + string.Join(", ", parameters).GetHashCode();
        }
    }
}
