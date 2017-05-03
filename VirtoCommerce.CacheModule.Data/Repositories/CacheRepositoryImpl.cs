using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CacheModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.CacheModule.Data.Repositories
{
    public class CacheRepositoryImpl : EFRepositoryBase, ICacheRepository
    {
        public CacheRepositoryImpl()
        {
        }

        public CacheRepositoryImpl(string nameOrConnectionString, params IInterceptor[] interceptors)
			: base(nameOrConnectionString, null, interceptors)
		{
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region LastModified
            modelBuilder.Entity<LastModifiedEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<LastModifiedEntity>().ToTable("LastModified");
            #endregion


            base.OnModelCreating(modelBuilder);
        }

        #region IStoreRepository Members


        public IQueryable<LastModifiedEntity> LastModifiedEntities
        {
            get { return GetAsQueryable<LastModifiedEntity>(); }
        }
     
        #endregion

        public LastModifiedEntity GetLastModifiedForScope(string scope)
        {
            return LastModifiedEntities.FirstOrDefault(x => x.Scope == scope);
        }
    }
}
