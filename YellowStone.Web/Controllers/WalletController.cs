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
using YellowStone.Controllers;
using YellowStone.Models;
using YellowStone.Models.Enums;
using YellowStone.Repository.Interfaces;
using YellowStone.Services;
using YellowStone.Services.FBNService.DTOs;
using YellowStone.Services.FIService.DTOs;
using YellowStone.Services.Processors;
using YellowStone.Services.WalletService;
using YellowStone.Services.WalletService.DTOs;
using YellowStone.Web.ViewModels;

namespace YellowStone.Web.Controllers
{
    [Authorize]
    public class WalletController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly IAuthorizationService _authSvc;
        private readonly ILogger<WalletController> _logger;
        private readonly IFlasher _flash;
        private readonly IFIService _fIService;
        private readonly ICustomerRequestRepository _customerRequestRepository;
        private readonly IFileHandler _fileHandler;
        private readonly ICommentRepository _commentsRepository;
        private readonly IFbnMobileService _fbnMobileService;
        private readonly WalletViewModel _walletViewModel;
        private readonly IWalletService _walletService;
        private readonly IFbnService _fbnService;
        private readonly IOptions<WalletServiceSettings> _walletOptions;
        private readonly IOptions<SystemSettings> _options;
        private readonly IOnboardingProcessor _onboardingProcessor;

        public WalletController(
            IFlasher flash,
            IAuthorizationService _authSvc,
            IOptions<SystemSettings> options,
            IOptions<WalletServiceSettings> walletOptions,
            ICommentRepository commentRepository,
            UserManager<User> userManager,
            ILogger<WalletController> logger,
            IUserRepository userRepository,
            IFileHandler fileHandler,
            IAuditTrailLog auditTrailLog,
            IHttpContextAccessor accessor,
            IFIService fIService,
            IFbnMobileService fbnMobileService,
            IFbnService fbnService,
            ICustomerRequestRepository customerRequestRepository,
            IOnboardingProcessor onboardingProcessor,
            IWalletService walletService) :
            base(userManager, options, auditTrailLog, accessor, userRepository)
        {
            this.userRepository = userRepository;

            this._authSvc = _authSvc;
            _logger = logger;
            _flash = flash;
            _options = options;
            _fIService = fIService;
            _customerRequestRepository = customerRequestRepository;
            _fileHandler = fileHandler;
            _commentsRepository = commentRepository;
            _fbnMobileService = fbnMobileService;
            _walletViewModel = new WalletViewModel();
            _walletService = walletService;
            _fbnService = fbnService;
            _walletOptions = walletOptions;
            _onboardingProcessor = onboardingProcessor;
        }

