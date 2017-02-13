using System;

namespace VirtoCommerce.CacheModule.Web.Services
{
    public interface IChangesTrackingService
    {
        DateTime GetLastModifiedDate(string scope);
        void Update(string scope, DateTime changedDateTime);
    }
}
