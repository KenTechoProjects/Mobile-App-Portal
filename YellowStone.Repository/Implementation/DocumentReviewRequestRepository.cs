using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;
using System.Linq;
using YellowStone.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace YellowStone.Repository.Implementation
{
    public class DocumentReviewRequestRepository : Repository<DocumentReviewRequest>,IDocumentReviewRequestRepository
    {
        private readonly FBNMDashboardContext context;
        public DocumentReviewRequestRepository(FBNMDashboardContext context):base(context)
        {
            this.context = context;
        }
       public async Task<bool> AddDocument(DocumentReviewRequest documentReviewRequest)
        {
            context.DocumentReviewRequests.Add(documentReviewRequest);
           var result=  await  context.SaveChangesAsync();
            return result > 0 ? true : false;
        }

        public async Task<DocumentReviewRequest> GetDocument(string phoneNumber,int documentType)
        {
             var document = await context.DocumentReviewRequests.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber && p.DocumentType == documentType);
             return document;
        }

        public async Task<DocumentReviewRequest> GetCustomerByPhoneNo(string phoneNumber)
        {
            var document = await context.DocumentReviewRequests.FirstOrDefaultAsync(p =>p.PhoneNumber == phoneNumber);
            return document;
        }

        public  void UpdateDocumentStatus(DocumentReviewRequest documentReviewRequest)
        {  
           
            var document =  context.DocumentReviewRequests.FirstOrDefault(p => p.PhoneNumber == documentReviewRequest.PhoneNumber && p.DocumentType == documentReviewRequest.DocumentType);

            if (documentReviewRequest.CurrentApprovalLevel == 1)
            {
                document.InitiatorComment = documentReviewRequest.InitiatorComment;
            }
            if (documentReviewRequest.CurrentApprovalLevel == 2)
            {
                document.BranchApproverComment = documentReviewRequest.BranchApproverComment;
            }
            document.Status = documentReviewRequest.Status;
            document.CurrentApprovalLevel = documentReviewRequest.CurrentApprovalLevel;
            document.DateLastActedOn = DateTime.Now;
            context.DocumentReviewRequests.Update(document);
            context.SaveChanges();
          
        }

        public List<DocumentReviewRequest> GetCustomerDocuments(string phoneNumber, int approvalLevel)
        {
            var documents = context.DocumentReviewRequests.Where(p => p.PhoneNumber == phoneNumber && p.CurrentApprovalLevel == approvalLevel && p.Status == 0).ToList();
            return documents;
        }


        public List<DocumentReviewRequest> GetCustomerDocumentsByApprovalLevel(int approvalLevel)
        {
            var documents = context.DocumentReviewRequests.Where(p => p.CurrentApprovalLevel == approvalLevel && p.Status == 0).ToList();
            return documents;
        }
    }
}
