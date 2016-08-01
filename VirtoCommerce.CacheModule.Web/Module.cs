using System;
using CacheManager.Core;
using Common.Logging;
using Microsoft.Practices.Unity;
using VirtoCommerce.CacheModule.Web.Decorators;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Domain.Store.Services;
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

            var commerceServicesDecorator = new CommerceServiceDecorator(_container.Resolve<ICommerceService>(), cacheManagerAdaptor, new ICachedServiceDecorator[] { catalogServicesDecorator, storeServiceDecorator });
            _container.RegisterInstance<ICommerceService>(commerceServicesDecorator);
        }
        #endregion
    }
}