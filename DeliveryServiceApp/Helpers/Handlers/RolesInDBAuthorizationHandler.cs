using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Helpers.Handlers
{
    public class RolesInDBAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public RolesInDBAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       RolesAuthorizationRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var found = false;
            if (requirement.AllowedRoles == null || !requirement.AllowedRoles.Any())
            {
                found = true;
            }
            else
            {
                var rolesAllow = requirement.AllowedRoles;
                var rolesList = await _roleRepository.GetAsync();
                var roleIds = rolesList.Where(p => rolesAllow.Contains(p.Name)).Select(p => p.Name).ToList();

                var users = await _userRepository.GetAsync();
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

                found = users.Any(p => p.Id.ToString().Equals(userId) && roleIds.Contains(p.RoleName));
            }

            if (found)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
