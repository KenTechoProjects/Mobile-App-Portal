using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Services.DocumentReviewService.DTOs;

namespace YellowStone.Web.ViewModels
{
    public class DocumentViewModel:PageLayout
    {
        
        public string AccountNumber { get; set; } 
        public string Message { get; set; }
        public string CustomerName { get; set; }
        public string  PhoneNumber { get; set; }
        public string Comment { get; set; }
        [Required(ErrorMessage ="Document selection is required")]
        public int Document { get; set; }
        public string Action { get; set; }
        public int Status { get; set; }
        public DocumentType  EnumDocumentType { get; set; }
        public Aprovalstatus   Aprovalstatus { get; set; }
        public string Description { get; set; }

        public bool IsBranchApprover { get; set; } = false;
        public string ReviewerMessage { get; set; }
        public string AppBaseUrl { get; set; }

        public List<CustomerDocument> CustomerDocument { get; set; }
    }

    public class CustomerDocument
    {
        public int DocumentTypeId { get; set; }
        public string DocumentName { get; set; }
        public string PhoneNumber { get; set; }
        public Aprovalstatus? Status { get; set; }
    }
    public enum ApprovalLevel
    {
        First = 1,
        Second,

    }
    public enum Aprovalstatus
    {

        Reject = 0,
        Accept,

    }

}
