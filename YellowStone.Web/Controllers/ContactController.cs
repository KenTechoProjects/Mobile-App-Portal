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
using Apollo.Models;
using Apollo.Models.Enums;
using Apollo.Repository;
using Apollo.Repository.Interfaces;
using Apollo.Filters;
using Apollo.Helpers;
using Apollo.HttpClientService;
using Apollo.Models;
using Apollo.Models.HttpClientModel;
using Apollo.ViewModels;

namespace Apollo.Controllers
{
    [Authorize]
    public class ContactController : BaseController
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly ICustomerHistoryRepository customerHistoryRepository;
        private readonly IUserRepository userRepository;
        private readonly IWhatsAppCustomerService _whatsAppCustomerService;

        private readonly ILogger logger;
        private readonly IAuthorizationService _authSvc;
        private readonly IOptions<Settings> _options;
        IFlasher _flash;
        ContactViewModel contactViewModel = new ContactViewModel();

        public ContactController(IFlasher flash, IAuthorizationService _authSvc, IOptions<Settings> options,
            UserManager<User> userManager, ILogger<ContactController> logger,
            ICustomerHistoryRepository customerHistoryRepository, IUserRepository userRepository,
            IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor, IWhatsAppCustomerService whatsAppCustomerService) :
            base(userManager, options, auditTrailLog, accessor)
        {
            this.userRepository = userRepository;

            this.customerHistoryRepository = customerHistoryRepository;
            this._authSvc = _authSvc;
            this.logger = logger;
            _flash = flash;
            _whatsAppCustomerService = whatsAppCustomerService;
            _options = options;
        }

        private async Task InitiateContact()
        {
            var user = await CurrentUser();

            contactViewModel.PageBaseClass = new Models.PageBaseClass();
            contactViewModel.PageBaseClass.User = new Apollo.Models.User();
            contactViewModel.PageBaseClass.User = user;
            contactViewModel.PageBaseClass.ShowFAQ = true;

            contactViewModel.CusInformationHistories = new List<CusInformationHistory>();
            contactViewModel.CustomerViews = new List<CustomerView>();
            contactViewModel.TransactionViews = new List<TransactionView>();

            var pendingTransactions = customerHistoryRepository.FindWhere(x => x.Status == "PP").ToList();

            if (pendingTransactions.Count() > 0)
            {
                contactViewModel.PageBaseClass.NotificationCount = pendingTransactions.Count();
            }


        }

