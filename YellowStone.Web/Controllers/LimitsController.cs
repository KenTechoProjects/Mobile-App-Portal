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
using YellowStone.Services.FBNMobile.DTOs;
using YellowStone.Services.Processors;
using YellowStone.Web.ViewModels;

namespace YellowStone.Controllers
{
    [Authorize]
    public class LimitsController : BaseController
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;
        private readonly IAuditTrailLog auditTrailRepository;
        private readonly ILogger logger;
        readonly LimitManagementViewModel limitsViewModel = new LimitManagementViewModel();
        private readonly IHttpContextAccessor _accessor;
        private readonly IAuthorizationService _authSvc;
        private readonly IUserService _userService;
        private readonly IFbnMobileService _fbnMobileService;
        private readonly ILimitRepository _limitRepository;
        IFlasher _flash;

        string ipAddy = string.Empty;
        string remoteIp = string.Empty;
        string url = string.Empty;

        public LimitsController
            (IUserService userService, IOptions<SystemSettings> options, UserManager<User> userManager, ILogger<UserController> logger,
            IRoleRepository roleRepository, IAuditTrailLog auditTrailRepository, IUserRepository userRepository, ILimitRepository limitRepository,
            IFbnMobileService fbnMobileService, IFlasher flash, IAuthorizationService authSvc, IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor) :
            base(userManager, options, auditTrailLog, accessor, userRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.auditTrailRepository = auditTrailRepository;
            _accessor = accessor;
            this.logger = logger;
            _flash = flash;
            _authSvc = authSvc;
            _userService = userService;
            _fbnMobileService = fbnMobileService;
            _limitRepository = limitRepository;
        }

        private async Task InitiateLimitsView()
        {
            try
            {
                var user = await CurrentUser();

                limitsViewModel.PageBaseClass = new PageBaseClass();
                limitsViewModel.PageBaseClass.User = new User();
                limitsViewModel.PageBaseClass.User = user;
                limitsViewModel.PageBaseClass.Users = true;
                limitsViewModel.PageBaseClass.AdminPageName = "Limits";

                limitsViewModel.User = user;

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
                await InitiateLimitsView();
                await GetLimits();

            }
            catch (Exception er)
            {
                logger.LogError("LimitsController : Index : " + er.ToString());
                return RedirectToAction("Login", "Account");
            }

            //Audit log
            await AddAudit($"Accessed limits page", user);

            return View(limitsViewModel);
        }

        private async Task GetLimits()
        {
            var limits = await _fbnMobileService.GetLimits();
            limitsViewModel.Limits = limits.Select(x => new LimitsViewModel() { DailyLimit = x.DailyLimit, SingleLimit = x.SingleLimit, TransactionType = x.TransactionType, ID=x.ID });

            var limitRequestModel = _limitRepository.Limits.Where(x => x.Status == RequestStatus.Pending).ToList();
            limitsViewModel.LimitRequests = limitRequestModel;
        }

        [HttpPost]
        public async Task<ActionResult> ChangeLimit(LimitsViewModel model)
        {
            var user = await CurrentUser();
            string returnMsg = string.Empty;
            try
            {
                var limitRequest = new LimitRequest
                {
                    TransactionType = model.TransactionType,
                    DailyLimit = model.DailyLimit,
                    SingleLimit = model.SingleLimit,
                    CreatedBy = user.StaffId,
                    CreatedDate = DateTime.Now,


                    Status = RequestStatus.Pending
                };

                await _limitRepository.Create(limitRequest);

                if (user.IsAdmin)
                {
                    var limitDTO = new Limit()
                    {
                        DailyLimit = limitRequest.DailyLimit,
                        SingleLimit = limitRequest.SingleLimit,
                        TransactionType = limitRequest.TransactionType
                    };
                    var response = await _fbnMobileService.UpdateLimits(limitDTO);
                    if (response.IsSuccessful)
                    {
                        limitRequest.Status = RequestStatus.Approved;
                        await _limitRepository.Update(limitRequest);
                        returnMsg = "Limit Updated Successfully";
                    }
                }
                else
                {
                    returnMsg = "Request logged for approval";
                }

              await AddAudit("Modified Limit", user: user);

                return Ok(returnMsg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult> ApproveLimit(LimitsViewModel model)
        {
            var user = await CurrentUser();
            string returnMsg = string.Empty;

            try
            {
                var request = await _limitRepository.GetByID(model.ID);

                if (request != null)
                {
                    var limitDTO = new Limit()
                    {
                        DailyLimit = request.DailyLimit,
                        SingleLimit = request.SingleLimit,
                        TransactionType = request.TransactionType, ID = request.Id
                    };

                    var result = await _fbnMobileService.UpdateLimits(limitDTO);

                    if (result.IsSuccessful)
                    {
                        request.Status = RequestStatus.Approved;
                        request.ApprovedBy = user.StaffId;
                        request.ApprovedDate = DateTime.Now;

                        await _limitRepository.Update(request);

                       // _ = AddAudit("Limit Approval: Successful", user: user);
                       await AddAudit("Limit Approval: Successful", user: user);
                        returnMsg = "Limit Approved Successfully";

                    }
                    else
                    {
                        await AddAudit("Limit Approval : Failed", user: user, extra: "Failed");
                        return BadRequest(result.Error?.ResponseDescription);
                    }
                }
                else
                {
                    return BadRequest("Could not locate rquest");
                }


                return Ok(returnMsg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}