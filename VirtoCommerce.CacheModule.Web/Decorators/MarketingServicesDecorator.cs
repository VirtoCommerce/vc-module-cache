using System;
using System.Collections.Generic;
using VirtoCommerce.CacheModule.Web.Extensions;
using VirtoCommerce.CacheModule.Web.Services;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.Domain.Marketing.Model.Promotions.Search;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CacheModule.Web.Decorators
{
    internal sealed class MarketingServicesDecorator : ICachedServiceDecorator, IDynamicContentService, IPromotionSearchService, IPromotionService, ICouponService
    {
        private readonly CacheManagerAdaptor _cacheManager;
        private readonly IDynamicContentService _contentService;
        private readonly IPromotionSearchService _promoSearchService;
        private readonly IPromotionService _promotionService;
        private readonly ICouponService _couponService;

        public const string RegionName = "Marketing-Cache-Region";

        public MarketingServicesDecorator(CacheManagerAdaptor cacheManager, IDynamicContentService contentService, IPromotionSearchService promoSearchService, IPromotionService promoService, ICouponService couponService)
        {
            _contentService = contentService;
            _cacheManager = cacheManager;
            _promoSearchService = promoSearchService;
            _promotionService = promoService;
            _couponService = couponService;
        }

        #region ICachedServiceDecorator
        public void ClearCache()
        {
            _cacheManager.ClearRegion(RegionName);
        }
        #endregion

        #region IDynamicContentService Members
        public DynamicContentFolder[] GetFoldersByIds(string[] ids)
        {
            var cacheKey = GetCacheKey("IDynamicContentService.GetFoldersByIds", string.Join(", ", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _contentService.GetFoldersByIds(ids));
            return retVal;
        }

        public void SaveFolders(DynamicContentFolder[] folders)
        {
            _contentService.SaveFolders(folders);
            ClearCache();
        }

        public void DeleteFolders(string[] ids)
        {
            _contentService.DeleteFolders(ids);
            ClearCache();
        }

        public DynamicContentItem[] GetContentItemsByIds(string[] ids)
        {
            var cacheKey = GetCacheKey("IDynamicContentService.GetContentItemsByIds", string.Join(", ", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _contentService.GetContentItemsByIds(ids));
            return retVal;
        }

        public void SaveContentItems(DynamicContentItem[] items)
        {
            _contentService.SaveContentItems(items);
            ClearCache();
        }

        public void DeleteContentItems(string[] ids)
        {
            _contentService.DeleteContentItems(ids);
            ClearCache();
        }

        public DynamicContentPlace[] GetPlacesByIds(string[] ids)
        {
            var cacheKey = GetCacheKey("IDynamicContentService.GetPlacesByIds", string.Join(", ", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _contentService.GetPlacesByIds(ids));
            return retVal;
        }

        public void SavePlaces(DynamicContentPlace[] places)
        {
            _contentService.SavePlaces(places);
            ClearCache();
        }

        public void DeletePlaces(string[] ids)
        {
            _contentService.DeletePlaces(ids);
            ClearCache();
        }

        public DynamicContentPublication[] GetPublicationsByIds(string[] ids)
        {
            var cacheKey = GetCacheKey("IDynamicContentService.GetPublicationsByIds", string.Join(", ", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _contentService.GetPublicationsByIds(ids));
            return retVal;
        }

        public void SavePublications(DynamicContentPublication[] publications)
        {
            _contentService.SavePublications(publications);
            ClearCache();
        }

        public void DeletePublications(string[] ids)
        {
            _contentService.DeletePublications(ids);
            ClearCache();
        }
        #endregion

        #region IPromotionSearchService Members
        public GenericSearchResult<Promotion> SearchPromotions(PromotionSearchCriteria criteria)
        {
            var cacheKey = GetCacheKey("IPromotionSearchService.SearchPromotions", criteria.ToJson().GetHashCode().ToString());
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _promoSearchService.SearchPromotions(criteria));
            return retVal;
        }
        #endregion

        #region IPromotionService Members

        public Promotion[] GetPromotionsByIds(string[] ids)
        {
            var cacheKey = GetCacheKey("IPromotionService.GetPromotionsByIds", string.Join(", ", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _promotionService.GetPromotionsByIds(ids));
            return retVal;
        }

        public void SavePromotions(Promotion[] promotions)
        {
            _promotionService.SavePromotions(promotions);
            ClearCache();
        }

        public void DeletePromotions(string[] ids)
        {
            _promotionService.DeletePromotions(ids);
            ClearCache();
        }
        #endregion

        #region ICouponService Members
        public GenericSearchResult<Coupon> SearchCoupons(CouponSearchCriteria criteria)
        {
            var cacheKey = GetCacheKey("ICouponService.SearchCoupons", criteria.ToJson().GetHashCode().ToString());
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _couponService.SearchCoupons(criteria));
            return retVal;
        }

        public Coupon[] GetByIds(string[] ids)
        {
            var cacheKey = GetCacheKey("ICouponService.GetByIds", string.Join(", ", ids));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _couponService.GetByIds(ids));
            return retVal;
        }

        public void SaveCoupons(Coupon[] coupons)
        {
            _couponService.SaveCoupons(coupons);
            ClearCache();
        }

        public void DeleteCoupons(string[] ids)
        {
            _couponService.DeleteCoupons(ids);
            ClearCache();
        }
        #endregion

        private static string GetCacheKey(params string[] parameters)
        {
            return "Marketing-" + string.Join(", ", parameters).GetHashCode();
        }

       
    }
}
