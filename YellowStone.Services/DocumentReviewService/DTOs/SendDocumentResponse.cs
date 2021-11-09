using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.DocumentReviewService.DTOs
{
   public class SendDocumentResponse
    {
        public string AccountNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Path { get; set; }
        public string Image { get; set; }
        public DocumentDTO Document { get; set; }
        public DocumentType DocumentType { get; set; }
        public bool ErrorOcurred { get; set; }
        public  DocumentStatus Status { get; set; }
    }
    public class DocumentDTO
    {
        public string RawData { get; set; }
        public string Extension { get; set; }
    }

    public enum DocumentType
    {
        PICTURE = 1,
        IDENTIFICATION,
        SIGNATURE,
        UTILITY_BILL
    }
    public enum DocumentStatus
    {
        Rejected,Accepted
    }
}
