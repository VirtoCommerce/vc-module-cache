using System;

namespace VirtoCommerce.CacheModule.Data.Services
{
    /// <summary>
    /// Used for signal of what something changed and for cache invalidation in external platform clients
    /// </summary>
    public interface IChangesTrackingService
    {
        DateTime GetLastModifiedDate(string scope);
        void Update(string scope, DateTime changedDateTime);
    }
}
