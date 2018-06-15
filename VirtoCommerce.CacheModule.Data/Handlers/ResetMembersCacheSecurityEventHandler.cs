using System.Threading.Tasks;
using VirtoCommerce.CacheModule.Data.Decorators;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security.Events;

namespace VirtoCommerce.CacheModule.Data.Handlers
{
    /// <summary>
    /// Clear Members cache region in response to a security account changes 
    /// </summary>
    public class ResetMembersCacheSecurityEventHandler : IEventHandler<UserChangedEvent>
    {
        private readonly CacheManagerAdaptor _cacheManager;
        public ResetMembersCacheSecurityEventHandler(CacheManagerAdaptor cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public virtual Task Handle(UserChangedEvent message)
        {
            var changedUser = message.ChangedEntry.OldEntry;
            //We should clear the whole region because is impossible to selective invalidation due
            //that fact that member associated  with the changed user can be cached within various cache keys.
            if (changedUser != null && !string.IsNullOrEmpty(changedUser.MemberId))
            {
                _cacheManager.ClearRegion(MemberServicesDecorator.RegionName);
            }
            return Task.CompletedTask;
        }
    }
}