        private async Task InitializeView(User user)
        {
            _walletViewModel.PageBaseClass = new PageBaseClass
            {
                User = user
            };

            ViewBag.user = user;
            _walletViewModel.PageBaseClass.AdminPageName = "Account Linking";
            var pendingTransactions = _customerRequestRepository.CustomerRequests.Where(x => x.RequestBranchCode == user.StaffBranchCode && x.RequestState == RequestState.Active);

            if (pendingTransactions.Count() > 0)
            {
                var isApprover = (await _authSvc.AuthorizeAsync(User, user, "ApproveAccountLinking")).Succeeded;
                if (isApprover)
                {
                    _walletViewModel.PageBaseClass.NotificationCount = pendingTransactions.Where(x => x.RequestStatus == RequestStatus.Pending).Count();
                }
                else
                {
                    _walletViewModel.PageBaseClass.NotificationCount = pendingTransactions.Where(x => new RequestStatus[] { RequestStatus.Pending }.Contains(x.RequestStatus)).Count();
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await CurrentUser();

            ViewBag.CurrentUser = user.StaffId;
            ViewBag.user = user;
            ViewBag.AccountNumber = "";

            await InitializeView(user);

            ViewBag.user = user;
            var action = $"accessed customer profile management page";
            await AddAudit(action, user);

            return View(_walletViewModel);
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
                _logger.LogError("Error at GetAccountDetails", ex);
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

        [HttpGet]
        public async Task<IActionResult> GetWallet(string walletId)
        {
            var user = await CurrentUser();

            _walletViewModel.WalletInfoViewModel = new WalletInfoViewModel(false);

            var request = _customerRequestRepository.CustomerRequests.Where(x => x.WalletNumber == walletId && x.RequestType == RequestTypes.LinkAccount).ToList();

            if (request.Any(x => x.RequestStatus == RequestStatus.Approved))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Wallet Already Linked to an Account");
            }

            if (request.Any(x => x.RequestStatus == RequestStatus.Pending && x.RequestBranchCode == user.StaffBranchCode))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"There is a already a pending request in your branch");
            }

            try
            {
                var response = await _walletService.GetWalletAsync(walletId);
                if (response == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Invalid Wallet ID");

                }
                else if (!string.IsNullOrEmpty(response.Code))
                {
                    return StatusCode(StatusCodes.Status404NotFound, response.Message);

                }
                else
                {
                    _walletViewModel.WalletInfoViewModel = new WalletInfoViewModel(true)
                    {
                        AccountName = response.Name,
                        Balance = response.Balance,
                        WalletAccountNumber = response.WalletAccountNumber,
                        Currency = _walletOptions.Value.Currency,
                        WalletId = walletId,
                        Email = response.Email,
                        PhoneNumber = response.PhoneNumber,
                        WalletType = response.WalletType
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong, please try again later");
            }
            return PartialView("_WalletDetails", _walletViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetLinkAccountRequestById(long id)
        {
            var model = new WalletLinkingViewModel(false);
            try
            {
                var request = _customerRequestRepository.CustomerRequests.FirstOrDefault(x => x.Id == id);

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var user = await CurrentUser();

                var accountDataTask = _fIService.GetAccountDetailsAsync(request.AccountNumber.Trim());

                var responseTask = _walletService.GetWalletAsync(request.WalletNumber.Trim());

                await Task.WhenAll(new List<Task> { accountDataTask, accountDataTask });

                var accountData = accountDataTask.Result;
                var response = responseTask.Result;


                if (!accountData.IsSuccessful())
                {
                    return StatusCode(StatusCodes.Status409Conflict, accountData.ResponseMessage);
                }

                var accountDetails = new AccountDetailsViewModel(accountData.IsSuccessful())
                {
                    AccountName = accountData.AccountName,
                    AccountNumber = request.AccountNumber,
                    AccountStatus = GetAccountStatus(accountData.AccountStatus),
                    AccountType = accountData.AccountType,
                    Branch = accountData.Branch,
                    BranchCode = accountData.BranchCode,
                    CurrencyCode = accountData.CurrencyCode,
                    CustomerId = accountData.CustomerId,
                    Email = accountData.Email,
                    MobileNo = accountData.MobileNo
                };

                model.AccountDetailsViewModel = accountDetails;
                model.AccountDetailsViewModel.IsSuccessful = true;


                response = await _walletService.GetWalletAsync(request.WalletNumber);
                if (response.Code != null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, response.Message);
                }

                model.WalletInfoViewModel = new WalletInfoViewModel(true)
                {
                    AccountName = response.Name,
                    Balance = response.Balance,
                    WalletAccountNumber = response.WalletAccountNumber,
                    Currency = _walletOptions.Value.Currency,
                    WalletId = request.WalletNumber,
                    Email = response.Email,
                    PhoneNumber = response.PhoneNumber,
                    WalletType = response.WalletType
                };

                //  _ = AddAudit("Link Account", user: user, accountNumber: request.AccountNumber);

                return PartialView("_WalletLinkingModal", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong, please try again later");
            }

        }

        private string CleanUpInput(string input)
        {
            input = input.Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "").Replace(" ", "");

            if (input.StartsWith("00"))
            {
                input = input.TrimStart('0').TrimStart('0');
            }
            return input;
        }

        private bool isSamePhone(string BankPhone, string WalletPhone)
        {
            BankPhone = CleanUpInput(BankPhone);
            WalletPhone = CleanUpInput(WalletPhone);

            if (string.Equals(BankPhone, WalletPhone))
            {
                return true;
            }

            return false;
        }

        [HttpPost]
        public async Task<IActionResult> LinkAccount(LinkAccountRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (!isSamePhone(model.BankPhoneNumber, model.WalletPhoneNumber))
                {
                    return BadRequest("Phone numbers do not match");
                }
                var user = await CurrentUser();


                var customerRequest = new CustomerRequest
                {
                    AccountNumber = model.AccountNumber,
                    RequestBranchCode = user.StaffBranchCode,
                    RequestStatus = RequestStatus.Pending,
                    CreatedBy = user.StaffId,
                    CreatedDate = DateTime.Now,
                    AccountName = model.AccountName,
                    RequestState = RequestState.Active,
                    WalletNumber = model.WalletId,
                    RequestType = RequestTypes.LinkAccount,
                    CustomerId = model.CIF
                };

                await _customerRequestRepository.Create(customerRequest);
                _logger.LogInformation($"Account Linked {JsonConvert.SerializeObject(customerRequest)}");
                await AddAudit("Link Account", user: user, accountNumber: model.AccountNumber);
                 // Task.WhenAll(new List<Task> { task1 , task2 });

                return Ok("Request logged for approval");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong, please try again later");
            }

        }

        public async Task<IActionResult> GetRequests(string walletNumber)
        {
            var result = new List<CustomerRequestHistory>();
            try
            {
                var request = _customerRequestRepository.CustomerRequests.Where(x => x.WalletNumber == walletNumber && x.RequestType == RequestTypes.LinkAccount).ToList();

                foreach (var item in request)
                {
                    var onboardingHistoryViewModel = new CustomerRequestHistory()
                    {
                        Activity = item.RequestStatus,
                        AccountBranchCode = item.RequestBranchCode,
                        Date = item.CreatedDate.ToString("dd-MMM-yyyy"),
                        InitiatedBy = item.CreatedBy,
                        Time = item.CreatedDate.ToString("hh:mm:ss"),
                        WalletId = item.WalletNumber,
                        AccountNumber = item.AccountNumber
                    };
                    result.Add(onboardingHistoryViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetRequests", ex);
                //result = null;
            }

            return PartialView("_RequestHistory", result);
        }

        //[HttpPost]
        //public async Task<IActionResult> FundWallet(FundWalletRequest request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest();
        //        }
        //        var user = await CurrentUser();
        //        request.TransactionReference = request.TransactionReference ?? new Guid().ToString();
        //        var tellerTillRequest = new TellerTillRequest
        //        {
        //            RequestId = request.TransactionReference,
        //            Currency = request.Currency,
        //            Username = user.UserName
        //        };
        //        var tellerTillResponse = await _fbnService.GetTellerTillAccountAsync(tellerTillRequest);
        //        var transferRequest = new TransferRequest
        //        {
        //            Amount = request.Amount,
        //            ClientReferenceId = request.TransactionReference,
        //            CountryId = _options.Value.CountryId,
        //            DestinationAccountNumber = _walletOptions.Value.SuspenseAccount,
        //            SourceAccountNumber = tellerTillResponse.TillAccount
        //        };
        //        var transferResponse = await _fIService.TransferAsync(transferRequest);
        //        if (!transferResponse.IsSuccessful())
        //        {
        //            return StatusCode(StatusCodes.Status422UnprocessableEntity, "Transfer failed. Please try again");
        //        }
        //        var response = await _walletService.FundWalletAsync(request);
        //        if (response != null && response.Message != null)
        //        {
        //            return StatusCode(StatusCodes.Status422UnprocessableEntity, response.Message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong, please try again later");
        //    }
        //    return Ok("Wallet has been funded successfully");
        //}
    }
}
