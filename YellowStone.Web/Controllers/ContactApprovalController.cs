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
using Apollo.ViewModels;

namespace Apollo.Controllers
{
    [Authorize]
    public class ContactApprovalController : BaseController
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICustomerHistoryRepository customerHistoryRepository;

        private readonly IUserRepository userRepository;
        private readonly IWhatsAppCustomerService _whatsAppCustomerService;
        private User user = new User();
        private readonly ILogger logger;
        private readonly IAuthorizationService _authSvc;
        IFlasher _flash;
        private readonly IOptions<Settings> _options;

        ContactApproveViewModel contactApproveViewModel = new ContactApproveViewModel();

        public ContactApprovalController(IFlasher flash, IAuthorizationService _authSvc,
            IOptions<Settings> options, UserManager<User> userManager, ILogger<ContactApprovalController> logger,
            ICustomerHistoryRepository customerHistoryRepository, IUserRepository userRepository,
            IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor, IWhatsAppCustomerService whatsAppCustomerService) :
            base(userManager, options, auditTrailLog, accessor)
        {
            this.userRepository = userRepository;
            this.customerHistoryRepository = customerHistoryRepository;
            this.logger = logger;
            _whatsAppCustomerService = whatsAppCustomerService;
            _options = options;
            this._authSvc = _authSvc;
            _flash = flash;

        }

        private async Task InitiateContact()
        {

            var user = await CurrentUser();

            contactApproveViewModel.PageBaseClass = new Models.PageBaseClass();
            contactApproveViewModel.PageBaseClass.User = new Apollo.Models.User();
            contactApproveViewModel.PageBaseClass.User = user;
            contactApproveViewModel.PageBaseClass.ShowFAQ = true;

            contactApproveViewModel.CustomerHistoryViews = new List<CustomerHistoryView>();

            var pendingTransactions = customerHistoryRepository.CustomerHistorys.Where(x => x.Status == "PP").ToList();

            if (pendingTransactions != null && pendingTransactions.Count > 0)
                contactApproveViewModel.PageBaseClass.NotificationCount = pendingTransactions.Count;

        }

