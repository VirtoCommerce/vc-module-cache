using System;
using System.Collections.Generic;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Services;

namespace VirtoCommerce.CacheModule.Data.Decorators
{
    public sealed class CommerceServiceDecorator : ICachedServiceDecorator, ICommerceService
    {
        private readonly ICommerceService _commerceService;
        private readonly IList<ICachedServiceDecorator> _allSeoInfoRelatedDecorators;

        public CommerceServiceDecorator(ICommerceService commerceService, IList<ICachedServiceDecorator> allSeoInfoRelatedDecorators)
        {
            _commerceService = commerceService;
            _allSeoInfoRelatedDecorators = allSeoInfoRelatedDecorators;
        }

        #region ICachedServiceDecorator
        public void ClearCache()
        {
            if (_allSeoInfoRelatedDecorators != null)
            {
                foreach (var decorator in _allSeoInfoRelatedDecorators)
                {
                    decorator.ClearCache();
                }
            }
        }
        #endregion

        #region ICommerceService Members
        public void DeleteCurrencies(string[] codes)
        {
            _commerceService.DeleteCurrencies(codes);
        }

        public void DeleteFulfillmentCenter(string[] ids)
        {
            _commerceService.DeleteFulfillmentCenter(ids);
        }

        public void DeletePackageTypes(string[] ids)
        {
            _commerceService.DeletePackageTypes(ids);
        }

        public IEnumerable<Currency> GetAllCurrencies()
        {
            return _commerceService.GetAllCurrencies();
        }

        public IEnumerable<FulfillmentCenter> GetAllFulfillmentCenters()
        {
            return _commerceService.GetAllFulfillmentCenters();
        }

        public IEnumerable<PackageType> GetAllPackageTypes()
        {
            return _commerceService.GetAllPackageTypes();
        }

        public IEnumerable<SeoInfo> GetAllSeoDuplicates()
        {
            return _commerceService.GetAllSeoDuplicates();
        }

        public IEnumerable<SeoInfo> GetSeoByKeyword(string keyword)
        {
            return _commerceService.GetSeoByKeyword(keyword);
        }

        public void LoadSeoForObjects(ISeoSupport[] seoSupportObjects)
        {
            _commerceService.LoadSeoForObjects(seoSupportObjects);
        }

        public void UpsertCurrencies(Currency[] currencies)
        {
            _commerceService.UpsertCurrencies(currencies);
        }

        public FulfillmentCenter UpsertFulfillmentCenter(FulfillmentCenter fullfilmentCenter)
        {
            return _commerceService.UpsertFulfillmentCenter(fullfilmentCenter);
        }

        public void UpsertPackageTypes(PackageType[] packageTypes)
        {
            _commerceService.UpsertPackageTypes(packageTypes);
        }

        public void UpsertSeoForObjects(ISeoSupport[] seoSupportObjects)
        {
            _commerceService.UpsertSeoForObjects(seoSupportObjects);
        }

        public void UpsertSeoInfos(SeoInfo[] seoinfos)
        {
            _commerceService.UpsertSeoInfos(seoinfos);
            ClearCache();
        }

        public void DeleteSeoForObject(ISeoSupport seoSupportObject)
        {
            _commerceService.DeleteSeoForObject(seoSupportObject);
        }
        #endregion
    }
}
