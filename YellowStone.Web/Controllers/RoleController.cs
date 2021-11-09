using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Flash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YellowStone.Models;
using YellowStone.Models.Enums;
using YellowStone.Repository.Interfaces;
using YellowStone.Services;
using YellowStone.Web.ViewModels;

namespace YellowStone.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        private readonly IRoleRepository roleRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IRolePermissionRepository rolePermissionRepository;
        private readonly IUserRepository userRepository;
        private readonly IAuditTrailLog auditTrailRepository;
        private readonly ILogger _logger;

        private readonly IAuthorizationService _authSvc;
        IFlasher _flash;
        RoleViewModel roleViewModel = new RoleViewModel();

        string ipAddy = string.Empty;
        string remoteIp = string.Empty;
        string url = string.Empty;

        public RoleController(IAuthorizationService _authSvc, IOptions<SystemSettings> options, UserManager<User> userManager, ILogger<RoleController> logger, IRoleRepository roleRepository,
            IPermissionRepository permissionRepository, IRolePermissionRepository rolePermissionRepository,
            IUserRepository userRepository, IFlasher flash, IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor, IDepartmentRepository departmentRepository) :
            base(userManager, options, auditTrailLog, accessor, userRepository)
        {
            this.permissionRepository = permissionRepository;
            this.rolePermissionRepository = rolePermissionRepository;
            this.rolePermissionRepository = rolePermissionRepository;
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
            this._logger = logger;
            this._authSvc = _authSvc;
            _flash = flash;
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long roleId = 0)
        {

            var user = await CurrentUser();
            ViewBag.user = user;
            try
            {
                await InitiateRole(roleId);

                GetList();

            }
            catch (Exception er)
            {
                _logger.LogError("RoleController : Index : " + er.ToString());
                return RedirectToAction("Login", "Account");
            }
            ViewBag.user = user;
            //Audit log
            var action = $"Accessed Roles page";
            await AddAudit(action, user);
            return View(roleViewModel);

        }


        private User GetUser()
        {
            return JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
        }

        private async Task InitiateRole(long roleId)
        {
            try

            {
                var user = await CurrentUser();
                roleViewModel.PageBaseClass = new PageBaseClass();
                roleViewModel.PageBaseClass.User = new User();
                roleViewModel.PageBaseClass.User = user;
                roleViewModel.PageBaseClass.Roles = true;
                roleViewModel.PageBaseClass.AdminPageName = "Roles";
                roleViewModel.Roles = roleRepository.Roles.ToList();
                roleViewModel.SaveRole = new SaveRoleViewModel
                {
                    PermissionsList = permissionRepository.Permissions.Select(s => new PermissionViewList
                    {
                        Id = s.Id,
                        Name = s.Name,
                        IsChecked = false,
                        Category = s.PermissionCategory

                    }).ToList(),

                    DepartmentList = _departmentRepository.Departments.Where(x => new RequestStatus[] { RequestStatus.Active }.Contains(x.Status)).ToList()

                };

                if (roleId != 0)
                {
                    var role = roleRepository.Roles.FirstOrDefault(x => x.Id == roleId);
                    roleViewModel.Role = role;
                    roleViewModel.SaveRole.role = role;
                    roleViewModel.SaveRole.DepartmentId = roleViewModel.Role.DepartmentId.GetValueOrDefault();

                    var rolePermissions = role.RolePermissions;

                    List<Permission> rList = new List<Permission>();
                    foreach (var r in rolePermissions)
                    {

                        foreach (var perm in roleViewModel.SaveRole.PermissionsList)
                        {
                            if (perm.Id == r.PermissionId)
                            {
                                perm.IsChecked = true;
                                roleViewModel.SaveRole.RoleType = perm.Category;
                            }
                        }
                    }

                }
                else
                {
                    roleViewModel.SaveRole.role = new Role();
                }
            }
            catch (Exception er) { _logger.LogError("RoleController : InitiateRole : " + er.ToString()); }
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveRoleViewModel roleModel)
        {
            List<Permission> resourceList = new List<Permission>();
            var user = await CurrentUser();
            List<RolePermission> oldPermissions = null;
            Role role = roleModel.role;
            role.CreatedDate = DateTime.Now;
            role.DepartmentId = roleModel.DepartmentId;
            var msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    var isUpdate = role.Id != 0;
                    var roleExists = roleRepository.Roles.Any(x => x.Name.ToLower().Contains(role.Name.ToLower()));
                    if (roleExists)
                    {
                        var oldRole = roleRepository.Roles.FirstOrDefault(x => x.Id == role.Id);
                        if (role.Name != oldRole.Name)
                        {
                            _flash.Flash("danger", string.Format("Duplicate Roles are not allowed. Kindly provide another role name."));
                            return RedirectToAction("Index");
                        }
                    }

                    if (isUpdate)
                    {
                        var rolePermissions = roleRepository.Roles.FirstOrDefault(x => x.Id == role.Id).RolePermissions.Select(x => x.PermissionId);
                        oldPermissions = rolePermissionRepository.RolePermission.Where(x => rolePermissions.Contains(x.PermissionId)).ToList();
                    }

                    role.CreatedBy = user.Name;

                    List<Permission> permissions = new List<Permission>();

                    if (roleModel.Selected == null)
                    {
                        _flash.Flash("danger", "Please select permissions");
                        return RedirectToAction("Index");
                    }
                    foreach (long per in roleModel.Selected)
                    {
                        var permission = await permissionRepository.Single(z => z.Id == per);
                        permissions.Add(permission);

                    }
                    var action = isUpdate ? "Updated" : "Created";

                    if (user.IsAdmin)
                    {
                        msg = $"Role {action} Successfully";
                        role.Status = RequestStatus.Active;
                    }
                    else
                    {
                        msg = $"Role {action} Successfully. Awaiting Approval";
                        role.Status = RequestStatus.Pending;

                    }


                    if (await rolePermissionRepository.SaveRolePermission(role, permissions))
                    {
                        //Audit log
                      
                        var serializedPermissions = JsonConvert.SerializeObject(permissions.Select(x => x.Name));
                        var actionDescription = $"{action} {role.Name} Role| Permissions : {serializedPermissions}";
                        if (isUpdate)
                        {
                            actionDescription = $"{actionDescription} | Old Permissions : {JsonConvert.SerializeObject(oldPermissions.Select(x => x.Permission.Name).Distinct())}";
                        }
                        await AddAudit(actionDescription, user);

                        _flash.Flash("success", msg);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _flash.Flash("danger", "Something went wrong trying to create/update role");
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    _flash.Flash("danger", string.Format("Invalid input"));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception er)
            {
                _logger.LogError($"Error Occurred. RoleController : Create : {er}");
                _flash.Flash("danger", string.Format("Something went wrong"));
                return RedirectToAction("Index");
            }


        }

        private async Task<List<Permission>> FetchRolePermissions(long roleId)
        {
            List<Permission> permissionList = new List<Permission>();
            List<RolePermission> rolePermissionList = new List<RolePermission>();

            try
            {
                rolePermissionList = rolePermissionRepository.FindWhere(x => x.RoleId == roleId).ToList();

                foreach (var rolePermission in rolePermissionList)
                {
                    var permission = await permissionRepository.Single(x => x.Id == rolePermission.PermissionId);
                    permissionList.Add(permission);

                }
            }
            catch (Exception e)
            {
                _logger.LogError("FetchRolePermissions : " + e.ToString());

            }

            return permissionList;

        }


        public async Task<IActionResult> ViewDetails(long roleId)
        {
            List<Permission> permissionList = new List<Permission>();

            try
            {
                if (roleId > 0)
                {
                    var rolePermissions = rolePermissionRepository.FindWhere(z => z.RoleId == roleId).ToList();
                    permissionList = await FetchRolePermissions(roleId);
                }
            }
            catch (Exception er) { _logger.LogError("RoleController : ViewPermissions : " + er.ToString()); }

            return PartialView("_ViewPermissions", permissionList);
        }

        public async Task<IActionResult> UpdateRoleStatus(long roleId, string actionType)
        {
            var message = string.Empty;
            var user = await CurrentUser();

            try
            {
                if (roleId > 0 && actionType != null)
                {
                    var role = await roleRepository.Single(x => x.Id == roleId);
                    var OldData = JsonConvert.SerializeObject(role);
                    if (actionType == ActionTypes.Approve.ToString())
                    {
                        role.Status = RequestStatus.Active;
                        message = $"Approved {role.Name}";
                    }
                    else
                    {
                        role.Status = RequestStatus.Rejected;
                        message = $"Rejected {role.Name}";

                    }

                    //Audit log
                    var action = $"{message} {role.Name} role";
                    var data = $"OldData {OldData} | NewData : {JsonConvert.SerializeObject(role)}";
                    await AddAudit(action, user, data);
                    _flash.Flash("success", "Role Status Updated Successfully");
                    await roleRepository.Update(role);
                }
            }
            catch (Exception er)
            {
                _logger.LogError("RoleController : UpdateRoleStatus : " + er.ToString());
            }
            return RedirectToAction("Index");
        }

        private void GetList()
        {
            var roles = roleRepository.Roles.Where(s => s.Id > 0).ToList();
            roleViewModel.RoleList = roles;
        }
    }
}