        [HttpGet]
        public async Task<IActionResult> Index(string Search = "", bool Error = false, bool Ajax = false, int? Page = 1, int PageSize = 20000)
        {

            var userc = await CurrentUser();

            var isInitiator = (await _authSvc.AuthorizeAsync(
                                                User, userc,
                                               "InitiatorContact")).Succeeded;
            var isApproval = (await _authSvc.AuthorizeAsync(
                                                 User, userc,
                                                "ApprovalNotification")).Succeeded;
            try
            {
                await InitiateContact();
                contactViewModel.PageBaseClass.Initiator = true;
                if (isInitiator)
                {
                    contactViewModel.PageBaseClass.AdminPageName = "FirstContact Initiator";
                }
                else
                {
                    contactViewModel.PageBaseClass.AdminPageName = "FirstContact Approval";
                }

                contactViewModel.CustomerViews = new List<CustomerView>();
                List<CustomerView> customerViews = new List<CustomerView>();
                CustomerView customerView = new CustomerView();

                CustomerHistory customerHistory = new CustomerHistory();
                List<CustomerHistory> customerHistories = new List<CustomerHistory>();
                string EnrollmentStatus2 = string.Empty;

                contactViewModel.CusInformationHistories = new List<CusInformationHistory>();
                CusInformationHistory cusInformationHistory = new CusInformationHistory();

                string status = string.Empty;

                if (!string.IsNullOrEmpty(Search))
                {
                    var query = "234" + Search.Substring(Search.Length - 10);
                    var customer = await _whatsAppCustomerService.GetCustomer(query);

                    if (customer != null)
                    {
                        customerHistories = customerHistoryRepository.FindWhere(x => x.PhoneNumber == customer.phoneNumber).OrderByDescending(x => x.CreatedDate).Take(5).ToList();
                        customerHistory = customerHistoryRepository.FindWhere(x => x.PhoneNumber == customer.phoneNumber).OrderByDescending(x => x.CreatedDate).Take(1).SingleOrDefault();

                        customerView.AccountNumber = customer.accountNumber;

                        customerView.IsActive = customer.isActive;
                        customerView.Name = customer.name;

                        // EnrollmentStatus(customer, ref status, ref EnrollmentStatus2);
                        customerView.EnrollmentStatus2 = DisplayNameFromEnum.GetCustomerStatus((CustomerStatus)customer.status);

                        customerView.EnrollmentStatus = DisplayNameFromEnum.GetCustomerStatus((CustomerStatus)customer.status);
                        customerView.DateEnrolled = string.Format("{0:dd/MM/yyyy}", customer.enrollmentDate);
                        customerView.PhoneNumber = customer.phoneNumber;
                        customerView.ProfileStatus = DisplayName.ProfileStatusName(customer.isLockedOut);
                        customerView.IsLockedOut = Convert.ToBoolean(customer.isLockedOut);
                        customerView.CustomerDetail = CryptoEngine.Encrypt(GetCustomerDetail(customer), _options.Value.CryptoKey);



                        customerView.ProfileStatus2 = DisplayNameFromEnum.GetProfileStatusName(customer.isLockedOut);


                        if (customerHistory != null)
                        {
                            customerView.LastChangeBy = customerHistory.CreatedBy;
                            customerView.LastChangeDate = string.Format("{0:dd/MM/yyyy HH:mm:ss tt}", customerHistory.CreatedDate);

                            customerView.LastChangeReason = customerHistory.Remark;
                        }

                        customerView.Status = (CustomerStatus)customer.status;
                        contactViewModel.CustomerViews.Add(customerView);


                        //Fetching Customer's History
                        FetchCusInformationHistory(customer);

                        //Fetch Customer Top 10 Transactions
                        FetchCustomerTransactions(customer);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "The phone number has no associated record.";
                        _flash.Flash("danger", "The phone number has no associated record.");
                    }

                    if (Ajax)
                    {
                        if (contactViewModel.CustomerViews != null && contactViewModel.CustomerViews.Count == 0)
                            ViewBag.ErrorMessage = "There are currently no records.";

                        return PartialView("_Index", contactViewModel.CustomerViews);
                    }
                }

            }
            catch (Exception er)
            {
                logger.LogError(string.Format("Error occured. ContactController : Index : {0}", er));
            }
            ViewBag.user = userc;
            //Audit log
            var action = $"User with staff id: {userc.StaffId} accessed contact page";
            await AddAudit(action);
            return View(contactViewModel);
        }

        private void EnrollmentStatus(Customers customer, ref string status, ref string EnrollmentStatus2)
        {
            string result = string.Empty;

            if (CustomerStatus.ENROLLED.ToString() == customer.Status.ToString())
            {
                status = string.Format("ENROLLED {1}", status, customer.EnrolmentDate);
                EnrollmentStatus2 = "Enrolled";
            }
            else if (CustomerStatus.OPTED_IN.ToString() == customer.Status.ToString())
            {
                status = string.Format("OPTED IN {1}", status, customer.OptInDate);
                EnrollmentStatus2 = "Opted In";

            }
            else if (CustomerStatus.OPTED_OUT.ToString() == customer.Status.ToString())
            {
                status = "OPTED OUT";
                EnrollmentStatus2 = "Opted Out";

            }
            else if (CustomerStatus.PRE_OPTIN.ToString() == customer.Status.ToString())
            {
                status = "PRE OPTIN";
                EnrollmentStatus2 = "Pre Optin";
            }
            else
            {
                status = "NOT OPTED IN";
                EnrollmentStatus2 = "Not Opted In";
            }

        }

