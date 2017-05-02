using System;
using System.Linq;
using CacheManager.Core;
using Microsoft.Practices.Unity;
using VirtoCommerce.CacheModule.Web.Decorators;
using VirtoCommerce.CacheModule.Web.Services;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

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
            base.Initialize();           
        }

        public override void PostInitialize()
        {
            var cacheManagerAdaptor = _container.Resolve<CacheManagerAdaptor>();
          
            var storeServiceDecorator = new StoreServicesDecorator(_container.Resolve<IStoreService>(), cacheManagerAdaptor);
            _container.RegisterInstance<IStoreService>(storeServiceDecorator);

            var catalogServicesDecorator = new CatalogServicesDecorator(_container.Resolve<IItemService>(), _container.Resolve<ICatalogSearchService>(), _container.Resolve<IPropertyService>(), _container.Resolve<ICategoryService>(), _container.Resolve<ICatalogService>(), cacheManagerAdaptor);
            _container.RegisterInstance<IItemService>(catalogServicesDecorator);
            _container.RegisterInstance<ICatalogSearchService>(catalogServicesDecorator);
            _container.RegisterInstance<IPropertyService>(catalogServicesDecorator);
            _container.RegisterInstance<ICategoryService>(catalogServicesDecorator);
            _container.RegisterInstance<ICatalogService>(catalogServicesDecorator);


            var memberServicesDecorator = new MemberServicesDecorator(_container.Resolve<IMemberService>(), _container.Resolve<IMemberSearchService>(), cacheManagerAdaptor);
            _container.RegisterInstance<IMemberService>(memberServicesDecorator);
            _container.RegisterInstance<IMemberSearchService>(memberServicesDecorator);

            var commerceServicesDecorator = new CommerceServiceDecorator(_container.Resolve<ICommerceService>(), new ICachedServiceDecorator[] { catalogServicesDecorator, storeServiceDecorator });
            _container.RegisterInstance<ICommerceService>(commerceServicesDecorator);

            var marketingServicesDecorator = new MarketingServicesDecorator(cacheManagerAdaptor, _container.Resolve<IDynamicContentService>(), _container.Resolve<IPromotionSearchService>(), _container.Resolve<IPromotionService>(), _container.Resolve<ICouponService>());
            _container.RegisterInstance<IDynamicContentService>(marketingServicesDecorator);
            _container.RegisterInstance<IPromotionSearchService>(marketingServicesDecorator);
            _container.RegisterInstance<IPromotionService>(marketingServicesDecorator);
            _container.RegisterInstance<ICouponService>(marketingServicesDecorator);

            var changeTrackingService = new ChangesTrackingService();
            _container.RegisterInstance<IChangesTrackingService>(changeTrackingService);
            var cacheManager = _container.Resolve<ICacheManager<object>>();
            var observedRegions = new[] { StoreServicesDecorator.RegionName, CatalogServicesDecorator.RegionName, MemberServicesDecorator.RegionName, MarketingServicesDecorator.RegionName};
            //Need observe cache events to correct update latest changes timestamp when platform running on multiple instances
            //Cache clean event will be raising  thanks to Redis cache synced invalidation
            cacheManager.OnClear += (e, args) =>
            {
                changeTrackingService.Update(null, DateTime.UtcNow);
            };
            cacheManager.OnClearRegion += (e, args) =>
            {
                if (args.Region != null && observedRegions.Any(x => x.EqualsInvariant(args.Region)))
                {
                    changeTrackingService.Update(null, DateTime.UtcNow);
                }
            };
        }
        #endregion
    }
}
