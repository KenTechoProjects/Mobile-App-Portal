using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Flash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;
using YellowStone.Services;
using YellowStone.Web.ViewModels;

namespace YellowStone.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IAuthorizationService _authSvc;
        IFlasher _flash;
        ILogger logger;

        public HomeController(IAuthorizationService _authSvc, IOptions<SystemSettings> options, UserManager<User> userManager,
           ILogger<HomeController> logger,
           IFlasher flash, IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor, IUserRepository userRepository) :
            base(userManager, options, auditTrailLog, accessor, userRepository)
        {

            this.logger = logger;
            _flash = flash;
            this._authSvc = _authSvc;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userc = await CurrentUser();
            if (userc.IsAdmin)
            {
                return RedirectToAction("Index", "Role");
            }

            var AD = (await _authSvc.AuthorizeAsync(
                                               User, userc,
                                              "ApproveDepartments")).Succeeded;
            var ID = (await _authSvc.AuthorizeAsync(
                                                 User, userc,
                                                 "InitiateDepartment")).Succeeded;
            var AR = (await _authSvc.AuthorizeAsync(
                                               User, userc,
                                              "ApproveRoles")).Succeeded;
            var IR = (await _authSvc.AuthorizeAsync(
                                                 User, userc,
                                                "InitiateRoles")).Succeeded;
            
            var AU = (await _authSvc.AuthorizeAsync(
                                              User, userc,
                                             "ApproveUsers")).Succeeded;
            var IU = (await _authSvc.AuthorizeAsync(
                                                 User, userc,
                                                "InitiateUser")).Succeeded;
            var SC = (await _authSvc.AuthorizeAsync(
                                                 User, userc,
                                                "SystemControl")).Succeeded;

            var IPM = (await _authSvc.AuthorizeAsync(
                                              User, userc,
                                             "InitiateProfileManagement")).Succeeded;
            var APM = (await _authSvc.AuthorizeAsync(
                                                 User, userc,
                                                 "ApproveProfileManagement")).Succeeded;

            var IAL = (await _authSvc.AuthorizeAsync(
                                             User, userc,
                                            "InitiateAccountLinking")).Succeeded;
            var AAL = (await _authSvc.AuthorizeAsync(
                                                 User, userc,
                                                 "ApproveAccountLinking")).Succeeded;


            if (AD || ID || AR || IR || AU || IU)
            {
                return RedirectToAction("Index", "Department");
            }
            //else if (IO)
            //{
            //    return RedirectToAction("Index", "ProfileManagement");
            //}
            else if (IAL)
            {
                return RedirectToAction("Index", "Wallet");
            }
            else if (IPM)
            {
                return RedirectToAction("Index", "ProfileManagement");
            }
            else if (APM || AAL)
            {
                return RedirectToAction("Index", "Notifications");

            }
            else if (SC)
            {
                return RedirectToAction("Index", "Audit");
            }


            return RedirectToAction("Index", "Role");


            //return View();
        }

        public IActionResult Test()
        {

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
