using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;
using YellowStone.Filters;
using YellowStone.Services;
using YellowStone.Web.ViewModels;

namespace YellowStone.Controllers
{
    [Authorize]
    public class AuditController : BaseController
    {

        private readonly IUserRepository _userRepository;
        private readonly ILogger logger;
        AuditLogViewModel auditLogViewModel = new AuditLogViewModel();
        private readonly IAuditTrailLog _auditTrailLog;

        public AuditController(IOptions<SystemSettings> options, UserManager<User> userManager,
            ILogger<AuditController> logger, IUserRepository userRepository,
            IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor) :
            base(userManager, options, auditTrailLog, accessor, userRepository)

        {
            _auditTrailLog = auditTrailLog;
            _userRepository = userRepository;
            this.logger = logger;
        }

        private async Task InitiatePage(User user)
        {
            try
            {
              //  var user = await CurrentUser();

                auditLogViewModel.PageBaseClass = new PageBaseClass();
                auditLogViewModel.PageBaseClass.User = new User();
                auditLogViewModel.PageBaseClass.User = user;
                auditLogViewModel.PageBaseClass.AuditLogs = true;
                auditLogViewModel.PageBaseClass.AdminPageName = "Audit Trail Log";
                auditLogViewModel.PageBaseClass.ViewAuditTrail = true;

                auditLogViewModel.AuditLog = new AuditLog();


            }
            catch (Exception er) { logger.LogError(er.ToString()); }
        }

        [HttpGet]
        public async Task<IActionResult> ClearSearch()
        {
            var user = await CurrentUser();

            await InitiatePage(user);
            ViewBag.user = user;
            auditLogViewModel.AuditLogList = new List<AuditLog>();
            return View("Index", auditLogViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Index(string currentFilter, string fromDate = "", string toDate = "", string Search = "")
        {
            var user = await CurrentUser();
            await InitiatePage(user);

            string action;
            if (!string.IsNullOrEmpty(currentFilter))
            {
                Search = currentFilter;
                action = $"Searched {currentFilter} in audit logs";
            }
            else
            {
                action = $"Viewed audit logs";
            }

            GetList(Search, fromDate, toDate);
            ViewBag.user = user;

            //Audit log
            await AddAudit(action, user);
            return View(auditLogViewModel);
        }

        private void GetList(string searchString, string fromDate, string toDate)
        {
           
            ViewData["CurrentFilter"] = searchString;
            ViewData["currentFromDate"] = fromDate;
            ViewData["currentToDate"] = toDate;
            List<AuditLog> audits = new List<AuditLog>();

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(searchString))
            {

                DateTime fromDateTime = Convert.ToDateTime(fromDate);
                DateTime toDateTime = Convert.ToDateTime(toDate);
                searchString = searchString.ToLower();
                toDateTime = toDateTime.AddDays(1);
                var result = _auditTrailLog.AuditLogs.Where(s => s.CreatedDate >= fromDateTime && s.CreatedDate <= toDateTime);

                audits = result.Where(x => x.StaffId.Contains(searchString) || x.UserBranch.Contains(searchString) || x.ActionPerformed.ToLower().Contains(searchString)
                      || x.AccountNumber.Contains(searchString) || x.IpAddress.Contains(searchString)).OrderByDescending(x => x.CreatedDate).ToList();

            }
            else if (!string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(searchString))
            {
                DateTime fromDateTime = Convert.ToDateTime(fromDate);
                DateTime toDateTime = Convert.ToDateTime(toDate);
                toDateTime = toDateTime.AddDays(1);
                audits = _auditTrailLog.AuditLogs.Where(s => s.CreatedDate >= fromDateTime && s.CreatedDate <= toDateTime).OrderByDescending(x => x.CreatedDate).ToList(); ;
            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();

                audits = _auditTrailLog.AuditLogs.Where(x => x.StaffId.Contains(searchString) || x.UserBranch.Contains(searchString) || x.ActionPerformed.ToLower().Contains(searchString)
                        || x.AccountNumber.Contains(searchString) || x.IpAddress.Contains(searchString)).OrderByDescending(x => x.CreatedDate).ToList();
            }
            else
            {
                audits = _auditTrailLog.AuditLogs.Where(x => x.Id > 0).OrderByDescending(x => x.CreatedDate).Take(100).ToList();
            }

            auditLogViewModel.AuditLogList = audits;
        }
    }
}