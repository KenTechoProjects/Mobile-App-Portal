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
using YellowStone.Models.DTO;
using YellowStone.Models.Enums;
using YellowStone.Repository.Interfaces;
using YellowStone.Services;
using YellowStone.Web.ViewModels;

namespace YellowStone.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly IAuditTrailLog auditTrailRepository;
        private readonly ILogger logger;
        UserViewModel userViewModel = new UserViewModel();
        private readonly IHttpContextAccessor _accessor;
        private readonly IAuthorizationService _authSvc;
        private readonly IUserService _userService;
        IFlasher _flash;

        string ipAddy = string.Empty;
        string remoteIp = string.Empty;
        string url = string.Empty;

        public UserController(IUserService userService, IOptions<SystemSettings> options, UserManager<User> userManager, ILogger<UserController> logger,
            IRoleRepository roleRepository, IAuditTrailLog auditTrailRepository, IUserRepository userRepository,
            IDepartmentRepository departmentRepository, IFlasher flash, IAuthorizationService authSvc, IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor) :
            base(userManager, options, auditTrailLog, accessor, userRepository)
        {
            this.userRepository = userRepository;
            this.departmentRepository = departmentRepository;
            this.roleRepository = roleRepository;
            this.auditTrailRepository = auditTrailRepository;
            _accessor = accessor;
            this.logger = logger;
            _flash = flash;
            _authSvc = authSvc;
            _userService = userService;
        }

        private async Task InitiateUser(string userId = "")
        {
            try
            {
                var user = await CurrentUser();

                userViewModel.PageBaseClass = new PageBaseClass();
                userViewModel.PageBaseClass.User = new User();
                userViewModel.PageBaseClass.User = user;
                userViewModel.PageBaseClass.Users = true;
                userViewModel.PageBaseClass.AdminPageName = "Users";

                userViewModel.User = new User();

                if (!string.IsNullOrEmpty(userId))
                {
                    userViewModel.User = await userRepository.Single(x => x.Id == userId);
                }

                userViewModel.Users = new List<User>();
                userViewModel.Users = userRepository.Users.ToList();

                userViewModel.DepartmentSummaries = new List<DepartmentSummary>();

                var departments = departmentRepository.Departments.Where(x=>x.Status == RequestStatus.Active).ToList();

                userViewModel.UserList = userRepository.Users.ToList(); ;

                foreach (var department in departments)
                {
                    DepartmentSummary departmentSummary = new DepartmentSummary
                    {
                        Id = department.Id,
                        Name = department.Name,
                        UserCount = userViewModel.Users.Where(x => x.Role.DepartmentId == department.Id && x.Status == UserStatus.Active).ToList().Count()
                    };
                    userViewModel.DepartmentSummaries.Add(departmentSummary);
                }

                ViewBag.Users = new List<User>();
                ViewBag.RoleResources = new List<Permission>();

                ViewBag.Departments = new List<Department>();
                ViewBag.Departments = departments.Where(x => x.Status == RequestStatus.Active).ToList();

                ViewBag.Roles = new List<Role>();
                ViewBag.Roles = roleRepository.Roles.Where(x => x.Status == RequestStatus.Active && !x.Name.ToLower().Contains("super")).ToList();


            }
            catch (Exception er) { logger.LogError("UserController : InitiateUser : " + er.ToString()); }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await CurrentUser();
            ViewBag.user = user;
            try
            {
                await InitiateUser();

            }
            catch (Exception er)
            {
                logger.LogError("UserController : Index : " + er.ToString());
                return RedirectToAction("Login", "Account");
            }

            //Audit log
            await AddAudit($"Accessed users page", user);

            return View(userViewModel);
        }

        public async Task<IActionResult> Create()
        {
            await InitiateUser();
            userViewModel.PageBaseClass.AdminPageName = "Create User";
            userViewModel.PageBaseClass.Users = true;
            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                var message = string.Empty;
                var currentUser = await CurrentUser();
                user.CreatedDate = DateTime.Now;
                if (!ModelState.IsValid)
                {
                    _flash.Flash("danger", $"Invalid input");
                    return RedirectToAction("Index");
                }
                if (currentUser != null)
                {
                    if (currentUser.IsAdmin)
                    {
                        user.Status = UserStatus.Active;
                    }
                    user.CreatedBy = currentUser.Name;
                    user.CreatedDate = DateTime.Now;
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    user.UserName = user.StaffId;

                    var createdUser = await userRepository.Create(user);
                    if (createdUser != null)
                    {

                        // await roleRepository.activateRole(user.RoleId);
                        var action = $"User Creation Request {createdUser.StaffId}";
                        var data = $" User: {JsonConvert.SerializeObject(user)}";
                        await AddAudit(action, currentUser, data);
                        if (user.Status == UserStatus.Pending_Approval)
                        {
                            message = $"Request logged for approval";
                        }
                        else
                        {
                            message = $"User {createdUser.StaffId.ToUpper()} was created successfully";
                        }
                        _flash.Flash("success", message);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _flash.Flash("danger", $"Failed to create user");
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception er)
            {
                logger.LogError("UserController : Create : " + er.ToString());
                _flash.Flash("danger", $"Something went wrong");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            var currentUser = await CurrentUser();
            var oldRole = string.Empty;
            var message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    User OldUser = userRepository.Users.Where(x => x.StaffId == user.StaffId && x.Status == user.Status).FirstOrDefault();
                    oldRole = OldUser.Role.Name;
                    var OldData = JsonConvert.SerializeObject(OldUser);
                    if (currentUser.IsAdmin)
                    {
                        OldUser.Status = UserStatus.Active;
                    }
                    else
                    {
                        OldUser.Status = UserStatus.Pending_Approval;
                    }
                    OldUser.RoleId = user.RoleId;
                    OldUser.SecurityStamp = Guid.NewGuid().ToString();

                    var updatedUser = await userRepository.Update(OldUser);

                    if (updatedUser != null)
                    {
                        var newrole = await roleRepository.GetByID(updatedUser.RoleId);
                        var action = $"Edit User Request {OldUser.StaffId} |Old Role: {oldRole} | New Role {newrole.Name}";
                        var data = $" OldData:  {OldData} | NewData : {JsonConvert.SerializeObject(OldUser)}";
                        await AddAudit(action, currentUser, data);
                        message = $"User {updatedUser.StaffId.ToUpper()} was updated";
                        if (user.Status == UserStatus.Pending_Approval)
                        {
                            message += ". Awaiting approval";
                        }
                        else
                        {
                            message += " successfully";
                        }
                        _flash.Flash("success", message);
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        _flash.Flash("danger", $"Something went wrong while saving");
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception er) { logger.LogError("UserController : Save Edit : " + er.ToString()); }

            ViewBag.user = currentUser;
            return RedirectToAction("Index");

        }
        public async Task<UserDetails> FetchStaffDetails(string SNNumber)
        {
           

            var result = new UserDetails(false);
            try
            {
                var userAlreadyExists = await userRepository.Any(x => x.StaffId == SNNumber && (x.Status != UserStatus.Rejected));

                if (userAlreadyExists)
                {
                    result.Message = "User Already Exists";
                    return result;
                }
                result = await _userService.Validate(SNNumber);



            }
            catch (Exception er) { logger.LogError("UserController : FetchStaffDetails : " + er.ToString()); }

            return result;
        }

        public IActionResult Edit(string Id, string status)
        {
            IEnumerable<User> user = new List<User>();
            try
            {
                user = userRepository.FindWhere(x => x.StaffId == Id && x.Status.ToString() == status).ToList();
                ViewBag.Roles = roleRepository.Roles.Where(x => x.Status == RequestStatus.Active && !x.Name.ToLower().Contains("super")).ToList();
            }
            catch (Exception er) { logger.LogError("UserController : " + er.ToString()); }

            return PartialView("_Edit", user.FirstOrDefault());
        }

        public async Task<IActionResult> UpdateStatus(string userId, string actionType, string status)
        {
            var currentUser = await CurrentUser();
            var action = string.Empty;
            var message = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(userId))
                {
                    var user = userRepository.Find(x => x.StaffId == userId && x.Status.ToString() == status).FirstOrDefault();
                    var OldData = JsonConvert.SerializeObject(user);
                    if (actionType == ActionTypes.Unlock.ToString())
                    {
                        if (currentUser.IsAdmin)
                        {
                            user.Status = UserStatus.Active;
                            action = $"Unlocked user {user.StaffId}";
                            message = $"The user {user.StaffId} was unlocked successfully";


                        }
                        else
                        {
                            user.Status = UserStatus.Pending_Unlock;
                            action = $"Unlocked User Request {user.StaffId}";
                            message = $"The request to unlock user {user.StaffId} was successful. Awaiting Approval.";


                        }
                    }
                    else if (actionType == ActionTypes.Deactivate.ToString())
                    {
                        if (currentUser.IsAdmin)
                        {
                            user.Status = UserStatus.Inactive;
                            action = $"Deactivated user {user.StaffId}";
                            message = $"The user {user.StaffId} was deactivated successfully";

                        }
                        else
                        {
                            user.Status = UserStatus.Pending_Deactivation;
                            action = $"Deactivated User Request {user.StaffId}";
                            message = $"The request to deactivate {user.StaffId} was successful. Awaiting Approval";

                        }

                    }
                    else if (actionType == ActionTypes.Activate.ToString())
                    {
                        if (currentUser.IsAdmin)
                        {
                            user.Status = UserStatus.Active;
                            action = $"Activated user {user.StaffId}";
                            message = $"The user {user.StaffId} was activated successfully";
                        }
                        else
                        {
                            user.Status = UserStatus.Pending_Approval; //Consider consolidating this
                            action = $"User {user.StaffId} was activated. Awaiting approval";
                            message = $"The request to activate {user.StaffId} was successful. Awaiting Approval";


                        }

                    }
                    else if (actionType == ActionTypes.Approve.ToString())
                    {

                        user.Status = UserStatus.Active;
                        action = $"Approved user {user.StaffId}";
                        message = $"The user {user.StaffId} was approved successfully";

                    }
                    else if (actionType == ActionTypes.Reject.ToString())
                    {

                        user.Status = UserStatus.Rejected;
                        action = $"Rejected user update for {user.StaffId}";
                        message = $"The user {user.StaffId} status update was rejected";

                    }

                    user.UpdatedBy = currentUser.Name;
                    user.UpdatedDate = DateTime.Now;
                    await userRepository.Update(user);
                    var data = $" OldData:  {OldData} | NewData : {JsonConvert.SerializeObject(user)}";
                    await AddAudit(action, currentUser, data);
                    _flash.Flash("success", message);

                }
            }
            catch (Exception er)
            {
                logger.LogError("UserController : RemoveUser : " + er.ToString());
                _flash.Flash("error", $"Failed to update status");

            }
            return RedirectToAction("Index");
        }

    }
}