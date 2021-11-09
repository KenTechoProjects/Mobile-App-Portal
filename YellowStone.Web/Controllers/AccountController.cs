using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
using YellowStone.Models.DTO;
using YellowStone.Services;
using YellowStone.Web.ViewModels;

namespace YellowStone.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {

        private readonly IUserRepository userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly IRoleRepository roleRepository;
        private readonly IRolePermissionRepository rolePermissionRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly ILogger logger;
        IFlasher _flash;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly SystemSettings _settings;


        public AccountController(IOptions<SystemSettings> settings, UserManager<User> userManager, SignInManager<User> signInManager,
                                ILogger<AccountController> logger
                                  , IUserRepository userRepository, IRoleRepository roleRepository,
                                IPermissionRepository permissionRepository, IDepartmentRepository departmentRepository,
                                 IRolePermissionRepository rolePermissionRepository,
                                IFlasher flash, IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor,
                                ITokenService tokenService, IUserService userService)
            : base(userManager, settings, auditTrailLog, accessor, userRepository)
        {

            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.permissionRepository = permissionRepository;
            this.departmentRepository = departmentRepository;
            this.rolePermissionRepository = rolePermissionRepository;
            _flash = flash;
            _signInManager = signInManager;
            this.logger = logger;
            _tokenService = tokenService;
            _userService = userService;
            _settings = settings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            HttpContext.Session.Remove("User");
            HttpContext.Session.Remove("Permissions");
            HttpContext.Session.Clear();
            return View();
        }



        [HttpPost]
       [ValidateAntiForgeryToken]

        public async Task<JsonResult> Authenticate(LoginDetails loginDetails)
        {
            var message = string.Empty;
            var result = new LoginResponse(false);
            User user = null;

            if (!ModelState.IsValid)
            {
                result.Message = "Invalid credentials";
                return Json(result);
            }

            string userRegex = @"^[a-zA-Z0-9]*$";
            Regex re = new Regex(userRegex);
            if (!re.IsMatch(loginDetails.StaffId))
            {
                result.Message = "Invalid credentials";
                await AddAudit($"Login : {result.Message}", loginDetails.StaffId);
                return Json(result);
            }

            try
            {

                var userName = loginDetails.StaffId;

                user = await userRepository.FindUserAsync(userName);

                if (user == null)
                {
                    result.Message = "User Unauthorized";
                    await AddAudit($"Login : {result.Message}", userName);
                    return Json(result);
                }



                if (user.Status == UserStatus.Locked)
                {
                    result.Message = "Account locked";
                    await AddAudit($"Login : {result.Message}", userName);
                    return Json(result);
                } 
                
                if (user.Status == UserStatus.Dormant)
                {
                    result.Message = "Account dormant";
                    await AddAudit($"Login : {result.Message}", userName);
                    return Json(result);
                }

                result = await _userService.Authenticate(loginDetails.StaffId, loginDetails.Password);

                if (result.IsSuccessful)
                {
                    var finacleDetails = await _userService.GetFinacleDetails(loginDetails.StaffId);
                    if (finacleDetails.IsSuccessful)
                    {
                        user.StaffBranchCode = finacleDetails.BranchCode;
                    }
                    //TODO: Get Teller til account
                    result.StaffId = loginDetails.StaffId;
                    user.AccessFailedCount = 0;
                    message = $"Login : Successful";

                }
                else
                {
                    user.AccessFailedCount++;
                    if (user.AccessFailedCount == _settings.MaxLoginCount)
                    {
                        result.Message = $"Account locked";
                        user.Status = UserStatus.Locked;
                    }
                    else
                    {
                        result.Message = $"{result.Message} : {_settings.MaxLoginCount - user.AccessFailedCount} attempt(s) left";

                    }

                    message = $"Login : {result.Message}";
                }
                await userRepository.Update(user);

            }
            catch (Exception er)
            {
                result.Message = er.Message;
                logger.LogError(string.Format("Error occured while {0} was trying to login : {1}. AccountController : Login : ", loginDetails.StaffId, er));
            }
            await AddAudit(message, user);

            return Json(result);
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<JsonResult> ValidateToken(TokenDetails tokenDetails)
        {
            var result = new TokenResponse(false);
            var action = string.Empty;
            User user = null;
            if (!ModelState.IsValid)
            {
                result.Message = "Invalid credentials";
                return Json(result);
            }
            try
            {

                var userName = tokenDetails.StaffId;
                user = await userRepository.GetUserByStaffId(userName);
                if (user == null)
                {
                    result.Message = "User Unauthorized";
                    await AddAudit($"Token Validation : {result.Message}", tokenDetails.StaffId);
                    return Json(result);
                }

                userName = user.UserName;
                HttpContext.Session.Remove("User");
                HttpContext.Session.Clear();

                result = await _tokenService.ValidateToken(tokenDetails.StaffId, tokenDetails.Token);
                logger.LogInformation($"FBNService user id : {tokenDetails.StaffId} token : {result.IsSuccessful.ToString()}");
             
                if (result.IsSuccessful)
                {
                    user.LastLoginDate = user.PreviousLoginDate;
                    user.PreviousLoginDate = DateTime.Now;

                    IdentityResult identityResult = await _userManager.UpdateSecurityStampAsync(user);

                    await userRepository.Update(user);
                    await _signInManager.SignInAsync(user, true);
                  //  await DisableUsersWithLastLoginDateGreaterThanNintyDay();
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));
                    var permissions = await FetchUserPermissions(user);
                    if (permissions != null && permissions.Count > 0)
                    {
                        HttpContext.Session.SetString("Permissions", JsonConvert.SerializeObject(permissions));
                    }
                    action = $"Token Validation : Successful";
                }
                else
                {
                    action = $"Token Validation : Failed";
                }

            }
            catch (Exception er)
            {
                logger.LogError(string.Format("Error occured while {0} was trying to login : {1}. AccountController : Login : ", tokenDetails.StaffId, er));
            }
            await AddAudit(action, user);

