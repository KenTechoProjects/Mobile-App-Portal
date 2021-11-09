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
using System.Globalization;
using YellowStone.Services.FBNMobile.DTOs;

namespace YellowStone.Controllers
{
    [Authorize]
    public class ProfileManagementController : BaseController
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICommentRepository _commentsRepository;
        private readonly IUserRepository userRepository;
        private readonly ICustomerRequestRepository _customerRequestRepository;
        private readonly IFIService _fIService;
        private readonly IFbnMobileService _fbnMobileService;
        private readonly ILogger logger;
        private readonly IAuthorizationService _authSvc;
        private readonly SystemSettings _options;
        private readonly IFileHandler _fileHandler;
        IFlasher _flash;
        ProfileManagementViewModel profileManagementViewModel;

        public ProfileManagementController(IFlasher flash, IAuthorizationService _authSvc, IOptions<SystemSettings> options,
            ICommentRepository commentRepository, UserManager<User> userManager, ILogger<OnboardingController> logger,
            IUserRepository userRepository, IFileHandler fileHandler,
            IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor, IFIService fIService, IFbnMobileService fbnMobileService,
            ICustomerRequestRepository customerRequestRepository) :
            base(userManager, options, auditTrailLog, accessor, userRepository)
        {
            this.userRepository = userRepository;

            this._authSvc = _authSvc;
            this.logger = logger;
            _flash = flash;
            _options = options.Value;
            _fIService = fIService;
            _customerRequestRepository = customerRequestRepository;
            _fileHandler = fileHandler;
            _commentsRepository = commentRepository;
            _fbnMobileService = fbnMobileService;
            profileManagementViewModel = new ProfileManagementViewModel();
        }

        private async Task InitializeView(User user)
        {
            profileManagementViewModel.PageBaseClass = new PageBaseClass
            {
                User = user
            };

            var pendingTransactions = _customerRequestRepository.CustomerRequests.Where(x => x.RequestBranchCode == user.StaffBranchCode && x.RequestState == RequestState.Active);

            if (pendingTransactions.Count() > 0)
            {
                var isApprover = (await _authSvc.AuthorizeAsync(User, user, "ApproveProfileManagement")).Succeeded;
                if (isApprover)
                {
                    profileManagementViewModel.PageBaseClass.NotificationCount = pendingTransactions.Where(x => x.RequestStatus == RequestStatus.Pending).Count();
                }
                else
                {
                    profileManagementViewModel.PageBaseClass.NotificationCount = pendingTransactions.Where(x => new RequestStatus[] { RequestStatus.Pending}.Contains(x.RequestStatus)).Count();
                }
            }

            profileManagementViewModel.PageBaseClass.User = user;
            profileManagementViewModel.PageBaseClass.AdminPageName = "Profile Management";

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


            await InitializeView(user);

            ViewBag.user = user;
            var action = $"accessed customer profile management page";
            await AddAudit(action, user);

            return View(profileManagementViewModel);
        }



