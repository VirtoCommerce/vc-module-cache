using System;
using System.Security;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.CacheModule.Web.Model;
using VirtoCommerce.CacheModule.Web.Security;
using VirtoCommerce.CacheModule.Web.Services;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.CacheModule.Web.Controllers.Api
{
    [RoutePrefix("api/changes")]
    public class ChangesTrackingController : ApiController
    {
        private readonly IChangesTrackingService _changesTrackingService;

        public ChangesTrackingController(IChangesTrackingService changesTrackingService)
        {
            _changesTrackingService = changesTrackingService;
        }

        /// <summary>
        /// Force set changes last modified date 
        /// </summary>
        /// <param name="forceRequest">Force changes request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("force")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = CachePredefinedPermissions.ResetCache)]
        public IHttpActionResult ForceChanges(ForceChangesRequest forceRequest)
        {
            _changesTrackingService.Update(forceRequest?.Scope, DateTime.UtcNow);
            return Ok();
        }

        /// <summary>
        /// Get last modified date for given scope
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("lastmodifieddate")]
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
