using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.CacheModule.Web.Extensions;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CacheModule.Web.Decorators
{
    internal sealed class MemberServicesDecorator : IMemberService, IMemberSearchService
    {
        private IMemberService _memberService;
        private IMemberSearchService _memberSearchService;
        private CacheManagerAdaptor _cacheManager;
        private const string _regionName = "Member-Cache-Region";

        public MemberServicesDecorator(IMemberService memberService, IMemberSearchService memberSearchService, CacheManagerAdaptor cacheManager)
        {
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _cacheManager = cacheManager;
        }

        #region IMemberService
        public void CreateOrUpdate(Member[] members)
        {
            _memberService.CreateOrUpdate(members);
            _cacheManager.ClearRegion(_regionName);
        }

        public void Delete(string[] ids, string[] memberTypes = null)
        {
            _memberService.Delete(ids, memberTypes);
            _cacheManager.ClearRegion(_regionName);
        }

        public Member[] GetByIds(string[] memberIds, string[] memberTypes = null)
        {
            var cacheKey = GetCacheKey("MemberService.GetByIds", string.Join(", ", memberIds), memberTypes.IsNullOrEmpty() ? "" : string.Join(", ", memberTypes));
            var retVal = _cacheManager.Get(cacheKey, _regionName, () => {
                return _memberService.GetByIds(memberIds, memberTypes);
            });
            return retVal;
        }
        #endregion

        #region IMemberSearchService
        public MembersSearchResult SearchMembers(MembersSearchCriteria criteria)
        {
            var cacheKey = GetCacheKey("MemberSearchService.SearchMembers", criteria.ToJson().GetHashCode().ToString());
            var retVal = _cacheManager.Get(cacheKey, _regionName, () => {
                return _memberSearchService.SearchMembers(criteria);
            });
            return retVal;
        }
        #endregion

        private static string GetCacheKey(params string[] parameters)
        {
            return string.Format("Member-{0}", string.Join(", ", parameters).GetHashCode());
        }
    }
}