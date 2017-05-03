using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using CacheManager.Core;
using VirtoCommerce.CacheModule.Data.Services;
using VirtoCommerce.CacheModule.Web.Model;
using VirtoCommerce.CacheModule.Web.Security;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.CacheModule.Web.Controllers.Api
{
    [RoutePrefix("api")]
    public class CacheModuleController : ApiController
    {
        private readonly ICacheManager<object> _cacheManager;
        private readonly IChangesTrackingService _changesTrackingService;

        public CacheModuleController(IChangesTrackingService changesTrackingService, ICacheManager<object> cacheManager)
        {
            _changesTrackingService = changesTrackingService;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Return current platform cache usage stats 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("cache/stats")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetCacheStats()
        {
            var retVal = new List<CacheHandleStat>();
            foreach (var handle in _cacheManager.CacheHandles)
            {
                var stat = new CacheHandleStat
                {
                    Name = handle.Configuration.Name,
                    ItemsCount = handle.Count,
                    Hits = handle.Stats.GetStatistic(CacheManager.Core.Internal.CacheStatsCounterType.Hits),
                    Misses = handle.Stats.GetStatistic(CacheManager.Core.Internal.CacheStatsCounterType.Misses),
                };
                retVal.Add(stat);
            }
            return Ok(retVal.ToArray());
        }

        /// <summary>
        /// Reset current platform cache
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("cache/reset")]
        [ResponseType(typeof(string))]
        [CheckPermission(Permission = CachePredefinedPermissions.ResetCache)]
        public IHttpActionResult CacheReset()
        {
            _cacheManager.Clear();       
            return Ok();
        }

        /// <summary>
        /// Force set changes last modified date 
        /// </summary>
        /// <param name="forceRequest">Force changes request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("changes/force")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = CachePredefinedPermissions.ResetCache)]
        public IHttpActionResult ForceChanges(ForceChangesRequest forceRequest)
        {
            _changesTrackingService.Update(forceRequest?.Scope, DateTime.UtcNow);
            return Ok();
        }

        /// <summary>
        /// Get last modified date for given scope
        /// Used for signal of what something changed and for cache invalidation in external platform clients
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("changes/lastmodifieddate")]
        [ResponseType(typeof(LastModifiedResponse))]
        public IHttpActionResult GetLastModifiedDate(string scope = null)
        {
            var result = new LastModifiedResponse
            {
                Scope = scope,
                LastModifiedDate = _changesTrackingService.GetLastModifiedDate(scope),
            };

            return Ok(result);
        }
    }
}