        private User GetUser()
        {
            return JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string Search = "", bool Error = false, bool Ajax = false, int? Page = 1, int PageSize = 20000)
        {

            var userc = await CurrentUser();
            try
            {

                await InitiateContact();
                contactApproveViewModel.PageBaseClass.InitiatorActivity = true;
                contactApproveViewModel.PageBaseClass.AdminPageName = "User Activities";
                contactApproveViewModel.PageBaseClass.ValidateUser("true");

                contactApproveViewModel.CustomerHistoryViews = new List<CustomerHistoryView>();
                List<CustomerHistoryView> customerHistoryViews = new List<CustomerHistoryView>();

                Customers customer = new Customers();
                CustomerHistory customerHistory = new CustomerHistory();
                List<CustomerHistory> customerHistories = new List<CustomerHistory>();

                contactApproveViewModel.CusInformationHistories = new List<CusInformationHistory>();
                CusInformationHistory cusInformationHistory = new CusInformationHistory();

                customerHistories = customerHistoryRepository.CustomerHistorys.OrderByDescending(x => x.CreatedDate).Take(100).ToList();
                var query = Search.Substring(Search.Length - 10);
                if (!string.IsNullOrEmpty(Search))
                    customerHistories = customerHistories.Where(x => x.AccountNumber == Search || x.ApprovedBy == Search || x.CreatedBy == Search || x.CustomerName == Search || EF.Functions.Like(x.PhoneNumber, $"%{query}")).ToList();

                if (customerHistories != null && customerHistories.Count > 0)
                {
                    foreach (var history in customerHistories)
                    {
                        CustomerHistoryView customerHistoryView = new CustomerHistoryView();

                        customerHistoryView.AccountNumber = history.AccountNumber;
                        customerHistoryView.ApprovedBy = history.ApprovedBy;
                        if (history.ApprovedDate != null)
                            customerHistoryView.ApprovedDate = string.Format("{0:dd/MM/yyyy}", history.ApprovedDate);

                        customerHistoryView.LastChangeBy = history.CreatedBy;
                        if (history.CreatedDate != null)
                            customerHistoryView.LastChangeDate = string.Format("{0:dd/MM/yyyy}", history.CreatedDate);

                        customerHistoryView.Name = history.CustomerName;
                        customerHistoryView.PhoneNumber = history.PhoneNumber;

                        contactApproveViewModel.CustomerHistoryViews.Add(customerHistoryView);

                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "There are no records found.";
                }


                if (Ajax)
                {
                    if (contactApproveViewModel.CustomerHistoryViews != null && contactApproveViewModel.CustomerHistoryViews.Count == 0)
                    {
                        ViewBag.ErrorMessage = "There are currently no records found";
                    }

                    return PartialView("_Index", contactApproveViewModel.CustomerHistoryViews);
                }

            }
            catch (Exception er)
            {
                logger.LogError(string.Format("Error occured. ContactController : Index : {0}", er));


            }

            //Audit log
            var action = $"User with staff id: {user.StaffId}  accessed contactApproval page";
            await AddAudit(action);
            ViewBag.user = userc;

            return View(contactApproveViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ApprovalNotification(string sortOrder, string currentFilter, string currentSearch, string fromDate = "", string toDate = "", string Search = "", string Status = "", bool Error = false, bool Ajax = false, int? Page = 1, int PageSize = 20000)
        {
            try
            {

                await InitiateContact();
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
                contactApproveViewModel.PageBaseClass.ApprovalNotification = true;
                contactApproveViewModel.PageBaseClass.AdminPageName = "";
                contactApproveViewModel.NotificationViews = new List<NotificationView>();
                List<CustomerHistory> customerHistories = new List<CustomerHistory>();


                string status = string.Empty;
                string EnrollmentStatus2 = string.Empty;

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

                        var customerDetail = CryptoEngine.Decrypt(cusHistory.CustomerDetails, _options.Value.CryptoKey);
                        CustomerDetailAudit customer = JsonConvert.DeserializeObject<CustomerDetailAudit>(customerDetail);
                        EnrollmentStatus(customer, ref status, ref EnrollmentStatus2);
                        notificationView.EnrollmentStatus = EnrollmentStatus2;

                        if (customer.enrollmentDate != null)
                            notificationView.EnrolledDate = string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", customer.enrollmentDate);

                        notificationView.AccountNumber = cusHistory.AccountNumber;
                        notificationView.ApprovedBy = cusHistory.ApprovedBy;

                        if (cusHistory.ApprovedDate != null)
                            notificationView.ApprovedDate = string.Format("{0:dd/MM/yyyy  hh:mm:ss tt}", cusHistory.ApprovedDate);

                        notificationView.CreatedBy = cusHistory.CreatedBy;

                        if (cusHistory.CreatedDate != null)
                            notificationView.CreatedDate = string.Format("{0:dd/MM/yyyy  hh:mm:ss tt}", cusHistory.CreatedDate);

                        notificationView.Name = cusHistory.CustomerName;
                        notificationView.PhoneNumber = cusHistory.PhoneNumber;

                        if (cusHistory.Status.Equals("PP"))
                            notificationView.ProfileStatus = "Pending Approval";
                        else if (cusHistory.Status.Equals("AA"))
                            notificationView.ProfileStatus = "Approved";
                        else if (cusHistory.Status.Equals("RR"))
                            notificationView.ProfileStatus = "Not Approved";

                        if (cusHistory.IsLockedOut == false)
                        {

                            notificationView.Status = "Awaiting Manager's Approval";
                            notificationView.Status2 = cusHistory.Status;
                        }
                        notificationView.PendingStatus = DisplayNameFromEnum.GetProfileStatusName(cusHistory.IsLockedOut);

                        notificationView.Remark = cusHistory.Remark;
                        notificationView.ID = cusHistory.Id;
                        notificationView.ManagerRemark = cusHistory.ManagerRemark;

                        contactApproveViewModel.NotificationViews.Add(notificationView);

                    }
                    contactApproveViewModel.NotificationViewsList = PaginatedList<NotificationView>.Create((IEnumerable<NotificationView>)contactApproveViewModel.NotificationViews, Page ?? 1, PageSize);

                }
                else
                    ViewBag.ErrorMessage = "There are currently no records.";


            }
            catch (Exception er)
            {
                logger.LogError(string.Format("Error occured. ContactController : Index : {0}", er));
                // return RedirectToAction("Login", "Account");
            }
            var user = await CurrentUser();
            ViewBag.user = user;

            //Audit log
            var action = $"User with staff id: {user.StaffId} accessed contact approval notification page";
            await AddAudit(action);
            return View(contactApproveViewModel);
        }

        private void EnrollmentStatus(CustomerDetailAudit customer, ref string status, ref string EnrollmentStatus2)
        {
            string result = string.Empty;

            if (CustomerStatus.ENROLLED.ToString() == customer.status.ToString())
            {
                status = string.Format("ENROLLED {1}", status, customer.enrollmentDate);
                EnrollmentStatus2 = "Enrolled";
            }
            else if (CustomerStatus.OPTED_IN.ToString() == customer.status.ToString())
            {
                status = string.Format("OPTED IN {1}", status, customer.optInDate);
                EnrollmentStatus2 = "Opted In";

            }
            else if (CustomerStatus.OPTED_OUT.ToString() == customer.status.ToString())
            {
                status = "OPTED OUT";
                EnrollmentStatus2 = "Opted Out";

            }
            else if (CustomerStatus.PRE_OPTIN.ToString() == customer.status.ToString())
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

        private async Task LoadData()
        {
            contactApproveViewModel.NotificationViews = new List<NotificationView>();
            List<CustomerHistory> customerHistories = new List<CustomerHistory>();
            NotificationView notificationView = new NotificationView();

            string status = string.Empty;
            string EnrollmentStatus2 = string.Empty;

            contactApproveViewModel.NotificationViews = new List<NotificationView>();
            customerHistories = customerHistoryRepository.CustomerHistorys.ToList();

            if (customerHistories != null && customerHistories.Count > 0)
            {
                foreach (var cusHistory in customerHistories)
                {
                    var customerDetail = CryptoEngine.Decrypt(cusHistory.CustomerDetails, cusHistory.PhoneNumber);
                    CustomerDetailAudit customer = JsonConvert.DeserializeObject<CustomerDetailAudit>(customerDetail);

                    EnrollmentStatus(customer, ref status, ref EnrollmentStatus2);
                    notificationView.EnrollmentStatus = EnrollmentStatus2;

                    if (customer.enrollmentDate != null)
                        notificationView.EnrolledDate = string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", customer.enrollmentDate);

                    notificationView.AccountNumber = cusHistory.AccountNumber;
                    notificationView.ApprovedBy = cusHistory.ApprovedBy;

                    if (cusHistory.ApprovedDate != null)
                        notificationView.ApprovedDate = string.Format("{0:dd/MM/yyyy  hh:mm:ss tt}", cusHistory.ApprovedDate);

                    notificationView.CreatedBy = cusHistory.CreatedBy;

                    if (cusHistory.CreatedDate != null)
                        notificationView.CreatedDate = string.Format("{0:dd/MM/yyyy  hh:mm:ss tt}", cusHistory.CreatedDate);

                    notificationView.Name = cusHistory.CustomerName;
                    notificationView.PhoneNumber = cusHistory.PhoneNumber;

                    if (cusHistory.Status.Equals("PP"))
                        notificationView.ProfileStatus = "Pending Approval";
                    else if (cusHistory.Status.Equals("AA"))
                        notificationView.ProfileStatus = "Approved";
                    else if (cusHistory.Status.Equals("RR"))
                        notificationView.ProfileStatus = "Not Approved";

                    if (cusHistory.IsLockedOut == false)
                    {
                        notificationView.PendingStatus = "Locked To Unlocked";
                        notificationView.Status = "Awaiting Manager's Approval";
                        notificationView.Status2 = cusHistory.Status;
                    }


                    notificationView.Remark = cusHistory.Remark;
                    notificationView.ID = cusHistory.Id;

                    contactApproveViewModel.NotificationViews.Add(notificationView);
                    notificationView = new NotificationView();
                }

            }
            else
            {
                ViewBag.ErrorMessage = "There are currently no records.";
            }
        }

        public async Task<IActionResult> ApproveProfileChange(string Status, string Message, long ID)
        {

            CustomerHistory customerHistory = new CustomerHistory();
            bool result = false;

            try
            {

                var user = await CurrentUser();
                customerHistory = await customerHistoryRepository.Single(x => x.Id == ID);


                var oldData = CryptoEngine.Decrypt(customerHistory.CustomerDetails, _options.Value.CryptoKey);

                if (!string.IsNullOrEmpty(Status) && Status.Equals("AA"))
                {

                    customerHistory.ApprovedBy = user.StaffId;
                    customerHistory.ApprovedDate = DateTime.Now;

                    customerHistory.ManagerRemark = Message;
                    customerHistory.IsLockedOut = false;
                    customerHistory.Status = "AA";
                    var response = await _whatsAppCustomerService.UpdateCustomerStatus(customerHistory.PhoneNumber, !customerHistory.IsLockedOut);
                    result = await customerHistoryRepository.UpdateCustomerProfileChange(customerHistory);
                    if (result)
                    {
                        //Audit log

                        var action = $"User with staff id: {user.StaffId} approved  a customer change status profile";
                        var data = $"old data : {oldData}|new data:  { JsonConvert.SerializeObject(new { PhoneNumber = customerHistory.PhoneNumber, Status = "Locked to Unlocked" })}";
                        await AddAudit(action, user, data);

                        _flash.Flash("success", "Customer Update status was approved");
                        return Content(customerHistory.PhoneNumber);
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(Status) && Status.Equals("RR"))
                    {
                        customerHistory.ManagerRemark = Message;
                        customerHistory.Status = "RR";
                        result = await customerHistoryRepository.UpdateCustomerProfileChange(customerHistory);
                        if (result)
                        {
                            //Audit log

                            var action = $"User with staff id: {user.StaffId} rejected  a customer change status profile";
                            var data = $"old data : {oldData}|new data:  {JsonConvert.SerializeObject(new { PhoneNumber = customerHistory.PhoneNumber, Status = "Locked" })}";
                            await AddAudit(action, user, data);
                            _flash.Flash("success", "Customer Update status was rejected");
                            return Content(customerHistory.PhoneNumber);
                        }
                    }

                }
            }
            catch (Exception er) { logger.LogError(string.Format("Error occured. ContactApprovalController : Index : {0}", er)); }

            return RedirectToAction("Index", new { Search = customerHistory.PhoneNumber });
        }

    }
}