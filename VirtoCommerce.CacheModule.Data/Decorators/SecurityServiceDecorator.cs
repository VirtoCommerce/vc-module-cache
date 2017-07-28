using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CacheModule.Data.Decorators
{
    public sealed class SecurityServiceDecorator : ICachedServiceDecorator, ISecurityService
    {
        private readonly ISecurityService _securityService;
        private readonly MemberServicesDecorator _memberServicesDecorator;

        public SecurityServiceDecorator(ISecurityService securityService, MemberServicesDecorator memberServicesDecorator)
        {
            _securityService = securityService;
            _memberServicesDecorator = memberServicesDecorator;
        }
        
        #region Implementation of ICachedServiceDecorator

        public void ClearCache()
        {
            _memberServicesDecorator.ClearCache();
        }

        #endregion

        #region Implementation of ISecurityService

        public async Task<ApplicationUserExtended> FindByNameAsync(string userName, UserDetails detailsLevel)
        {
            return await _securityService.FindByNameAsync(userName, detailsLevel);
        }

        public async Task<ApplicationUserExtended> FindByIdAsync(string userId, UserDetails detailsLevel)
        {
            return await _securityService.FindByIdAsync(userId, detailsLevel);
        }

        public async Task<ApplicationUserExtended> FindByEmailAsync(string email, UserDetails detailsLevel)
        {
            return await _securityService.FindByEmailAsync(email, detailsLevel);
        }

        public async Task<ApplicationUserExtended> FindByLoginAsync(string loginProvider, string providerKey, UserDetails detailsLevel)
        {
            return await _securityService.FindByLoginAsync(loginProvider, providerKey, detailsLevel);
        }

        public async Task<SecurityResult> CreateAsync(ApplicationUserExtended user)
        {
            var retVal = await _securityService.CreateAsync(user);
            if (!string.IsNullOrEmpty(user.MemberId))
            {
                ClearCache();
            }
            return retVal;
        }

        public async Task<SecurityResult> UpdateAsync(ApplicationUserExtended user)
        {
            var retVal = await _securityService.UpdateAsync(user);
            if (!string.IsNullOrEmpty(user.MemberId))
            {
                ClearCache();
            }
            return retVal;
        }

        public async Task DeleteAsync(string[] names)
        {
            await _securityService.DeleteAsync(names);
            ClearCache();
        }

        public ApiAccount GenerateNewApiAccount(ApiAccountType type)
        {
            return _securityService.GenerateNewApiAccount(type);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string userId)
        {
            return await _securityService.GeneratePasswordResetTokenAsync(userId);
        }

        public async Task<SecurityResult> ChangePasswordAsync(string name, string oldPassword, string newPassword)
        {
            return await _securityService.ChangePasswordAsync(name, oldPassword, newPassword);
        }

        public async Task<SecurityResult> ResetPasswordAsync(string name, string newPassword)
        {
            return await _securityService.ResetPasswordAsync(name, newPassword);
        }

        public async Task<SecurityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            return await _securityService.ResetPasswordAsync(userId, token, newPassword);
        }

        public async Task<UserSearchResponse> SearchUsersAsync(UserSearchRequest request)
        {
            return await _securityService.SearchUsersAsync(request);
        }

        public bool UserHasAnyPermission(string userName, string[] scopes, params string[] permissionIds)
        {
            return _securityService.UserHasAnyPermission(userName, scopes, permissionIds);
        }

        public Permission[] GetAllPermissions()
        {
            return _securityService.GetAllPermissions();
        }

        public Permission[] GetUserPermissions(string userName)
        {
            return _securityService.GetUserPermissions(userName);
        }

        #endregion
    }
}