        private void FetchCusInformationHistory(CustomerDetail customer)
        {

            contactViewModel.CusInformationHistories = new List<CusInformationHistory>();
            CusInformationHistory cusInformationHistory = new CusInformationHistory();
            //var customerHistories = customerHistoryRepository.FindWhere(x => x.Id == customer.Id).OrderByDescending(x => x.CreatedDate).Take(5).ToList();

            cusInformationHistory.ID = 1;
            cusInformationHistory.Activity = "Enrolled";
            if (customer.enrollmentDate != null)
            {
                cusInformationHistory.CreatedDate = string.Format("{0:dd/MM/yyyy}", customer.enrollmentDate);
                cusInformationHistory.Time = string.Format("{0:HH:mm:ss}", customer.enrollmentDate);
            }
            contactViewModel.CusInformationHistories.Add(cusInformationHistory);

            cusInformationHistory = new CusInformationHistory();
            cusInformationHistory.ID = 2;
            cusInformationHistory.Activity = "Opted In";
            if (customer.optInDate != null)
            {
                cusInformationHistory.CreatedDate = string.Format("{0:dd/MM/yyyy}", customer.optInDate);
                cusInformationHistory.Time = string.Format("{0:HH:mm:ss}", customer.optInDate);
            }
            contactViewModel.CusInformationHistories.Add(cusInformationHistory);

            cusInformationHistory = new CusInformationHistory();
            cusInformationHistory.ID = 3;
            cusInformationHistory.Activity = "Opted Out";
            if (customer.optInDate != null)
            {
                cusInformationHistory.CreatedDate = ""; //string.Format("{0:dd/MM/yyyy}", customer.OptInDate);
                cusInformationHistory.Time = ""; //string.Format("{0:HH:mm:ss}", customer.OptInDate);
            }
            contactViewModel.CusInformationHistories.Add(cusInformationHistory);

        }

