using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Models.Enums;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Authorization
{
    public class OnboardingInitiatorAuthorizationHandler
                    : AuthorizationHandler<InitiateOnboardingRequirement, User>
    {

        private readonly IRolePermissionRepository rolepermissionRepository;
        private readonly IPermissionRepository permissionRepository;
        internal readonly UserManager<User> _userManager;

        public OnboardingInitiatorAuthorizationHandler(IPermissionRepository permissionRepository, IRolePermissionRepository rolepermissionRepository, UserManager<User> _userManager)
        {
            this.rolepermissionRepository = rolepermissionRepository ?? throw new ArgumentNullException(nameof(rolepermissionRepository));
            this._userManager = _userManager;
            this.permissionRepository = permissionRepository;
        }
        protected override async Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                   InitiateOnboardingRequirement requirement,
                                     User user)
        {
            if (context.User == null)
            {
                return;
            }

            await Task.Run(() =>
             {
                 var name = "InitiateOnboarding";
                 var perm = permissionRepository.FindWhere(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();
                 bool hasPermission = rolepermissionRepository.CheckPermissionForUser(user, perm.Id);
                 if (hasPermission)
                 {
                     context.Succeed(requirement);
                 }

                 return;
             });

        }
    }
    public class InitiateOnboardingRequirement : IAuthorizationRequirement { }
}
