using Core.Flash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Controllers;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;
using YellowStone.Services;
using YellowStone.Services.DocumentReviewService;
using YellowStone.Services.DocumentReviewService.DTOs;
using YellowStone.Services.Processors;
using YellowStone.Web.ViewModels;

namespace YellowStone.Web.Controllers
{
    public class DocumentReviewController : BaseController
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IDocumentReview _documenReviewService;
        private readonly IUserRepository userRepository;

        private readonly IFbnMobileService _fbnMobileService;
        private readonly ILogger logger;
        private readonly IAuthorizationService _authSvc;
        private readonly SystemSettings _options;

        private readonly IRolePermissionRepository _rolepermissionRepository;
        private readonly IPermissionRepository _permissionRepository;

        private readonly IDocumentReviewRequestRepository _documentReviewRequestRepository;
        IFlasher _flash;
        DocumentReviewViewModel documentReviewViewModel;
        DocumentViewModel _documentViewModel;


        public DocumentReviewController(IFlasher flash, IAuthorizationService _authSvc, IOptions<SystemSettings> options,
            ICommentRepository commentRepository, UserManager<User> userManager, ILogger<OnboardingController> logger,
            IUserRepository userRepository,
            IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor, IFbnMobileService fbnMobileService,
           IDocumentReview documentReviewService, IDocumentReviewRequestRepository documentReviewRequestRepository, IRolePermissionRepository rolepermissionRepository, IPermissionRepository permissionRepository) :
            base(userManager, options, auditTrailLog, accessor, userRepository)
        {
            this.userRepository = userRepository;

            this._authSvc = _authSvc;
            this.logger = logger;
            _flash = flash;
            _options = options.Value;
            _documenReviewService = documentReviewService;
            _fbnMobileService = fbnMobileService;
            _rolepermissionRepository = rolepermissionRepository ?? throw new ArgumentNullException(nameof(rolepermissionRepository));

            _permissionRepository = permissionRepository;
            _documentReviewRequestRepository = documentReviewRequestRepository;
            documentReviewViewModel = new DocumentReviewViewModel();
            _documentViewModel = new DocumentViewModel();
            _documentViewModel.AppBaseUrl = _options.AppBaseUrl;
            documentReviewViewModel.AppBaseUrl = _options.AppBaseUrl;

        }
        private async Task<bool> CanInitiateUser()
        {
            var user = await CurrentUser();

            documentReviewViewModel.PageBaseClass = new PageBaseClass
            {
                User = user
            };

            //var user = await CurrentUser();
            var name = "InitiateProfileManagement";
            var perm = _permissionRepository.FindWhere(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();

            if (perm != null)
            {

                bool hasPermission = _rolepermissionRepository.CheckPermissionForUser(user, perm.Id);
                if (hasPermission)
                {
                    return true;
                }
            }
            return false;
        }
        private async Task<bool> CanApproveUser()
        {
            var user = await CurrentUser();
            var name = "ApproveProfileManagement";
            var perm = _permissionRepository.FindWhere(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();
            //var isApprover = ((await _authSvc.AuthorizeAsync(User, user, "ApproveProfileManagement")).Succeeded);
            if (perm != null)
            {
                bool hasPermission = _rolepermissionRepository.CheckPermissionForUser(user, perm.Id);
                if (hasPermission)
                {
                    return true;
                }
            }
            return false;
        }
        private void InitialiseDocumentReviewView(User user)
        {

            documentReviewViewModel.PageBaseClass = new PageBaseClass
            {
                User = user
            };

        }
        private void InitialiseDocumentViewModel(User user)
        {

            _documentViewModel.PageBaseClass = new PageBaseClass
            {
                User = user
            };

        }
        private void InitialiseDocumentViewModel2(User user, ref DocumentViewModel documentView)
        {

            documentView.PageBaseClass = new PageBaseClass
            {
                User = user
            };

        }
        [HttpGet]
        public async Task<IActionResult> GetDocumentReviews(string searchText)
        {
            logger.LogInformation("Inside the GetDocumentReviews action method of Document review Controller");
            var result = new List<UnapprovedDocumentResponse>();

            documentReviewViewModel.UnapprovedDocumentResponses = await _documenReviewService.GetUnApprovedDocumentHeader();
            //documentReviewViewModel.UnapprovedDocumentResponses = await _documenReviewService.GetUnApprovedDocumentHeaderOld();

            if (documentReviewViewModel.UnapprovedDocumentResponses.Count == 0)
            {
                logger.LogInformation("NO_DOC!!!!!!!!!!!!!!!!!!!!!!");
                return PartialView("_NoRecordFound");
            }

            if (string.IsNullOrEmpty(searchText))
            {
                result = documentReviewViewModel.UnapprovedDocumentResponses;
                return PartialView("_DocumentReviewRecord", result);
            }

            result = documentReviewViewModel.UnapprovedDocumentResponses.Where(p => p.AccountNumber.ToLower().Contains(searchText.ToLower()) || p.PhoneNumber.ToLower().Contains(searchText.ToLower()) || p.CustomerName.ToLower().Contains(searchText.ToLower())).Select(p => p).ToList();
            if (result.Count == 0 || result == null)
                return PartialView("_NoRecordFound");

            return PartialView("_DocumentReviewRecord", result);
        }


        private async Task<List<UnapprovedDocumentResponse>> FilterDocumentListByUserRole(List<UnapprovedDocumentResponse> unapprovedDocumentResponses)
        {
            logger.LogInformation("Inside the FilterDocumentListByUserRole method of DocumentreviewController");
            var customers = new List<UnapprovedDocumentResponse>();
            var currentUser = await CurrentUser();
            var isApprover = ((await _authSvc.AuthorizeAsync(User, currentUser, "ApproveProfileManagement")).Succeeded);

            if (isApprover)
            {
                logger.LogInformation("User can approve this record level is 2");
                foreach (var customer in unapprovedDocumentResponses)
                {
                    var customerDocuments = _documentReviewRequestRepository.GetCustomerDocuments(customer.PhoneNumber, 2);
                    logger.LogInformation("Approval level is 2");
                    if (customerDocuments.Count >= 1)
                    {

                        customers.Add(customer);
                    }
                    logger.LogInformation($"{customerDocuments.Count} Records added");
                }
            }
            else
            {
                logger.LogInformation("User is not an approver this record level is 1");
                foreach (var customer in unapprovedDocumentResponses)
                {
                    var customerDocuments = _documentReviewRequestRepository.GetCustomerDocuments(customer.PhoneNumber, 1);//no document exist at all for the customer
                    if (customerDocuments.Count <= 0 || customerDocuments.Count >= 1)
                    {
                        customers.Add(customer);
                    }
                    //add customer if a document has not gotten initiator approval
                }
            }
            return customers;


            //if (await CanInitiateUser())
            //    {
            //            foreach (var customer in unapprovedDocumentResponses)
            //            {
            //              var customerDocuments = _documentReviewRequestRepository.GetCustomerDocuments(customer.PhoneNumber,1);//no document exist at all for the customer
            //               if(customerDocuments.Count <= 0  || customerDocuments.Count >= 1 )
            //                {
            //                 customers.Add(customer);
            //                }
            //               //add customer if a document has not gotten initiator approval
            //            }
            //    }

            //if (await CanApproveUser())
            //{
            //    foreach (var customer in unapprovedDocumentResponses)
            //    {
            //        var customerDocuments = _documentReviewRequestRepository.GetCustomerDocuments(customer.PhoneNumber, 2);//no document exist at all for the customer
            //        if (customerDocuments.Count >= 1)
            //        {
            //            customers.Add(customer);
            //        }

            //    }
            //}
            //return customers;
        }


        [HttpGet]
        public async Task<IActionResult> DocumentReviewList()
        {
            logger.LogInformation($"Inside the  DocumentReviewList of DocumentReviewController");
            var user = await CurrentUser();
            documentReviewViewModel.PageBaseClass = new PageBaseClass
            {
                User = user
            };
            ViewBag.CurrentUser = user?.StaffId;

            InitialiseDocumentReviewView(user);
            ViewBag.user = user;

            var customerDocumentFromService = await _documenReviewService.GetUnApprovedDocumentHeader();
            logger.LogInformation($"Records gotten====> {JsonConvert.SerializeObject(customerDocumentFromService)}");
            //var customerDocumentFromService = await _documenReviewService.GetUnApprovedDocumentHeaderOld();

            if (customerDocumentFromService.Count > 0)
            {
                logger.LogInformation("Document records available");
                documentReviewViewModel.UnapprovedDocumentResponses = await FilterDocumentListByUserRole(customerDocumentFromService);
                logger.LogInformation($"Un approved Records gotten====> {JsonConvert.SerializeObject(documentReviewViewModel.UnapprovedDocumentResponses)}");
            }
            else
            {
                logger.LogInformation("No available document record");
                documentReviewViewModel.UnapprovedDocumentResponses = new List<UnapprovedDocumentResponse>();
                //return View(documentReviewViewModel.UnapprovedDocumentResponses);
                //return View("NoDocumentReviewRecordSummary");
                return View(documentReviewViewModel);
            }


            //ViewBag.user = user;
            var action = $"accessed doucment review list page";
            await AddAudit(action, user);

            return View(documentReviewViewModel);
        }

        public IActionResult NoDocumentReviewRecordSummary()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewDocument(string phoneNumber)
        {
            logger.LogInformation($"Inside the ViewDocument of DocumentReviewController");
            var currentUser = await CurrentUser();
            var isApprover = ((await _authSvc.AuthorizeAsync(User, currentUser, "ApproveProfileManagement")).Succeeded);
            ViewBag.CurrentUser = currentUser?.StaffId;
            _documentViewModel.ReviewerMessage = "Initiator reviewing document";
            InitialiseDocumentViewModel(currentUser);
            var customer = await _documenReviewService.GetUnApprovedDocumentHeader();

            var customerInfo = customer.Where(p => p.PhoneNumber == phoneNumber).FirstOrDefault();
            if (customerInfo != null)
            {
                _documentViewModel.AccountNumber = customerInfo.AccountNumber;
                _documentViewModel.PhoneNumber = customerInfo.PhoneNumber;
                _documentViewModel.CustomerName = customerInfo.CustomerName;
            }

            //if (await CanApproveUser())
            //if (isApprover)
            //{
            //    _documentViewModel.IsBranchApprover = true;
            //    _documentViewModel.ReviewerMessage = "Branch Approver reviewing document";
            //}


            if (isApprover == true)
            {

                logger.LogInformation("User is an Approver");
                _documentViewModel.IsBranchApprover = true;
                _documentViewModel.ReviewerMessage = "Branch Approver reviewing document";
                _documentViewModel.CustomerDocument = new List<CustomerDocument>();
                var customerDoc = _documentReviewRequestRepository.GetCustomerDocuments(phoneNumber, 2);
                foreach (var item in customerDoc)
                {
                    foreach (int i in Enum.GetValues(typeof(DocumentType)))
                    {
                        if (item.DocumentType == i)
                        {
                            _documentViewModel.CustomerDocument.Add(new CustomerDocument { DocumentTypeId = i, DocumentName = Enum.GetName(typeof(DocumentType), i) });
                        }
                    }
                }

                logger.LogInformation($"Records {JsonConvert.SerializeObject(_documentViewModel.CustomerDocument)}");
            }
            else
            {
                var getdocumentsFromService = await GetCustomerDocument(phoneNumber);//from service
                logger.LogInformation($"GetCustomerDocument result=>{JsonConvert.SerializeObject(getdocumentsFromService)}");
                if (getdocumentsFromService.Count > 0)
                {
                    //Added newly on 30-10-2021
                    if (getdocumentsFromService.Any(p => p.DocumentTypeId == 0))
                    {

                        getdocumentsFromService = getdocumentsFromService.Select(p => new CustomerDocument
                        {
                            DocumentName = p.DocumentName,
                            DocumentTypeId = p.DocumentTypeId + 1,
                            PhoneNumber = p.PhoneNumber

                        }).ToList();

                    }
                    _documentViewModel.CustomerDocument = new List<CustomerDocument>();
                    foreach (var item in getdocumentsFromService)
                    {



                        var doc = await _documentReviewRequestRepository.GetDocument(item.PhoneNumber, item.DocumentTypeId);


                        if (doc == null || (doc.Status == 0 && doc.CurrentApprovalLevel == 1))
                        {
                            foreach (int i in Enum.GetValues(typeof(DocumentType)))
                            {


                                if (item.DocumentTypeId == i)
                                {
                                    _documentViewModel.CustomerDocument.Add(new CustomerDocument { DocumentTypeId = i, DocumentName = Enum.GetName(typeof(DocumentType), i), PhoneNumber = item.PhoneNumber });
                                }
                            }
                        }
                    }
                }


            }

            //if (await CanInitiateUser())
            //{
            //    var getdocumentsFromService = await GetCustomerDocument(phoneNumber);//from service
            //    if(getdocumentsFromService.Count > 0)
            //        foreach (var item in getdocumentsFromService)
            //        {
            //            _documentViewModel.CustomerDocument = new List<CustomerDocument>();
            //            var doc= await  _documentReviewRequestRepository.GetDocument(item.PhoneNumber, item.DocumentTypeId);
            //            if(doc== null ||(doc.Status == 0 && doc.CurrentApprovalLevel == 1))
            //            {
            //                foreach (int i in Enum.GetValues(typeof(DocumentType)))
            //                {
            //                    if (item.DocumentTypeId == i)
            //                        _documentViewModel.CustomerDocument.Add(new CustomerDocument { DocumentTypeId = i, DocumentName = Enum.GetName(typeof(DocumentType), i) });
            //                }
            //            }
            //        }
            //}

            //if (await CanApproveUser())
            //{
            //    _documentViewModel.CustomerDocument = new List<CustomerDocument>();

            //    var customerDoc =  _documentReviewRequestRepository.GetCustomerDocuments(phoneNumber, 2);

            //    foreach (var item in customerDoc)
            //    {

            //        foreach (int i in Enum.GetValues(typeof(DocumentType)))
            //        {
            //            if (item.DocumentType == i)
            //                _documentViewModel.CustomerDocument.Add(new CustomerDocument { DocumentTypeId = i, DocumentName = Enum.GetName(typeof(DocumentType), i) });
            //        }

            //    }
            //}
            //Used to set the viewDocument image element to nothing at first launch of the page
            ViewBag.NewLunch = true;
            ViewBag.user = currentUser;
            var action = $"accessed document view page";
            await AddAudit(action, currentUser);
            _documentViewModel.Description = $"Document approving window for {customerInfo.CustomerName} with Phone Number: {customerInfo.PhoneNumber}";
            logger.LogInformation($"Viewed record on viewDocument methd result=>{JsonConvert.SerializeObject(_documentViewModel)}");
            // _documentViewModel.CustomerDocument.Where(p => p.Status != Aprovalstatus.Accept).ToList();
            return View(_documentViewModel);
        }

        private async Task<List<CustomerDocument>> GetCustomerDocument(string phoneNumber)
        {

            logger.LogInformation($"Inside the GetCustomerDocument of DocumentReviewController . Phone number for search {phoneNumber}");
            var customerDocuments = new List<CustomerDocument>();
            var getCustomerDoc = await _documenReviewService.GetDocumentsByPhoneNumber(phoneNumber);

            //Reconsider
            //if (getCustomerDoc.Any(p => p.DocumentType == 0) == true)
            //{
            //    getCustomerDoc = getCustomerDoc.Select(p => new SendDocumentResponse { AccountNumber=p.AccountNumber, Document= p.Document, DocumentType=(p.DocumentType+1), Image=p.Image, Path=p.Path, PhoneNumber=p.PhoneNumber}).ToList();
            //}


            logger.LogInformation($"CUSTOMER_DOC_DROPDOWN:{JsonConvert.SerializeObject(getCustomerDoc)}");
            foreach (var doc in getCustomerDoc)

            {

                customerDocuments.Add(new CustomerDocument
                {
                    DocumentTypeId = (int)doc.DocumentType,
                    DocumentName = doc.DocumentType.ToString(),
                    PhoneNumber = doc.PhoneNumber//, Status = (Aprovalstatus)doc.Status
                });
            }
            return customerDocuments;
        }

        [HttpGet]
        public async Task<IActionResult> GetInitiatorComment(string phoneNumber, int docType)
        {
            var initiatorComment = string.Empty;
            var document = await _documentReviewRequestRepository.GetDocument(phoneNumber, docType);
            if (document != null)
            {
                initiatorComment = document.InitiatorComment;
            }

            return Json(initiatorComment);

        }

        [HttpGet]
        [EnableCors]
        public async Task<IActionResult> GetDocumentOld(string phoneNumber, int docType)
        {
            var finalDocumentPath = string.Empty;
            var documentServiceResult = await _documenReviewService.GetDocument(phoneNumber, docType);

            if (documentServiceResult == null)
            {
                return Json(finalDocumentPath);
            }

            //string[] splitedPath = documentServiceResult.Path.Split(@"\");
            //if(splitedPath.Length > 0)
            //{
            //    var docPath = $"{splitedPath[splitedPath.Length - 2]}/{splitedPath[splitedPath.Length - 1]}";
            //    logger.LogInformation($"DOC_PATH: {docPath}");
            //    finalDocumentPath = $"{_options.DocumentBaseUrl}{docPath}";
            //    logger.LogInformation($"FINAL_DOC_PATH: {docPath}");
            //}

            // return Json(finalDocumentPath);
            return Json(documentServiceResult.Image);
        }

        [HttpGet]
        // [EnableCors]
        public async Task<IActionResult> GetDocument(string phoneNumber, int docType)
        {
            logger.LogInformation($"Inside the GetDocument of the DocumentreviewController .PhoneNumber={phoneNumber} documentType={docType}");
            var finalDocumentPath = string.Empty;
            var documentServiceResult = await _documenReviewService.GetDocument(phoneNumber, docType);

            if (documentServiceResult == null)
            {
                // ViewBag.Imagepath = finalDocumentPath;
                return Json(finalDocumentPath);
            }

            //string[] splitedPath = documentServiceResult.Path.Split(@"\");
            //if(splitedPath.Length > 0)
            //{
            //    var docPath = $"{splitedPath[splitedPath.Length - 2]}/{splitedPath[splitedPath.Length - 1]}";
            //    logger.LogInformation($"DOC_PATH: {docPath}");
            //    finalDocumentPath = $"{_options.DocumentBaseUrl}{docPath}";
            //    logger.LogInformation($"FINAL_DOC_PATH: {docPath}");
            //}

            // return Json(finalDocumentPath);
            //ViewBag.Imagepath = documentServiceResult.Image;
            return Json(new { Image = documentServiceResult.Image, ErrorOcurred = documentServiceResult.ErrorOcurred });
        }

        public IActionResult DocumentPartialView(string image)
        {
            ViewBag.Imagepath = image;
            return PartialView();
        }


        [HttpGet]
        public async Task<IActionResult> GetDocumentApprovalStatus(string phoneNumber, int docType)
        {
            logger.LogInformation($"Inside GetDocumentApprovalStatus PhoneNo={phoneNumber} DocumentType={docType}");
            var currentUser = CurrentUser();
            var isApprover = ((await _authSvc.AuthorizeAsync(User, currentUser, "ApproveProfileManagement")).Succeeded);

            var document = await _documentReviewRequestRepository.GetDocument(phoneNumber, docType);
            var isDocApproved = false;
            if (document == null)
            {
                logger.LogInformation("Document not approved");
                isDocApproved = false;
                return Json(isDocApproved);

            }
            //if (document.Status == (int)Aprovalstatus.Accept && await CanInitiateUser() && document.CurrentApprovalLevel== 1)
            if (document.Status == (int)Aprovalstatus.Accept && !isApprover && document.CurrentApprovalLevel == 1)
            {
                logger.LogInformation("Document is approved");
                isDocApproved = true;
            }
            //if (document.Status == (int)Aprovalstatus.Accept && await CanApproveUser() && document.CurrentApprovalLevel == 2)
            if (document.Status == (int)Aprovalstatus.Accept && isApprover && document.CurrentApprovalLevel == 2)
            {
                logger.LogInformation("Document is approved");
                isDocApproved = true;
            }
            return Json(new { approved = isDocApproved, sentforapproval = document.CurrentApprovalLevel == 2 });
        }

        [HttpGet]
        public async Task<IActionResult> SaveDocumentRequest(string phoneNumber, string acctNumber, int docType, string docAction, string docComment)
        {
            logger.LogInformation($"Inside the SaveDocumentRequest method of DocumentReviewController");
            string taskLog = "";
            var currentUser = await CurrentUser();
            var isApprover = ((await _authSvc.AuthorizeAsync(User, currentUser, "ApproveProfileManagement")).Succeeded);

            var approvalAction = Convert.ToInt32(docAction);

            var document = await _documentReviewRequestRepository.GetDocument(phoneNumber, docType);


            var isDocumentRejected = approvalAction == (int)Aprovalstatus.Reject ? true : false;
            var documentStatus = approvalAction == (int)Aprovalstatus.Accept ? true : false;
          bool? documentStatusExternal = false ;
            if (document == null && !isApprover)
            {
                var documentRequest = new DocumentReviewRequest
                {
                    AccountNumber = acctNumber,
                    PhoneNumber = phoneNumber,
                    InitiatorComment = docComment,
                    ReviewedBy = currentUser.StaffId,
                    DateFirstActedOn = DateTime.Now,
                    DateLastActedOn = DateTime.Now,
                    DocumentType = docType,
                    Status = approvalAction,
                    CurrentApprovalLevel = 1,

                };

                var isAdded = await _documentReviewRequestRepository.AddDocument(documentRequest);
                logger.LogInformation($"DOCUMENT_SUBMITTED1111111111111111111111111 Result==>  {isAdded}");
                if (approvalAction == (int)Aprovalstatus.Accept)
                {
                    taskLog = $"Document sent for approval";
                    documentRequest.CurrentApprovalLevel = 2;
                    documentRequest.Status = 0;
                    _documentReviewRequestRepository.UpdateDocumentStatus(documentRequest);
                    logger.LogInformation($"DOCUMENT Updated: approval action =accepted. Result==>  {isAdded}, Level=>{ documentRequest.CurrentApprovalLevel}, Status=> { documentRequest.Status}");
                }
                else
                {
                    taskLog = $"Document rejected";
                    logger.LogInformation("Document was rejected");
                     
                        documentStatusExternal = null;
                     
                    await _documenReviewService.UpdateDocumentApprovalStatus(phoneNumber, docType, documentStatusExternal);
                }
            }
            else if (document != null && !isApprover)
            {
                var currentLevel = document.CurrentApprovalLevel;
                document.InitiatorComment = docComment;
                document.ReviewedBy = currentUser.StaffId;
                document.DateLastActedOn = DateTime.Now;
                if (approvalAction == (int)Aprovalstatus.Accept)
                {
                    document.Status = 0;
                    if (currentLevel == 2)
                    {
                        taskLog = $"Document already sent for approval";
                    }
                    else
                    {
                        taskLog = $"Document sent for approval";
                    }

                    document.CurrentApprovalLevel = 2;
                    documentStatusExternal = null;

                    logger.LogInformation($"DOCUMENT   Result==>  {taskLog }, Level=>{  document.CurrentApprovalLevel}, Status=> {  document.Status}");
                }
                else
                {
                    document.CurrentApprovalLevel = 1;
                    document.Status = 0;
                    taskLog = $"Document rejected"; 
                    documentStatusExternal = false;
                    logger.LogInformation($"DOCUMENT rejected Result==>  {taskLog }, Level=>{ document.CurrentApprovalLevel}, Status=> { document.Status}");
                }

                _documentReviewRequestRepository.UpdateDocumentStatus(document);
                 
                //call mobile app service to log document status decision
                await _documenReviewService.UpdateDocumentApprovalStatus(phoneNumber, docType, documentStatusExternal);
                logger.LogInformation($"DOCUMENT APPROVED FINALLY999999999999999999 {JsonConvert.SerializeObject(document)}");
            }
            else // for the approval
            {
                string me = string.Empty;
                document.BranchApproverComment = docComment;
                document.ApprovedBy = currentUser.StaffId;
                document.DateLastActedOn = DateTime.Now;
                if (approvalAction == (int)Aprovalstatus.Accept)
                {
                    documentStatusExternal = true;
                    document.Status = 1;
                    taskLog = $"Document approved finally";
                    me = $"starting to approve the document by {currentUser?.Name}";
                }
                else
                {
                    documentStatusExternal = false;
                    document.CurrentApprovalLevel = 1;
                    document.Status = 0;
                    taskLog = $"Document rejected";
                    me = $"starting to reject the document by {currentUser?.Name}";
                }
                logger.LogInformation(me);
                _documentReviewRequestRepository.UpdateDocumentStatus(document);
      
                //call mobile app service to log document status decision
                await _documenReviewService.UpdateDocumentApprovalStatus(phoneNumber, docType, documentStatusExternal);
                logger.LogInformation($"DOCUMENT APPROVED FINALLY999999999999999999 {JsonConvert.SerializeObject(document)}");
            }


            //if (document != null )
            //{
            //    if(await CanInitiateUser() && document.CurrentApprovalLevel == 1)
            //    {
            //        document.Status = approvalAction;
            //        document.ReviewedBy = currentUser.StaffId;
            //        document.InitiatorComment = docComment;

            //        if (isDocumentRejected)
            //        {
            //            document.CurrentApprovalLevel = 1;

            //            taskLog = $"Document rejected";

            //        }
            //        else
            //        {
            //            document.CurrentApprovalLevel = 2;
            //            document.Status = 0;
            //            taskLog = $"Document sent for approval";
            //            _documentReviewRequestRepository.UpdateDocumentStatus(document);

            //        }
            //        await _documenReviewService.UpdateDocumentApprovalStatus(phoneNumber, docType, documentStatus);
            //    }

            //    if (await CanApproveUser() && document.CurrentApprovalLevel == 2)
            //    {
            //        document.BranchApproverComment = docComment;
            //        document.ApprovedBy = currentUser.StaffId;
            //        if (isDocumentRejected)
            //        {
            //            document.Status = 0;
            //            taskLog = $"Document rejected";
            //        }
            //        else
            //        {
            //            document.Status = 1;
            //            taskLog = $"Document submitted successfully";
            //        }
            //        _documentReviewRequestRepository.UpdateDocumentStatus(document);
            //        await _documenReviewService.UpdateDocumentApprovalStatus(phoneNumber, docType, documentStatus);
            //    }

            //}

            var action = $"document view approval transaction successful";
            await AddAudit(action, currentUser);

            return Json(taskLog);
        }
        public IActionResult DocumentView()
        {
            return View();
        }
    }
}
