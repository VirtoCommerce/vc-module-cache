using System.Collections.Generic;
using VirtoCommerce.Domain.Pricing.Model;
using VirtoCommerce.Domain.Pricing.Model.Search;
using VirtoCommerce.Domain.Pricing.Services;

namespace VirtoCommerce.CacheModule.Data.Decorators
{
    public sealed class PricingServicesDecorator : ICachedServiceDecorator, IPricingService
    {
        private readonly CacheManagerAdaptor _cacheManager;
        private readonly IPricingService _pricingService;
        public const string RegionName = "Pricing-Cache-Region";

        public PricingServicesDecorator(IPricingService inventoryService, CacheManagerAdaptor cacheManager)
        {
            _pricingService = inventoryService;
            _cacheManager = cacheManager;
        }

        #region ICachedServiceDecorator
        public void ClearCache()
        {
            _cacheManager.ClearRegion(RegionName);
        }
        #endregion

        #region IPricingService Members
        public Price[] GetPricesById(string[] ids)
        {
            var cacheKey = GetCacheKey("IPricingService.GetPricesById", string.Join(", ", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _pricingService.GetPricesById(ids));
            return retVal;
        }

        public Pricelist[] GetPricelistsById(string[] ids)
        {
            var cacheKey = GetCacheKey("IPricingService.GetPricelistsById", string.Join(", ", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _pricingService.GetPricelistsById(ids));
            return retVal;
        }

        public PricelistAssignment[] GetPricelistAssignmentsById(string[] ids)
        {
            var cacheKey = GetCacheKey("IPricingService.GetPricelistAssignmentsById", string.Join(", ", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _pricingService.GetPricelistAssignmentsById(ids));
            return retVal;
        }

        public void SavePrices(Price[] prices)
        {
            _pricingService.SavePrices(prices);
            ClearCache();
        }

        public void SavePricelists(Pricelist[] priceLists)
        {
            _pricingService.SavePricelists(priceLists);
            ClearCache();
        }

        public void SavePricelistAssignments(PricelistAssignment[] assignments)
        {
            _pricingService.SavePricelistAssignments(assignments);
            ClearCache();
        }

        public void DeletePricelists(string[] ids)
        {
            _pricingService.DeletePricelists(ids);
            ClearCache();
        }

        public void DeletePrices(string[] ids)
        {
            _pricingService.DeletePrices(ids);
            ClearCache();
        }

        public void DeletePricelistsAssignments(string[] ids)
        {
            _pricingService.DeletePricelistsAssignments(ids);
            ClearCache();
        }

        public void DeletePricelistsAssignmentsByFilter(PricelistAssignmentsSearchCriteria criteria)
        {
            _pricingService.DeletePricelistsAssignmentsByFilter(criteria);
            ClearCache();
        }

        public IEnumerable<Pricelist> EvaluatePriceLists(PriceEvaluationContext evalContext)
        {
            return _pricingService.EvaluatePriceLists(evalContext);
        }

        public IEnumerable<Price> EvaluateProductPrices(PriceEvaluationContext evalContext)
        {
            return _pricingService.EvaluateProductPrices(evalContext);
        }

        #endregion

        private static string GetCacheKey(params string[] parameters)
        {
            return "Pricing-" + string.Join(", ", parameters);
        }


    }
}
