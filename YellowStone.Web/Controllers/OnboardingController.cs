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
    public class OnboardingController : BaseController
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

        public OnboardingController(IFlasher flash, IAuthorizationService _authSvc, IOptions<SystemSettings> options,
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
                var isApprover = (await _authSvc.AuthorizeAsync(User, user, "ApproveOnboarding")).Succeeded;
                if (isApprover)
                {
                    onboardingViewModel.PageBaseClass.NotificationCount = pendingTransactions.Where(x => x.RequestStatus == RequestStatus.Pending).Count();
                }
                else
                {
                    onboardingViewModel.PageBaseClass.NotificationCount = pendingTransactions.Where(x => new RequestStatus[] { RequestStatus.Pending, RequestStatus.Rejected }.Contains(x.RequestStatus)).Count();
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(long? id = null)
        {
            var user = await CurrentUser();

            ViewBag.CurrentUser = user.StaffId;
            ViewBag.AccountNumber = "";

            if (id != null)
            {
                var onboardingRequest = await _customerRequestRepository.GetByID(id.Value);
                if (onboardingRequest != null)
                {
                    ViewBag.AccountNumber = onboardingRequest.AccountNumber;
                    ViewBag.Id = id;
                    ViewBag.ReInitiated = "ReInitiated";
                }
            }

            var isInitiator = (await _authSvc.AuthorizeAsync(
                                                User, user,
                                               "InitiateOnboarding")).Succeeded;
            await InitializeView(user);

            onboardingViewModel.PageBaseClass.Initiator = true;
            if (isInitiator)
            {
                onboardingViewModel.PageBaseClass.AdminPageName = "Initiate Onboarding";
            }
            else
            {
                onboardingViewModel.PageBaseClass.AdminPageName = "Approve Onboarding";
            }

            ViewBag.user = user;
            //Audit log
            var action = $"accessed onboarding initiation page";
            await AddAudit(action, user);

            return View(onboardingViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Archives()
        {
            var user = await CurrentUser();
            await InitializeView(user);
            onboardingViewModel.PageBaseClass.Initiator = true;
            try
            {
                var onboardingRequests = _customerRequestRepository.CustomerRequests
                    .Where(x => x.RequestBranchCode == user.StaffBranchCode && x.RequestState == RequestState.Archived);

                var isApprover = (await _authSvc.AuthorizeAsync(
                                    User, user,
                                   "ApproveOnboarding")).Succeeded;

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

        public async Task<IActionResult> GetAccountDetails(string accountNumber)
        {

            var result = new AccountDetailsViewModel(false);
            try
            {
                var accountDetails = await _fIService.GetAccountDetailsAsync(accountNumber);

                result = new AccountDetailsViewModel(accountDetails.IsSuccessful())
                {
                    AccountName = accountDetails.AccountName,
                    AccountNumber = accountNumber,
                    AccountStatus = GetAccountStatus(accountDetails.AccountStatus),
                    AccountType = accountDetails.AccountType,
                    Branch = accountDetails.Branch,
                    BranchCode = accountDetails.BranchCode,
                    CurrencyCode = accountDetails.CurrencyCode,
                    CustomerId = accountDetails.CustomerId,
                    Email = accountDetails.Email,
                    MobileNo = accountDetails.MobileNo
                };
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetAccountDetails", ex);
                // throw ex;
            }

            return PartialView("_AccountDetails", result);
        }

        //TODO: Refactor this later
        private string GetAccountStatus(string status)
        {
            switch (status)
            {
                case "D":
                    {
                        return "Dormant";
                    }
                case "I":
                    {
                        return "Inactive";
                    }
                case "A":
                    {
                        return "Active";
                    }
                default:
                    {
                        return "Unknown";
                    }
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                var files = HttpContext.Request.Form.Files;

                string currentUser = User.Identity.Name;
                string fileKey = currentUser + "attachment";
                string accountNumberKey = currentUser + "accountNumber";

                var request = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                var accountNumber = request[accountNumberKey];
                var file = files[fileKey];

                string clientFilename = file.FileName;
                //string fileExt = file.ContentType;
                string ext = clientFilename.Substring(clientFilename.LastIndexOf('.') + 1, clientFilename.Length - (clientFilename.LastIndexOf(".") + 1));
                if (!new string[] { "pdf", "jpeg", "jpg" }.Contains(ext))
                {
                    throw new Exception("File format not supported");
                }

                long size = files.Sum(f => f.Length);

                // full path to file in temp location
                var filename = $"{accountNumber}-{DateTime.Now.ToString("ddMMyyyyhhmmssffff")}";
                var filePath = $"{_options.Value.AttachmentsServerPath}{filename}.{ext}";

                await _fileHandler.UploadFile(file, filename, ext);

                var attachment = new Attachment()
                {
                    AccountNumber = accountNumber,
                    CreatedBy = currentUser,
                    CreatedDate = DateTime.Now,
                    FileName = filename,
                    FileType = ext
                };

                await _attachmentRepository.Create(attachment);

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError("Error", ex);
                return BadRequest(ex.Message);
            }

        }

        public async Task<IActionResult> GetOnboardingRequests(string accountNumber)
        {
            var result = new List<OnboardingHistoryViewModel>();
            try
            {
                var onboardingRequests = _customerRequestRepository.CustomerRequests.Where(x => x.AccountNumber == accountNumber && x.RequestType == RequestTypes.Onboarding).ToList();

                foreach (var item in onboardingRequests)
                {
                    var onboardingHistoryViewModel = new OnboardingHistoryViewModel()
                    {
                        Activity = item.RequestStatus,
                        AccountBranchCode = item.RequestBranchCode,
                        Date = item.CreatedDate.ToString("dd-MMM-yyyy"),
                        InitiatedBy = item.CreatedBy,
                        Time = item.CreatedDate.ToString("hh:mm:ss")
                    };
                    result.Add(onboardingHistoryViewModel);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetAccountDetails", ex);
                //result = null;
            }

            return PartialView("_OnboardingHistory", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAttachments(string accountNumber)
        {
            var result = new AttachmentsViewModel(false);
            var attachmentItems = new List<AttachmentItems>();
            try
            {
                var attachments = _attachmentRepository.Attachments.Where(x => x.AccountNumber == accountNumber).ToList();

                if (attachments.Any())
                {
                    result = new AttachmentsViewModel(true);

                    foreach (var item in attachments)
                    {
                        var attachmentItem = new AttachmentItems()
                        {
                            FileName = item.FileName,
                            FileType = item.FileType,
                            Id = item.Id,
                            DateUploaded = item.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss")
                        };
                        attachmentItems.Add(attachmentItem);
                    };

                    result.AttachmentItems = attachmentItems;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetAccountDetails", ex);
                // result = null;
            }

            return PartialView("_Attachments", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(long requestId)
        {
            var result = new CommentsViewModel(false);
            var commentItems = new List<CommentItems>();
            try
            {
                var comments = _commentsRepository.Comments.Where(x => x.RequestId == requestId && x.RequestType == RequestTypes.Onboarding).ToList();

                if (comments.Any())
                {
                    result = new CommentsViewModel(true);

                    foreach (var item in comments)
                    {
                        var commentItem = new CommentItems()
                        {
                            CommentText = item.CommentText,
                            RequestTypes = item.RequestType,
                            Id = item.Id,
                            DateCreated = item.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss"),
                            CreatedBy = item.CreatedBy
                        };
                        commentItems.Add(commentItem);
                    };

                    result.commentsItems = commentItems;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetComments", ex);
                // result = null;
            }

            return PartialView("_Comments", result);
        }


        [HttpGet]
        public async Task<FileResult> GetFile(long Id)
        {

            var attachment = _attachmentRepository.Attachments.FirstOrDefault(x => x.Id == Id);
            if (attachment == null)
            {
                return null;
            }

            var byteArray = await _fileHandler.GetFile(attachment.FileName, attachment.FileType);

            return File(byteArray, $"application/{attachment.FileType}");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteFile(long Id)
        {
            var attachment = _attachmentRepository.Attachments.FirstOrDefault(x => x.Id == Id);
            if (attachment == null)
            {
                return null;
            }

            await _fileHandler.DeleteFile(attachment.FileName, attachment.FileType);
            await _attachmentRepository.Delete(attachment);

            return Ok();

        }

        [HttpPost]
        public async Task<ActionResult> InitiateOnboarding(InitiateOnboardingViewModel model)
        {
            var user = await CurrentUser();
            try
            {

                var onboardingRequest = new CustomerRequest();
                string action;
                if (model.Id.HasValue)
                {
                    onboardingRequest = await _customerRequestRepository.GetByID(model.Id.Value);
                    onboardingRequest.RequestStatus = RequestStatus.Pending;
                    await _customerRequestRepository.Update(onboardingRequest);
                    action = "Re-Initiated Onboarding";
                }
                else
                {
                    onboardingRequest = new CustomerRequest
                    {
                        AccountNumber = model.AccountNumber,
                        RequestBranchCode = user.StaffBranchCode,
                        RequestStatus = RequestStatus.Pending,
                        CreatedBy = user.StaffId,
                        CreatedDate = DateTime.Now,
                        AccountName = model.AccountName,
                        AccountBranchCode = model.BranchCode,
                        RequestType = RequestTypes.Onboarding,
                        RequestState = RequestState.Active

                    };
                    await _customerRequestRepository.Create(onboardingRequest);
                    action = "Initiated Onboarding";

                }

                if (!string.IsNullOrEmpty(model.Comment))
                {
                    await AddComment(onboardingRequest.Id, model.Comment, user);
                };

              await AddAudit(action, user: user, accountNumber: model.AccountNumber);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult> ApproveOnboarding(InitiateOnboardingViewModel model)
        {
            var user = await CurrentUser();
            try
            {
                var onboardingRequest = await _customerRequestRepository.GetByID(model.Id.GetValueOrDefault());

                if (onboardingRequest != null)
                {
                    var result = await _onboardingProcessor.InitiateOnboarding(onboardingRequest.AccountNumber, onboardingRequest.AccountBranchCode);
                    if (result.IsSuccessful)
                    {
                        onboardingRequest.RequestStatus = RequestStatus.Approved;
                        onboardingRequest.ApprovedBy = user.StaffId;
                        onboardingRequest.ApprovedDate = DateTime.Now;

                        await _customerRequestRepository.Update(onboardingRequest);

                        if (!string.IsNullOrEmpty(model.Comment))
                        {
                            await AddComment(onboardingRequest.Id, model.Comment, user);
                        }
                      await AddAudit("Approved Onboarding : Successful", user: user, accountNumber: onboardingRequest.AccountNumber);
                    }
                    else
                    {
                       await AddAudit("Approved Onboarding : Failed", user: user, extra: "Failed", accountNumber: onboardingRequest.AccountNumber);
                        return BadRequest(result.Message);
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

        [HttpPost]
        public async Task<ActionResult> RejectOnboarding(InitiateOnboardingViewModel model)
        {
            var user = await CurrentUser();
            try
            {
                var onboardingRequest = await _customerRequestRepository.GetByID(model.Id.GetValueOrDefault());

                onboardingRequest.RequestStatus = RequestStatus.Rejected;
                onboardingRequest.ApprovedBy = user.StaffId;
                onboardingRequest.ApprovedDate = DateTime.Now;

                await _customerRequestRepository.Update(onboardingRequest);

                await AddComment(onboardingRequest.Id, model.Comment, user);

           await AddAudit("Rejected Onboarding", user: user, accountNumber: onboardingRequest.AccountNumber);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        public async Task<ActionResult> Archive(InitiateOnboardingViewModel model)
        {
            var user = await CurrentUser();
            try
            {
                var onboardingRequest = await _customerRequestRepository.GetByID(model.Id.GetValueOrDefault());
                var attachments = _attachmentRepository.Attachments.Where(x => x.AccountNumber == onboardingRequest.AccountNumber).ToList();

                foreach (var item in attachments)
                {
                    await _fileHandler.ArchiveFile(item.FileName, item.FileType);

                }
                onboardingRequest.RequestState = RequestState.Archived;

                await _customerRequestRepository.Update(onboardingRequest);

                await AddComment(onboardingRequest.Id, model.Comment, user);

                await AddAudit("Archived Onboarding", user: user, accountNumber: onboardingRequest.AccountNumber);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

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
    }
}