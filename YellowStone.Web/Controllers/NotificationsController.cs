using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Flash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YellowStone.Models;
using YellowStone.Models.Enums;
using YellowStone.Repository.Interfaces;
using YellowStone.Filters;
using YellowStone.Helpers;
using YellowStone.Services;
using YellowStone.Services.Processors;
using YellowStone.Web.ViewModels;
using System.Collections.Specialized;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Transactions;
using YellowStone.Services.FBNMobile.DTOs;

namespace YellowStone.Controllers
{
    [Authorize]
    public class NotificationsController : BaseController
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ICommentRepository _commentsRepository;
        private readonly IUserRepository userRepository;
        private readonly ICustomerRequestRepository _customerRequestRepository;
        private readonly IFIService _fIService;
        private readonly IOnboardingProcessor _onboardingProcessor;
        private readonly IFbnMobileService _fbnMobileService;
        private readonly ILogger logger;
        private readonly IAuthorizationService _authSvc;
        private readonly IOptions<SystemSettings> _options;
        private readonly IFileHandler _fileHandler;
        IFlasher _flash;
        readonly OnboardingViewModel onboardingViewModel = new OnboardingViewModel();

        public NotificationsController(IFlasher flash, IAuthorizationService _authSvc, IOptions<SystemSettings> options,
            ICommentRepository commentRepository, UserManager<User> userManager, ILogger<OnboardingController> logger,
            IAttachmentRepository attachmentRepository, IUserRepository userRepository, IFileHandler fileHandler,
            IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor, IFIService fIService,
            ICustomerRequestRepository customerRequestRepository, IOnboardingProcessor onboardingProcessor, IFbnMobileService fbnMobileService) :
            base(userManager, options, auditTrailLog, accessor, userRepository)
        {
            this.userRepository = userRepository;

            this._authSvc = _authSvc;
            this.logger = logger;
            _flash = flash;
            _options = options;
            _fIService = fIService;
            _customerRequestRepository = customerRequestRepository;
            _attachmentRepository = attachmentRepository;
            _fileHandler = fileHandler;
            _commentsRepository = commentRepository;
            _onboardingProcessor = onboardingProcessor;
            _fbnMobileService = fbnMobileService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await CurrentUser();
            onboardingViewModel.PageBaseClass = new PageBaseClass
            {
                User = user
            };
            try
            {
                var onboardingRequests = _customerRequestRepository.CustomerRequests
                    .Where(x => x.RequestBranchCode == user.StaffBranchCode && x.RequestState == RequestState.Active);

                if (onboardingRequests.Count() > 0)
                {
                    var isApprover = ((await _authSvc.AuthorizeAsync(User, user, "ApproveProfileManagement")).Succeeded || (await _authSvc.AuthorizeAsync(User, user, "ApproveAccountLinking")).Succeeded);
                    if (isApprover)
                    {
                        onboardingViewModel.PageBaseClass.NotificationCount = onboardingRequests.Where(x => x.RequestStatus == RequestStatus.Pending).Count();
                    }
                    else
                    {
                        onboardingViewModel.PageBaseClass.NotificationCount = onboardingRequests.Where(x => new RequestStatus[] { RequestStatus.Pending }.Contains(x.RequestStatus)).Count();
                    }
                }

                onboardingViewModel.OnboardingHistoryViewModel = new List<OnboardingHistoryViewModel>();

                foreach (var item in onboardingRequests)
                {
                    var o = new OnboardingHistoryViewModel
                    {
                        Activity = item.RequestStatus,
                        AccountBranchCode = item.AccountBranchCode,
                        Date = item.CreatedDate.ToString(),
                        InitiatedBy = item.CreatedBy,
                        Time = item.CreatedDate.ToString("hh:mm:ss"),
                        AccountName = item.AccountName,
                        AccountNumber = item.AccountNumber,
                        ApprovedBy = item.ApprovedBy,
                        DateApproved = item.ApprovedDate.ToString(),
                        Id = item.Id,
                        RequestType = item.RequestType
                    };

                    onboardingViewModel.OnboardingHistoryViewModel.Add(o);
                }
            }
            catch (Exception er)
            {
                logger.LogError(string.Format("Error occured. Notifications : Index : {0}", er));

                _flash.Flash("danger", $"Something went wrong");
            }

            ViewBag.user = user;
            await AddAudit("Accessed Notifications Page", user);
            return View(onboardingViewModel);
        }

        private async Task AddComment(long requestId, string commentText, User user)
        {
            var comment = new Comment()
            {
                RequestId = requestId,
                CommentText = commentText,
                CreatedBy = user.StaffId,
                CreatedDate = DateTime.Now,
                RequestType = RequestTypes.Onboarding
            };

            await _commentsRepository.Create(comment);
        }

        [HttpPost]
        public async Task<ActionResult> ApproveRequest(ApprovalViewModel model)
        {
            var user = await CurrentUser();
            try
            {
               // logger.LogInformation("Model  =>" + JsonConvert.SerializeObject(model));
                var request = await _customerRequestRepository.GetByID(model.Id.GetValueOrDefault());
             //   logger.LogInformation("Request  =>" + JsonConvert.SerializeObject(request));

                if (request != null)
                {
                    OperationResponse result = new OperationResponse();
                    result.IsSuccessful = false;
                    if (request.RequestType == RequestTypes.ResetProfile)
                    {
                        result = await _onboardingProcessor.ResetProfile(request.AccountNumber);
                    }
                    else if (request.RequestType == RequestTypes.ActivateDevice)
                    {
                        result = await _fbnMobileService.ActivateDevice(request.DeviceId);
                    }
                    else if (request.RequestType == RequestTypes.DeactivateDevice)
                    {
                        result = await _fbnMobileService.DeactivateDevice(request.DeviceId);
                    }
                    else if (request.RequestType == RequestTypes.LockProfile)
                    {
                        result = await _fbnMobileService.LockProfile(request.AccountNumber);
                    }
                    else if (request.RequestType == RequestTypes.UnlockProfile)
                    {
                        result = await _fbnMobileService.UnlockProfile(request.AccountNumber);
                    }
                    else if (request.RequestType == RequestTypes.ReleaseDevice)
                    {
                        result = await _fbnMobileService.ReleaseDevice(request.DeviceId);
                    }
                    else if (request.RequestType == RequestTypes.LinkAccount)
                    {
                        result = await _onboardingProcessor.LinkAccount(request.CustomerId, request.WalletNumber);
                    }
                    else
                    {
                        throw new Exception("Invalid Approval Operation");

                    }

                    if (result.IsSuccessful)
                    {
                        request.RequestStatus = RequestStatus.Approved;
                        request.ApprovedBy = user.StaffId;
                        request.ApprovedDate = DateTime.Now;

                        await _customerRequestRepository.Update(request);

                        if (!string.IsNullOrEmpty(model.Comment))
                        {
                            await AddComment(request.Id, model.Comment, user);
                        }
                        await AddAudit($"Approved {request.RequestType.GetDescription()} : Successful", user: user, accountNumber: request.AccountNumber);
                    }
                    else
                    {
                        await AddAudit($"Approved {request.RequestType.GetDescription()} : Failed", user: user, extra: "Failed", accountNumber: request.AccountNumber);
                        return BadRequest(result.Error?.ResponseDescription);
                    }
                }
                else
                {
                    return BadRequest();
                }


                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}