        private void FetchCustomerTransactions(CustomerDetail customer)
        {
            contactViewModel.TransactionViews = new List<TransactionView>();


            List<Models.HttpClientModel.Transactions> transactions = customer.Transactions;

            if (transactions != null && transactions.Count > 0)
            {
                foreach (var trans in transactions)
                {
                    TransactionView transactionView = new TransactionView();
                    transactionView.Date = string.Format("{0:dd/MM/yyyy}", trans.date);
                    transactionView.Status = Enum.GetName(typeof(TransactionStatus), (int)trans.status);
                    transactionView.Transaction = Enum.GetName(typeof(TransactionType), trans.transactionType);
                    transactionView.Value = string.Format("{0:n}", trans.amount);
                    transactionView.Description = trans.statusDescription;
                    transactionView.Details = trans.details;
                    contactViewModel.TransactionViews.Add(transactionView);

                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> ProfileChange(CustomerView customerView)
        {
            Customers customer = new Customers();
            try
            {
                string status = string.Empty;
                string EnrollmentStatus2 = string.Empty;
                User user = await CurrentUser();

                if (ModelState.IsValid)
                {
                    CustomerHistory customerHistory = new CustomerHistory();
                    customerHistory.CreatedBy = user.StaffId;
                    customerHistory.CreatedDate = DateTime.Now;
                    customerHistory.CustomerName = customerView.Name;
                    var oldData = CryptoEngine.Decrypt(customerView.CustomerDetail, _options.Value.CryptoKey);
                    customerHistory.EnrollmentStatus = customerView.EnrollmentStatus2;
                    customerHistory.IsLockedOut = customerView.IsLockedOut;
                    customerHistory.PhoneNumber = customerView.PhoneNumber;
                    if (customer.IsLockedOut)
                        customerHistory.ProfileStatus = false;
                    else
                        customerHistory.ProfileStatus = true;


                    EnrollmentStatus(customer, ref status, ref EnrollmentStatus2);

                    customerHistory.Reason = customerView.Reason;
                    customerHistory.Remark = customerView.Remark;
                    customerHistory.PhoneNumber = customerView.PhoneNumber;
                    customerHistory.AccountNumber = customerView.AccountNumber;
                    customerHistory.CallerNumber = customerView.CustomerPhoneNumber;
                    customerHistory.EnrollmentStatus = EnrollmentStatus2;
                    customerHistory.CustomerName = customerView.Name;
                    customerHistory.CustomerDetails = customerView.CustomerDetail;

                    if (customerHistory.IsLockedOut == true)
                    {
                        customerHistory.Status = "AA";
                        customerHistory.ApprovedBy = user.StaffId;
                        customerHistory.ApprovedDate = DateTime.Now;
                        customerHistory.HistoryStatus = false;
                        bool result = await customerHistoryRepository.SaveCustomerProfileChange(customerHistory);
                        var response = await _whatsAppCustomerService.UpdateCustomerStatus(customerHistory.PhoneNumber, !customerHistory.IsLockedOut);
                        if (result)
                        {
                            var action = $"User with staff id: {user.StaffId} approved  a customer change status profile";
                            var data = $"old data : {oldData}|new data:  {JsonConvert.SerializeObject(new { Phonenumber = customerHistory.PhoneNumber, Status = "Unlocked to Locked" })}";
                            await AddAudit(action, null, data);
                            _flash.Flash("success", $"Customer status have been updated successfully");
                            return RedirectToAction("Index", new { Search = customer.PhoneNumber });
                        }
                    }
                    else
                    {
                        customerHistory.Status = "PP";
                        customerHistory.HistoryStatus = true;
                        bool result = await customerHistoryRepository.SaveCustomerProfileChange(customerHistory);
                        if (result)
                        {

                            var action = $"User with staff id: {user.StaffId} requested a change in customer status profile";
                            var data = $"old data : {oldData}|new data:  {JsonConvert.SerializeObject(new { Phonenumber = customerHistory.PhoneNumber, Status = "From Locked to Unlock" })}";
                            await AddAudit(action, null, data);

                            _flash.Flash("success", $"Your request has been sent for approval");
                            return RedirectToAction("Index", new { Search = customer.PhoneNumber });
                        }
                    }
                }
            }
            catch (Exception er)
            {
                _flash.Flash("danger", $"Something went wrong, Cannot Update Customer request at this time");
                logger.LogError(string.Format("Error occured. ContactController : Index : {0}", er));
            }

            return RedirectToAction("Index", new { Search = customer.PhoneNumber });
        }


        [HttpGet]
        public async Task<IActionResult> Notification(string sortOrder, string currentFilter, string currentSearch, string fromDate = "", string toDate = "", string Search = "", string Status = "", bool Error = false, bool Ajax = false, int? Page = 1, int PageSize = 10)
        {
            var userc = await CurrentUser();
            try
            {
                ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_asc" : "";
                ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";

                if (!string.IsNullOrEmpty(Status))
                {
                    ViewData["CurrentFilter"] = Status;
                }
                if (!string.IsNullOrEmpty(Search))
                {
                    ViewData["CurrentSearch"] = Search;
                }


                await InitiateContact();
                contactViewModel.PageBaseClass.InitiatorNotification = true;
                contactViewModel.PageBaseClass.AdminPageName = "";
                contactViewModel.NotificationViews = new List<NotificationView>();
                List<CustomerHistory> customerHistories = new List<CustomerHistory>();

                Customers customer = new Customers();

                customerHistories = customerHistoryRepository.CustomerHistorys.ToList();

                if (!string.IsNullOrEmpty(Status))
                    customerHistories = customerHistories.Where(x => x.Status == Status).ToList();

                if (!string.IsNullOrEmpty(Search))
                {

                    var query = Search.Substring(Search.Length - 10);
                    customerHistories = customerHistories.Where(x => EF.Functions.Like(x.PhoneNumber, $"%{query}") || EF.Functions.Like(x.AccountNumber, $"%{query}")).ToList();
                }
                if (customerHistories != null && customerHistories.Count > 0)
                {
                    foreach (var cusHistory in customerHistories)
                    {

                        NotificationView notificationView = new NotificationView();
                        notificationView.AccountNumber = cusHistory.AccountNumber;
                        notificationView.ApprovedBy = cusHistory.ApprovedBy;
                        notificationView.CreatedBy = cusHistory.CreatedBy;
                        notificationView.ID = cusHistory.Id;


                        if (cusHistory.CreatedDate != null)
                            notificationView.CreatedDate = string.Format("{0:dd/MM/yyyy hh:mm tt}", cusHistory.CreatedDate);


                        if (cusHistory.ApprovedDate != null)
                            notificationView.ApprovedDate = string.Format("{0:dd/MM/yyyy hh:mm tt}", cusHistory.ApprovedDate);

                        notificationView.Name = cusHistory.CustomerName;
                        notificationView.PhoneNumber = cusHistory.PhoneNumber;
                        notificationView.Reason = cusHistory.Reason;
                        notificationView.Remark = cusHistory.Remark;
                        notificationView.Status2 = DisplayNameFromEnum.GetProfileStatusName(cusHistory.IsLockedOut);
                        notificationView.ManagerRemark = cusHistory.ManagerRemark;
                        if (cusHistory.Status.Equals("PP"))
                        {
                            notificationView.ProfileStatus = "Pending Approval";
                            notificationView.Status = "Awaiting Manager's Approval";
                        }
                        else if (cusHistory.Status.Equals("AA"))
                        {
                            notificationView.ProfileStatus = "Approved";
                            notificationView.Status = "Approved";
                        }
                        else if (cusHistory.Status.Equals("RR"))
                        {
                            notificationView.ProfileStatus = "Not Approved";
                            notificationView.Status = "Not Approved";
                        }

                        contactViewModel.NotificationViews.Add(notificationView);

                    }

                    contactViewModel.NotificationViewsList = PaginatedList<NotificationView>.Create((IEnumerable<NotificationView>)contactViewModel.NotificationViews, Page ?? 1, PageSize);
                }
                else
                {
                    ViewBag.ErrorMessage = "There are currently no records.";
                }



            }
            catch (Exception er)
            {
                logger.LogError(string.Format("Error occured. ContactController : Index : {0}", er));

                _flash.Flash("danger", $"Something went wrong");
            }
            ViewBag.user = userc;
            //Audit log
            var action = $"User with staff id: {userc.StaffId} accessed contact notification page";
            await AddAudit(action);
            return View(contactViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var customerHistory = customerHistoryRepository.CustomerHistorys.Where(x => x.Id == id).FirstOrDefault();
            var customerDetail = CryptoEngine.Decrypt(customerHistory.CustomerDetails, _options.Value.CryptoKey);
            CustomerDetailAudit customer = JsonConvert.DeserializeObject<CustomerDetailAudit>(customerDetail);
            CustomerView customerView = new CustomerView();
            customerView.Id = customerHistory.Id;
            customerView.IsActive = customer.isActive;
            customerView.Name = customer.name;

            // EnrollmentStatus(customer, ref status, ref EnrollmentStatus2);
            customerView.EnrollmentStatus2 = DisplayNameFromEnum.GetCustomerStatus((CustomerStatus)customer.status);

            customerView.EnrollmentStatus = DisplayNameFromEnum.GetCustomerStatus((CustomerStatus)customer.status);
            customerView.DateEnrolled = string.Format("{0:dd/MM/yyyy}", customer.enrollmentDate);
            customerView.PhoneNumber = customerHistory.PhoneNumber;
            customerView.ProfileStatus = DisplayName.ProfileStatusName(customerHistory.IsLockedOut);
            customerView.IsLockedOut = Convert.ToBoolean(customerHistory.IsLockedOut);
            customerView.Reason = customerHistory.Reason;
            customerView.Remark = customerHistory.Remark;
            customerView.AccountNumber = customerHistory.AccountNumber;

            customerView.CustomerPhoneNumber = customerHistory.CallerNumber;
            customerView.ProfileStatus2 = DisplayNameFromEnum.GetProfileStatusName(customerHistory.IsLockedOut);


            if (customerHistory != null)
            {
                customerView.LastChangeBy = customerHistory.CreatedBy;
                customerView.LastChangeDate = string.Format("{0:dd/MM/yyyy HH:mm:ss tt}", customerHistory.CreatedDate);
                customerView.LastChangeReason = customerHistory.Reason;
            }

            customerView.Status = (CustomerStatus)customer.status;
            return PartialView("_editCustomerHistory", customerView);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileChangeEdit(CustomerView customerView)
        {
            Customers customer = new Customers();
            try
            {
                string status = string.Empty;
                string EnrollmentStatus2 = string.Empty;
                User user = await CurrentUser();

                if (ModelState.IsValid)
                {
                    CustomerHistory customerHistory = customerHistoryRepository.CustomerHistorys.Where(x => x.Id == customerView.Id).FirstOrDefault();

                    var oldData = JsonConvert.SerializeObject(customerHistory);

                    customerHistory.Status = "PP";
                    customerHistory.AccountNumber = customerHistory.AccountNumber;
                    customerHistory.CallerNumber = customerView.CustomerPhoneNumber;
                    customerHistory.Remark = customerView.Remark;
                    customerHistory.Reason = customerView.Reason;

                    await customerHistoryRepository.Update(customerHistory);


                    var action = $"User with staff id: {user.StaffId} approved  a customer change status profile";
                    var data = $"old data : {oldData}|new data:  {JsonConvert.SerializeObject(customerHistory)}";
                    await AddAudit(action, null, data);

                    _flash.Flash("success", $"Record was launched successufully");
                    return RedirectToAction("Index", new { Search = customer.PhoneNumber });


                }
            }
            catch (Exception er)
            {
                _flash.Flash("danger", $"Something went wrong, Cannot Update Customer request at this time");
                logger.LogError(string.Format("Error occured. ContactController : Index : {0}", er));
            }

            return RedirectToAction("Index", new { Search = customer.PhoneNumber });
        }

        public async Task<IActionResult> FAQ(string Search = "")
        {

            try
            {
                await InitiateContact();
                contactViewModel.PageBaseClass.FAQ = true;
                contactViewModel.PageBaseClass.AdminPageName = "";


                if (!string.IsNullOrEmpty(Search))
                {
                    return PartialView("_FAQ");
                }

            }
            catch (Exception er) { logger.LogError(string.Format("Error occured. ContactController : FAQ : {0}", er)); return RedirectToAction("Login", "Account"); }
            var userc = await CurrentUser();
            var action = $"User with staff id: {userc.StaffId} accessed FAQ page";
            await AddAudit(action);
            return View(contactViewModel);
        }

        private string GetCustomerDetail(CustomerDetail customer)
        {

            CustomerDetailAudit customerDetailAudit = new CustomerDetailAudit
            {
                phoneNumber = customer.phoneNumber,
                accountNumber = customer.accountNumber,
                isActive = customer.isActive,
                isLockedOut = customer.isLockedOut,
                name = customer.name,
                enrollmentDate = customer.enrollmentDate,
                status = customer.status,
                optInDate = customer.optInDate
            };
            return JsonConvert.SerializeObject(customerDetailAudit);
        }



    }
}