        [HttpGet]
        public async Task<IActionResult> CustomerInfo(string accountNumber, string viewType = "full")
        {
            var result = new CustomerInfoViewModel(false);

            var renderedView = viewType == "full" ? "_CustomerInfo" : "_CustomerInfo_modal";
            try
            {
                var customerInfo = await _fbnMobileService.GetCustomerInformation(accountNumber);


                result = new CustomerInfoViewModel(customerInfo.IsSuccessful)
                {
                    FirstName = customerInfo.FirstName,
                    AccountNumber = customerInfo.AccountNumber,
                    LastName = customerInfo.LastName,
                    IsActive = customerInfo.IsActive,
                    MiddleName = customerInfo.MiddleName,
                    PhoneNumber = customerInfo.PhoneNumber,
                    CustomerId = customerInfo.CustomerId,
                    ProfileStatus = customerInfo.IsActive.ToString(),
                    DateEnrolled = customerInfo.EnrollmentDate.ToString("dd-MMM-yyyy hh:mm:ss TT"),
                    Email = customerInfo.EmailAddress, HasAccount= customerInfo.HasAccount
                };

                if (customerInfo.UserActivities.Any())
                {
                    var customerActivities = new List<CustomerActivityViewModel>();

                    foreach (var item in customerInfo.UserActivities)
                    {
                        result.CustomerActivitiesViewModel.Add(new CustomerActivityViewModel()
                        {
                            Activity = item.Activity,
                            ActivityDate = item.ActivityDate.ToString("dd-MMM-yyyy"),
                            Time = item.ActivityDate.ToString("hh:mm:ss tt")
                        });
                    }
                }

                if (customerInfo.Transactions.Any())
                {
                    var transactions = new List<TransactionHistoryViewModel>();
                    foreach (var item in customerInfo.Transactions)
                    {
                        result.TransactionsViewModel.Add(new TransactionHistoryViewModel()
                        {
                            Status = item.TransactionStatus.ToString(),
                            Date = item.DateCreated.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                            TransactionType = item.TransactionType.ToString(),
                            Value = item.Amount.ToString(),
                            SourceAccount = item.SourceAccountNumber,
                            DestinationAccount = item.DestinationAccountNumber,
                            Currency = item.Currency
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetAccountDetails", ex);
            }

            return PartialView(renderedView, result);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDevices(string accountNumber)
        {
            var result = new CustomerDevicesViewModel(false);

            try
            {
                var customerDevices = await _fbnMobileService.GetCustomerDevices(accountNumber);

                if (customerDevices.Devices.Any())
                {
                    result.IsSuccessful = true;
                    result.CustomerDevices = customerDevices.Devices.Select(x => new CustomerDevices()
                    {
                        Model = x.Model,
                        CustomerId = x.Customer_Id,
                        DateCreated = x.DateCreated,
                        DeviceId = x.DeviceId,
                        Id = x.Id,
                        IsActive = x.IsActive
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at CustomerDevices", ex);
            }

            return PartialView("_CustomerDevices", result);
        }


        [HttpGet]
        public async Task<IActionResult> CustomerInfoById(long Id, string viewType = "full")
        {
            var result = new CustomerInfoViewModel(false);

            var renderedView = viewType == "full" ? "_CustomerInfo" : "_CustomerInfo_modal";
            try
            {
                var request = await _customerRequestRepository.GetByID(Id);

                var customerInfo = await _fbnMobileService.GetCustomerInformation(request.AccountNumber);


                result = new CustomerInfoViewModel(customerInfo.IsSuccessful)
                {
                    FirstName = customerInfo.FirstName,
                    AccountNumber = request.AccountNumber,
                    LastName = customerInfo.LastName,
                    IsActive = customerInfo.IsActive,
                    MiddleName = customerInfo.MiddleName,
                    PhoneNumber = customerInfo.PhoneNumber,
                    CustomerId = customerInfo.CustomerId,
                    ProfileStatus = customerInfo.IsActive.ToString(),
                    DateEnrolled = customerInfo.EnrollmentDate.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    Email = customerInfo.EmailAddress,
                    customerManagementViewModel = new CustomerManagementViewModel()
                    {
                        AccountName = request.AccountName,
                        AccountNumber = request.AccountNumber,
                        Id = request.Id,
                        CallingPhone = request.CallingPhoneNumber,
                        Reason = request.ActionReason,
                        RequestType = request.RequestType,
                        DeviceModel = request.DeviceModel,
                        DeviceId = request.DeviceId
                    }

                };
                if (request.RequestType != RequestTypes.ActivateDevice 
                    && request.RequestType != RequestTypes.DeactivateDevice 
                    && request.RequestType != RequestTypes.ReleaseDevice)
                {
                    if (customerInfo.UserActivities.Any())
                    {
                        var customerActivities = new List<CustomerActivityViewModel>();

                        foreach (var item in customerInfo.UserActivities)
                        {
                            result.CustomerActivitiesViewModel.Add(new CustomerActivityViewModel()
                            {
                                Activity = item.Activity,
                                ActivityDate = item.ActivityDate.ToString("dd-MMM-yyyy"),
                                Time = item.ActivityDate.ToString("hh:mm:ss tt")
                            });
                        }
                    }

                    if (customerInfo.Transactions.Any())
                    {
                        var transactions = new List<TransactionHistoryViewModel>();
                        foreach (var item in customerInfo.Transactions)
                        {
                            result.TransactionsViewModel.Add(new TransactionHistoryViewModel()
                            {
                                Status = item.TransactionStatus.ToString(),
                                Date = item.DateCreated.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                                TransactionType = item.TransactionType.ToString(),
                                Value = item.Amount.ToString("N2", CultureInfo.InvariantCulture),
                                SourceAccount = item.SourceAccountNumber,
                                DestinationAccount = item.DestinationAccountNumber,
                                Currency = item.Currency
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetAccountDetails", ex);
            }
            return PartialView(renderedView, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(long requestId)
        {
            var result = new CommentsViewModel(false);
            var commentItems = new List<CommentItems>();
            try
            {
                var comments = _commentsRepository.Comments.Where(x => x.RequestId == requestId && x.RequestType != RequestTypes.Onboarding).ToList();

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
                            DateCreated = item.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss tt"),
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
        public async Task<IActionResult> GetRequestDetails(long requestId)
        {
            CustomerManagementViewModel result = new CustomerManagementViewModel();
            try
            {
                var request = _customerRequestRepository.CustomerRequests.FirstOrDefault(x => x.Id == requestId);

                if (request != null)
                {
                    result = new CustomerManagementViewModel()
                    {
                        AccountName = request.AccountName,
                        Id = request.Id,
                        AccountNumber = request.AccountNumber,
                        CallingPhone = request.CallingPhoneNumber,
                        Reason = request.ActionReason,
                        RequestType = request.RequestType,
                        DeviceId = request.DeviceId,
                        DeviceModel = request.DeviceModel
                    };
                };
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetComments", ex);
                // result = null;
            }

            return PartialView("_Comments", result);
        }

        [HttpPost]
        public async Task<ActionResult> InitiateRequest(CustomerManagementViewModel model)
        {
            var user = await CurrentUser();
            string returnMsg;
            try
            {
                var customerRequest = new CustomerRequest();
                customerRequest = new CustomerRequest
                {
                    AccountNumber = model.AccountNumber,
                    RequestBranchCode = user.StaffBranchCode,
                    RequestStatus = RequestStatus.Pending,
                    CreatedBy = user.StaffId,
                    CreatedDate = DateTime.Now,
                    AccountName = model.AccountName,
                    CallingPhoneNumber = model.CallingPhone,
                    ActionReason = model.Reason,
                    RequestType = model.RequestType,
                    RequestState = RequestState.Active

                };

                await _customerRequestRepository.Create(customerRequest);

                returnMsg = "Request logged for approval";

                if (model.RequestType == RequestTypes.LockProfile)
                {
                    var response = await _fbnMobileService.LockProfile(model.AccountNumber);

                    customerRequest.RequestStatus = response.IsSuccessful ? RequestStatus.Successful : RequestStatus.Failed;

                    returnMsg = response.IsSuccessful ? "Account Locked Successfully" : "Cannot Lock Profile At This Time";
                    await _customerRequestRepository.Update(customerRequest);

                }

                var action = model.RequestType.GetDescription();


                if (!string.IsNullOrEmpty(model.Comment))
                {
                    await AddComment(customerRequest.Id, model.Comment, user);
                };

                await AddAudit(action, user: user, accountNumber: model.AccountNumber);

                return Ok(returnMsg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost]
        public async Task<ActionResult> ChangeDeviceStatus(CustomerManagementViewModel model)
        {
            var user = await CurrentUser();

            try
            {
                var requestAlreadyExists = await _customerRequestRepository.Any(x => x.AccountNumber == model.AccountNumber && x.DeviceId == model.DeviceId
                && x.RequestStatus == RequestStatus.Pending && x.RequestType == model.RequestType && x.RequestBranchCode == user.StaffBranchCode);

                if (requestAlreadyExists)
                {
                    return BadRequest("Request is already awaiting approval");
                }

                var customerRequest = new CustomerRequest();
                customerRequest = new CustomerRequest
                {
                    AccountNumber = model.AccountNumber,
                    DeviceId = model.DeviceId,
                    RequestBranchCode = user.StaffBranchCode,
                    RequestStatus = RequestStatus.Pending,
                    CreatedBy = user.StaffId,
                    CreatedDate = DateTime.Now,
                    AccountName = model.AccountName,
                    CallingPhoneNumber = model.CallingPhone,
                    ActionReason = model.Reason,
                    RequestType = model.RequestType,
                    RequestState = RequestState.Active,
                    DeviceModel = model.DeviceModel

                };

                await _customerRequestRepository.Create(customerRequest);

                var returnMsg = "Request logged for approval";

                if (model.RequestType == RequestTypes.DeactivateDevice)
                {
                    //call deactivate API
                    var response = await _fbnMobileService.DeactivateDevice(model.DeviceId);

                    customerRequest.RequestStatus = response.IsSuccessful? RequestStatus.Successful : RequestStatus.Failed;
                    returnMsg = response.IsSuccessful ?  "Device deactivated successfully" : $"Failed to deactivate device : {response.Error?.ResponseDescription}";
                    await _customerRequestRepository.Update(customerRequest);
                }

                var action = $"{model.RequestType.GetDescription()}:{model.DeviceId}";


                if (!string.IsNullOrEmpty(model.Comment))
                {
                    await AddComment(customerRequest.Id, model.Comment, user);
                };

                await AddAudit(action, user: user, accountNumber: model.AccountNumber);

                return Ok(returnMsg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        [HttpPost]
        public async Task<ActionResult> RejectRequest(InitiateOnboardingViewModel model)
        {
            var user = await CurrentUser();
            try
            {
                var request = await _customerRequestRepository.GetByID(model.Id.GetValueOrDefault());

                request.RequestStatus = RequestStatus.Rejected;
                request.ApprovedBy = user.StaffId;
                request.ApprovedDate = DateTime.Now;

                await _customerRequestRepository.Update(request);

                await AddComment(request.Id, model.Comment, user);

                await AddAudit($"Rejected {request.RequestType.GetDescription()}", user: user, accountNumber: request.AccountNumber);

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