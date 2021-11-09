using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Repository.Interfaces
{
    public interface IDocumentReviewRequestRepository
    {
        Task<DocumentReviewRequest> GetDocument(string phoneNumber,int documentType);
        List<DocumentReviewRequest> GetCustomerDocuments(string phoneNumber,int approvalLevel);
        Task<DocumentReviewRequest> GetCustomerByPhoneNo(string phoneNumber);
        Task<bool> AddDocument(DocumentReviewRequest documentReviewRequest);
        void UpdateDocumentStatus(DocumentReviewRequest documentReviewRequest);
      

    }
}
