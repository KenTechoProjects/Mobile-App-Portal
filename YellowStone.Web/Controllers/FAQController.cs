using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YellowStone.Controllers;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;
using YellowStone.Services;
using YellowStone.Web.ViewModels;

namespace YellowStone.Web.Controllers
{
    public class FAQController : BaseController
    {
        private readonly ILogger _logger;
        FAQViewModel faqViewModel = new FAQViewModel();
        public FAQController(ILogger<FAQController> logger, UserManager<User> userManager, IOptions<SystemSettings> options, IUserRepository userRepository,
            IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor)
            : base(userManager, options, auditTrailLog, accessor, userRepository)
        {
            _logger = logger;
        }


        private async Task InitiateFAQ()
        {
            var user = await CurrentUser();

            faqViewModel.PageBaseClass = new PageBaseClass
            {
                User = user,
                FAQ = true,
            };
        }


        public async Task<IActionResult> ViewFaq()
        {

            try
            {
                await InitiateFAQ();
                var user = await CurrentUser();
                ViewBag.user = user;
                var action = $"accessed FAQ page";
                await AddAudit(action, user);
                return View(faqViewModel);
            }
            catch (Exception er)
            {
                _logger.LogError(string.Format("Error occured. FAQController : FAQ : {0}", er));
                return RedirectToAction("Login", "Account");
            }

        }



    }
}