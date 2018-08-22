using VirtoCommerce.CacheModule.Data.Extensions;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CacheModule.Data.Decorators
{
    public sealed class MemberServicesDecorator : ICachedServiceDecorator, IMemberService, IMemberSearchService
    {
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly CacheManagerAdaptor _cacheManager;
        public const string RegionName = "Member-Cache-Region";

        public MemberServicesDecorator(IMemberService memberService, IMemberSearchService memberSearchService, CacheManagerAdaptor cacheManager)
        {
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _cacheManager = cacheManager;
        }

        #region ICachedServiceDecorator
        public void ClearCache()
        {
            _cacheManager.ClearRegion(RegionName);
        }
        #endregion

        #region IMemberService
        public void SaveChanges(Member[] members)
        {
            _memberService.SaveChanges(members);
            ClearCache();
        }

        public void Delete(string[] ids, string[] memberTypes = null)
        {
            _memberService.Delete(ids, memberTypes);
            ClearCache();
        }

        public Member[] GetByIds(string[] memberIds, string responseGroup = null, string[] memberTypes = null)
        {
            var cacheKey = GetCacheKey("MemberService.GetByIds", string.Join(", ", memberIds), memberTypes.IsNullOrEmpty() ? "" : string.Join(", ", memberTypes));
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _memberService.GetByIds(memberIds, responseGroup, memberTypes));
            return retVal;
        }
        #endregion

        #region IMemberSearchService
        public GenericSearchResult<Member> SearchMembers(MembersSearchCriteria criteria)
        {
            var cacheKey = GetCacheKey("MemberSearchService.SearchMembers", criteria.GetCacheKey());
            var retVal = _cacheManager.Get(cacheKey, RegionName, () => _memberSearchService.SearchMembers(criteria));
            return retVal;
        }
        #endregion

        private static string GetCacheKey(params string[] parameters)
        {
            return "Member-" + string.Join(", ", parameters).GetHashCode();
        }
    }
}
