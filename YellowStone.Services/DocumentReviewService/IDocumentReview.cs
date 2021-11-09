using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using YellowStone.Services.DocumentReviewService.DTOs;

namespace YellowStone.Services.DocumentReviewService
{
    public interface IDocumentReview
    {
        Task<List<UnapprovedDocumentResponse>> GetUnApprovedDocumentHeader();
        Task<List<UnapprovedDocumentResponse>> GetUnApprovedDocumentHeaderOld();
        Task<SendDocumentResponse> GetDocument(string phoneNumber, int documentType);
     
        Task UpdateDocumentApprovalStatus(string phoneNumber, int documentType,bool? status);
        Task<List<SendDocumentResponse>> GetDocumentsByPhoneNumber(string phoneNumber);
        string GetImage(string photoUrl,DocumentType documentType);
        bool ErrorOcurred();

    }
}
