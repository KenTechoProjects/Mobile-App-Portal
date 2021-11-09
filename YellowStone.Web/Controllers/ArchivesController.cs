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

namespace YellowStone.Controllers
{
    [Authorize]
    public class ArchivesController : BaseController
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ICommentRepository _commentsRepository;
        private readonly IUserRepository userRepository;
        private readonly ICustomerRequestRepository _customerRequestRepository;
        private readonly IFIService _fIService;
        private readonly IOnboardingProcessor _onboardingProcessor;

        private readonly ILogger logger;
        private readonly IAuthorizationService _authSvc;
        private readonly IOptions<SystemSettings> _options;
        private readonly IFileHandler _fileHandler;
        readonly IFlasher _flash;

        readonly OnboardingViewModel onboardingViewModel;

        public ArchivesController(IFlasher flash, IAuthorizationService _authSvc, IOptions<SystemSettings> options,
            ICommentRepository commentRepository, UserManager<User> userManager, ILogger<OnboardingController> logger,
            IAttachmentRepository attachmentRepository, IUserRepository userRepository, IFileHandler fileHandler,
            IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor, IFIService fIService,
            ICustomerRequestRepository customerRequestRepository, IOnboardingProcessor onboardingProcessor) :
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
            onboardingViewModel = new OnboardingViewModel();
        }

        private async Task InitializeView(User user)
        {
            onboardingViewModel.PageBaseClass = new PageBaseClass
            {
                User = new User()
            };
            onboardingViewModel.PageBaseClass.User = user;
            onboardingViewModel.PageBaseClass.ShowFAQ = true;

            var pendingTransactions = _customerRequestRepository.CustomerRequests.Where(x => x.RequestBranchCode == user.StaffBranchCode && x.RequestState == RequestState.Active);

            if (pendingTransactions.Count() > 0)
            {
                var isApprover =( (await _authSvc.AuthorizeAsync(User, user, "ApproveProfileManagement")).Succeeded || (await _authSvc.AuthorizeAsync(User, user, "ApproveAccountLinking")).Succeeded);
                if (isApprover)
                {
                    onboardingViewModel.PageBaseClass.NotificationCount = pendingTransactions.Where(x => x.RequestStatus == RequestStatus.Pending).Count();
                }
                else
                {
                    onboardingViewModel.PageBaseClass.NotificationCount = pendingTransactions.Where(x => new RequestStatus[] { RequestStatus.Pending }.Contains(x.RequestStatus)).Count();
                }
            }
        }

      
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await CurrentUser();
            await InitializeView(user);
            onboardingViewModel.PageBaseClass.Initiator = true;
            try
            {
                var onboardingRequests = _customerRequestRepository.CustomerRequests
                    .Where(x => x.RequestBranchCode == user.StaffBranchCode && x.RequestState == RequestState.Archived);

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
                        Id = item.Id
                    };

                    onboardingViewModel.OnboardingHistoryViewModel.Add(o);
                }

            }
            catch (Exception er)
            {
                logger.LogError(string.Format("Error occured. OnboardingInitiaion : Index : {0}", er));

                _flash.Flash("danger", $"Something went wrong");
            }

            ViewBag.user = user;
            await AddAudit("accessed contact notification page", user);
            return View(onboardingViewModel);
        }

    }
}