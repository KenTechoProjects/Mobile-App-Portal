using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.DocumentReviewService.DTOs
{
    public class UnapprovedDocumentResponse 
    {
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class EmptyDocumentResponseResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
    }
}