            return Json(result);
        }

        public async Task<List<Permission>> FetchUserPermissions(User user)
        {
            List<Permission> permissions = new List<Permission>();
            Permission permission = new Permission();
            Role role = roleRepository.FindWhere(x => x.Id == user.RoleId).FirstOrDefault();

            if (role != null)
            {
                var perms = rolePermissionRepository.FindWhere(x => x.RoleId == role.Id).ToList();
                foreach (var rolePerm in perms)
                {
                    permission = await permissionRepository.Single(x => x.Id == rolePerm.PermissionId);

                    if (permission != null && permission.Name != null)
                    {
                        permissions.Add(permission);
                    }
                }
            }
            return permissions;
        }

        public async Task<IActionResult> LogOut(LoginDetails loginDetails)
        {
            try
            {
                User currentUser = await CurrentUser();
                await _signInManager.SignOutAsync();
                HttpContext.Session.Remove("User");
                HttpContext.Session.Remove("Permissions");
                logger.LogInformation("User logged out.");

                //Audit log
                var action = $"logged out";
                await AddAudit(action, currentUser);

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            catch (Exception er) { logger.LogError(er.ToString()); }


            return RedirectToAction("Login");
        }

        public async Task DisableUsersWithLastLoginDateGreaterThanNintyDay()
        {
            try
            {
                List<User> users = new List<User>();
                users = userRepository.FindWhere(x => x.Status == UserStatus.Active && x.IsAdmin == false).ToList();
                foreach (var user in users)
                {
                    if (user.LastLoginDate.HasValue && user.LastLoginDate.Value.AddDays(_settings.InactiveDays) <= DateTime.Now)
                    {
                        user.Status = UserStatus.Dormant;
                        await userRepository.Update(user);
                    }
                }

            }
            catch (Exception er) { logger.LogError(er.ToString()); }
            // return Task.CompletedTask;
        }
    }
}