using System;
using System.Linq;
using CacheManager.Core;
using CacheManager.Core.Internal;
using Common.Logging;
using Microsoft.Practices.Unity;
using VirtoCommerce.CacheModule.Data.Decorators;
using VirtoCommerce.CacheModule.Data.Extensions;
using VirtoCommerce.CacheModule.Data.Handlers;
using VirtoCommerce.CacheModule.Data.Repositories;
using VirtoCommerce.CacheModule.Data.Services;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Domain.Inventory.Services;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.Domain.Pricing.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security.Events;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.CacheModule.Web
{
    public class Module : ModuleBase
    {
        private const string _connectionStringName = "VirtoCommerce";
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members  

        public override void SetupDatabase()
        {
            using (var db = new CacheRepositoryImpl(_connectionStringName))
            {
                var initializer = new SetupDatabaseInitializer<CacheRepositoryImpl, Data.Migrations.Configuration>();
                initializer.InitializeDatabase(db);
            }
        }


        public override void PostInitialize()
        {
            var cacheManagerAdaptor = _container.Resolve<CacheManagerAdaptor>();

            var storeServiceDecorator = CreateStoreServicesDecorator(cacheManagerAdaptor);

            var catalogServicesDecorator = CreateCatalogServicesDecorator(cacheManagerAdaptor);

            if (storeServiceDecorator != null && catalogServicesDecorator != null)
                CreateCommerceServiceDecorator(storeServiceDecorator, catalogServicesDecorator);

            RegisterMemberServicesDecorators(cacheManagerAdaptor);

            RegisterMarketingServicesDecorators(cacheManagerAdaptor);

            RegisterInventoryServicesDecorators(cacheManagerAdaptor);

            RegisterPricingServicesDecorators(cacheManagerAdaptor);

            RegisterChangesTrackingService();

            var eventHandlerRegistrar = _container.Resolve<IHandlerRegistrar>();
            //Clear Members cache region in response to a security account changes 
            eventHandlerRegistrar.RegisterHandler<UserChangedEvent>(async (message, token) => await _container.Resolve<ResetMembersCacheSecurityEventHandler>().Handle(message));
        }

        private void RegisterChangesTrackingService()
        {
            Func<ICacheRepository> repositoryFactory = () => new CacheRepositoryImpl(_connectionStringName, new EntityPrimaryKeyGeneratorInterceptor());
            var changeTrackingService = new ChangesTrackingService(repositoryFactory);
            _container.RegisterInstance<IChangesTrackingService>(changeTrackingService);
            var cacheManager = _container.Resolve<ICacheManager<object>>();

            var observedRegions = new[] {
                StoreServicesDecorator.RegionName,
                CatalogServicesDecorator.RegionName,
                MemberServicesDecorator.RegionName,
                MarketingServicesDecorator.RegionName,
                InventoryServicesDecorator.RegionName,
                PricingServicesDecorator.RegionName
            };

            var logger = _container.Resolve<ILog>();

            //Need observe cache events to correct update latest changes timestamp when platform running on multiple instances
            //Cache clean event will be raising  thanks to Redis cache synced invalidation
            EventHandler<CacheClearEventArgs> onClearHandler = (sender, args) =>
            {
                try
                {
                    changeTrackingService.Update(null, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            };

            EventHandler<CacheClearRegionEventArgs> onClearRegionHandler = (sender, args) =>
            {
                if (args.Region != null && observedRegions.Any(x => x.EqualsInvariant(args.Region)))
                {
                    try
                    {
                        changeTrackingService.Update(null, DateTime.UtcNow);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            };
            //Throttling is required to prevent frequent database updates operations. 10 seconds is set as minimum interval to prevent flooding of database when multiple
            //platform instances is running
            cacheManager.OnClearRegion += onClearRegionHandler.Throttle(TimeSpan.FromSeconds(10));
            cacheManager.OnClear += onClearHandler.Throttle(TimeSpan.FromSeconds(10));
        }

        private void RegisterInventoryServicesDecorators(CacheManagerAdaptor cacheManagerAdaptor)
        {
            if (_container.IsRegistered<IInventoryService>())
            {
                var inventoryServicesDecorator = new InventoryServicesDecorator(_container.Resolve<IInventoryService>(), _container.Resolve<IInventorySearchService>(), cacheManagerAdaptor);
                _container.RegisterInstance<IInventoryService>(inventoryServicesDecorator);
                _container.RegisterInstance<IInventorySearchService>(inventoryServicesDecorator);
            }
        }

        private void RegisterMarketingServicesDecorators(CacheManagerAdaptor cacheManagerAdaptor)
        {
            if (_container.IsRegistered<IDynamicContentService>() &&
                _container.IsRegistered<IPromotionSearchService>() &&
                _container.IsRegistered<IPromotionService>() &&
                _container.IsRegistered<ICouponService>())
            {
                var marketingServicesDecorator = new MarketingServicesDecorator(cacheManagerAdaptor, _container.Resolve<IDynamicContentService>(), _container.Resolve<IPromotionSearchService>(), _container.Resolve<IPromotionService>(), _container.Resolve<ICouponService>());
                _container.RegisterInstance<IDynamicContentService>(marketingServicesDecorator);
                _container.RegisterInstance<IPromotionSearchService>(marketingServicesDecorator);
                _container.RegisterInstance<IPromotionService>(marketingServicesDecorator);
                _container.RegisterInstance<ICouponService>(marketingServicesDecorator);
            }
        }

        private void CreateCommerceServiceDecorator(StoreServicesDecorator storeServiceDecorator, CatalogServicesDecorator catalogServicesDecorator)
        {
            if (_container.IsRegistered<ICommerceService>())
            {
                var commerceServicesDecorator = new CommerceServiceDecorator(_container.Resolve<ICommerceService>(), new ICachedServiceDecorator[] { catalogServicesDecorator, storeServiceDecorator });
                _container.RegisterInstance<ICommerceService>(commerceServicesDecorator);
            }
        }

        private void RegisterMemberServicesDecorators(CacheManagerAdaptor cacheManagerAdaptor)
        {
            if (_container.IsRegistered<IMemberService>() &&
                _container.IsRegistered<IMemberSearchService>())
            {
                var memberServicesDecorator = new MemberServicesDecorator(_container.Resolve<IMemberService>(), _container.Resolve<IMemberSearchService>(), cacheManagerAdaptor);
                _container.RegisterInstance<IMemberService>(memberServicesDecorator);
                _container.RegisterInstance<IMemberSearchService>(memberServicesDecorator);
            }
        }

        private CatalogServicesDecorator CreateCatalogServicesDecorator(CacheManagerAdaptor cacheManagerAdaptor)
        {
            if (_container.IsRegistered<IItemService>() &&
                _container.IsRegistered<ICatalogSearchService>() &&
                _container.IsRegistered<IPropertyService>() &&
                _container.IsRegistered<ICategoryService>() &&
                _container.IsRegistered<ICatalogService>())
            {
                var catalogServicesDecorator = new CatalogServicesDecorator(_container.Resolve<IItemService>(), _container.Resolve<ICatalogSearchService>(), _container.Resolve<IPropertyService>(), _container.Resolve<ICategoryService>(), _container.Resolve<ICatalogService>(), cacheManagerAdaptor);
                _container.RegisterInstance<IItemService>(catalogServicesDecorator);
                _container.RegisterInstance<ICatalogSearchService>(catalogServicesDecorator);
                _container.RegisterInstance<IPropertyService>(catalogServicesDecorator);
                _container.RegisterInstance<ICategoryService>(catalogServicesDecorator);
                _container.RegisterInstance<ICatalogService>(catalogServicesDecorator);
                return catalogServicesDecorator;
            }

            return null;
        }

        private StoreServicesDecorator CreateStoreServicesDecorator(CacheManagerAdaptor cacheManagerAdaptor)
        {
            if (_container.IsRegistered<IStoreService>())
            {
                var storeServiceDecorator = new StoreServicesDecorator(_container.Resolve<IStoreService>(), cacheManagerAdaptor);
                _container.RegisterInstance<IStoreService>(storeServiceDecorator);
                return storeServiceDecorator;
            }
            return null;
        }

        private void RegisterPricingServicesDecorators(CacheManagerAdaptor cacheManagerAdaptor)
        {
            if (_container.IsRegistered<IPricingService>())
            {
                var pricingServiceDecorator = new PricingServicesDecorator(_container.Resolve<IPricingService>(), cacheManagerAdaptor);
                _container.RegisterInstance<IPricingService>(pricingServiceDecorator);
            }
        }
        #endregion
    }
}
