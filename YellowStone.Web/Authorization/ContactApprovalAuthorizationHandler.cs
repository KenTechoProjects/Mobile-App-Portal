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
    public class ContactApprovalAuthorizationHandler
                    : AuthorizationHandler<ContactApprovalRequirement, User>
    {

        private readonly IRolePermissionRepository rolepermissionRepository;
        private readonly IPermissionRepository permissionRepository;
        //private readonly IUserRepository userRepository;
        internal readonly UserManager<User> _userManager;

        public ContactApprovalAuthorizationHandler(IPermissionRepository permissionRepository,IRolePermissionRepository rolepermissionRepository, UserManager<User> _userManager)
        {
            this.rolepermissionRepository = rolepermissionRepository ?? throw new ArgumentNullException(nameof(rolepermissionRepository));
            this._userManager = _userManager;
            this.permissionRepository = permissionRepository;
            //this.userRepository = userRepository;
        }
        protected override  Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                   ContactApprovalRequirement requirement,
                                     User resource)
        {
            if (context.User == null)
            {
                return  Task.CompletedTask;
            }

            // Administrators can do anything.
            var name = "ApprovalNotification";
           // var user =  userRepository.FindWhere(x => x.StaffId.ToLower().Contains(context.User.Identity.Name.ToLower())).FirstOrDefault();
            var perm = permissionRepository.FindWhere(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();
            bool hasPermission = rolepermissionRepository.CheckPermissionForUser(resource, perm.Id);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return  Task.CompletedTask;
        }
    }
    public class ContactApprovalRequirement : IAuthorizationRequirement { }
}
