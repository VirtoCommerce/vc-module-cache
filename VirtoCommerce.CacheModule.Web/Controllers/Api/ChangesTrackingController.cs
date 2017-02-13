using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.CacheModule.Web.Model;
using VirtoCommerce.CacheModule.Web.Services;

namespace VirtoCommerce.CacheModule.Web.Controllers.Api
{
    public class ChangesTrackingController : ApiController
    {
        private readonly IChangesTrackingService _changesTrackingService;

        public ChangesTrackingController(IChangesTrackingService changesTrackingService)
        {
            _changesTrackingService = changesTrackingService;
        }

        /// <summary>
        /// Get last modified date for given scope
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/changes/lastmodifieddate")]
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
