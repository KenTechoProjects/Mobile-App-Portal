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
    public class AccountLinkingInitiatorAuthorizationHandler
                    : AuthorizationHandler<AccountLinkingInitiatorRequirement, User>
    {

        private readonly IRolePermissionRepository rolepermissionRepository;
        private readonly IPermissionRepository permissionRepository;
        internal readonly UserManager<User> _userManager;

        public AccountLinkingInitiatorAuthorizationHandler(IPermissionRepository permissionRepository,IRolePermissionRepository rolepermissionRepository, UserManager<User> _userManager)
        {
            this.rolepermissionRepository = rolepermissionRepository ?? throw new ArgumentNullException(nameof(rolepermissionRepository));
            this._userManager = _userManager;
            this.permissionRepository = permissionRepository;
        }
        protected override  Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                   AccountLinkingInitiatorRequirement requirement,
                                     User resource)
        {
            if (context.User == null)
            {
                return  Task.CompletedTask;
            }

            // Administrators can do anything.
            var name = "InitiateAccountLinking";
            var perm = permissionRepository.FindWhere(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();
            bool hasPermission = rolepermissionRepository.CheckPermissionForUser(resource, perm.Id);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return  Task.CompletedTask;
        }
    }
    public class AccountLinkingInitiatorRequirement : IAuthorizationRequirement { }
}
