using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Repository;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Extensions
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IRolePermissionRepository rolepermissionRepository;
        internal readonly UserManager<User> _userManager;

        public PermissionHandler(IRolePermissionRepository rolepermissionRepository,  UserManager<User> _userManager)
        {
            if (rolepermissionRepository == null)
                throw new ArgumentNullException(nameof(rolepermissionRepository));

            this.rolepermissionRepository = rolepermissionRepository;
            this._userManager = _userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                // no user authorizedd. Alternatively call context.Fail() to ensure a failure 
                // as another handler for this requirement may succeed
                context.Fail();

            }

            
           // var user = await _userManager.FindByNameAsync(context.User.Identity.Name);
//bool hasPermission = await rolepermissionRepository.CheckPermissionForUser(user, requirement.Permission);
            //if (hasPermission)
            //{
            //    context.Succeed(requirement);
            //}
        }
        

        
    }
}
