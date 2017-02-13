using System;
using System.Threading;

namespace VirtoCommerce.CacheModule.Web.Services
{
    public class ChangesTrackingService : IChangesTrackingService
    {
        private long _lastModifiedDateTicks = DateTime.UtcNow.Ticks;

        public DateTime GetLastModifiedDate(string scope)
        {
            var ticks = Interlocked.Read(ref _lastModifiedDateTicks);
            return new DateTime(ticks);
        }

        public void Update(string scope, DateTime changedDateTime)
        {
            var ticks = changedDateTime.Ticks;
            Interlocked.Exchange(ref _lastModifiedDateTicks, ticks);
        }
    }
}
