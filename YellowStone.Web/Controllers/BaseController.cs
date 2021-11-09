using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Flash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;
using YellowStone.Services;

namespace YellowStone.Controllers
{
    public class BaseController : Controller
    {
        internal readonly UserManager<User> _userManager;
        private readonly IOptions<SystemSettings> _options;
        private readonly IAuditTrailLog _auditTrailLog;
        private readonly IHttpContextAccessor _accessor;
        private readonly IUserRepository  _userRepository;
        private readonly List<Role> _roles;
        public BaseController(UserManager<User> userManager, IOptions<SystemSettings> options, IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor, IUserRepository userRepository)
        {
            _userManager = userManager;
            _options = options;
            _auditTrailLog = auditTrailLog;
            _accessor = accessor;
          
           _userRepository = userRepository;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public object SignedIn()
        {
            User user = new User();

            string userLoginString = HttpContext.Session.GetString("UserLogin");

            if (!string.IsNullOrEmpty(userLoginString))
            {
                user = JsonConvert.DeserializeObject<User>(userLoginString);
            }

            if (user == null || user.Id == "")
            {
                return RedirectToAction("Login", "Account");
            }

            return true;
        }

        internal async Task<User> CurrentUser()
        {
            var identity = User.Identity.Name;
            var user =  _userRepository.Users.FirstOrDefault(x=>x.UserName == identity);
          
            return user;
        }

        internal async Task AddAudit(string actionPerformed, string userId = null, string adDepartment = null, string extra = null, string accountNumber = null)
        {
           

            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
         //   string iPAddress = _accessor.HttpContext.Request.UserHostAddress;
           string iPAddress = _accessor.HttpContext.Connection.LocalIpAddress.ToString();
            var Audit = new AuditLog
            {
                StaffId = userId,
                IpAddress = iPAddress,
                ActionPerformed = actionPerformed,
                UrlAccessed = $"{controllerName}/{actionName}",
                CreatedDate = DateTime.Now,
                Extra = extra ?? "N/A",
                AccountNumber = accountNumber ?? "N/A",
                UserBranch = adDepartment
            };

            await _auditTrailLog.Create(Audit);
        }


        internal async Task AddAudit(string actionPerformed, User user = null, string extra = null, string accountNumber = null)
        {
            if (user == null)
            {
                user = await CurrentUser();
            }

            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            string iPAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var Audit = new AuditLog
            {
                StaffId = user?.StaffId,
                IpAddress = iPAddress,
                ActionPerformed = actionPerformed,
                UrlAccessed = $"{controllerName}/{actionName}",
                CreatedDate = DateTime.Now,
                Extra = extra ?? "N/A",
                AccountNumber = accountNumber ?? "N/A",
                UserBranch = user?.StaffBranchCode ?? user?.StaffAdDepartment
            };

            await _auditTrailLog.Create(Audit);
        }
    }
}