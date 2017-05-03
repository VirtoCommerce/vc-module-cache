using System;
using System.Threading;
using VirtoCommerce.CacheModule.Data.Model;
using VirtoCommerce.CacheModule.Data.Repositories;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CacheModule.Data.Services
{
    public class ChangesTrackingService : ServiceBase, IChangesTrackingService
    {
        private readonly DateTime _now;
        private readonly Func<ICacheRepository> _repositoryFactory;
        public ChangesTrackingService(Func<ICacheRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _now = DateTime.UtcNow;
        }

        public DateTime GetLastModifiedDate(string scope)
        {
            var retVal = _now;
            using (var repository = _repositoryFactory())
            {
                var lastModified = repository.GetLastModifiedForScope(scope);
                if(lastModified != null)
                {
                    retVal = lastModified.LastModifiedDate;
                }
            }
            return retVal;
        }

        public void Update(string scope, DateTime changedDateTime)
        {
            using (var repository = _repositoryFactory())
            {
                var lastModified = repository.GetLastModifiedForScope(scope);
                if(lastModified == null)
                {
                    lastModified = new LastModifiedEntity
                    {
                        Scope = scope
                    };
                    repository.Add(lastModified);
                }
                lastModified.LastModifiedDate = changedDateTime;                

                CommitChanges(repository);
            }
        }
    }
}
