using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Services.DocumentReviewService;
using YellowStone.Services.DocumentReviewService.DTOs;

namespace YellowStone.Services.Mock
{
    public class MockDocumentReviewService : IDocumentReview
    {
        

        public Task<SendDocumentResponse> GetDocument(string phoneNumber, int documentType)
        {
            var document = new SendDocumentResponse
            {
                 AccountNumber="3445485758",
                 Document= new DocumentDTO { Extension="png",RawData=""},
                 DocumentType=DocumentType.IDENTIFICATION,
                 PhoneNumber="08066874723"
            };
            return Task.FromResult(document);
        }

        public Task<List<SendDocumentResponse>> GetDocumentsByPhoneNumber(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public string GetImage(string photoUrl, DocumentType documentType)
        {
            throw new NotImplementedException();
        }
 public Task<List<UnapprovedDocumentResponse>> GetUnApprovedDocumentHeaderOld()
        {

            var unApprovedDocuments = new List<UnapprovedDocumentResponse>()
            {
                new UnapprovedDocumentResponse
                {
                    AccountNumber = "5759948495",
                    CustomerName = "Yemi John Adewale",
                    PhoneNumber = "08066874723",
                   CreationDate = DateTime.Now
                },
                new UnapprovedDocumentResponse
                {
                    AccountNumber = "8846466445859",
                    CustomerName = "Chuks Beatrice",
                    PhoneNumber = "080556765555",
                    CreationDate = DateTime.Now
                }


            };

            return Task.FromResult(unApprovedDocuments);
        }

        public Task<List<UnapprovedDocumentResponse>> GetUnApprovedDocumentHeader()
        {

            var unApprovedDocuments = new List<UnapprovedDocumentResponse>()
            {
                new UnapprovedDocumentResponse
                {
                    AccountNumber = "5759948495",
                    CustomerName = "Yemi John Adewale",
                    PhoneNumber = "08066874723",
                   CreationDate = DateTime.Now
                },
                new UnapprovedDocumentResponse
                {
                    AccountNumber = "8846466445859",
                    CustomerName = "Chuks Beatrice",
                    PhoneNumber = "080556765555",
                    CreationDate = DateTime.Now
                }


            };

            return Task.FromResult(unApprovedDocuments);
        }

        public Task UpdateDocumentApprovalStatus(string phoneNumber, int documentType)
        {
            return Task.FromResult(true);
        }

        public Task UpdateDocumentApprovalStatus(string phoneNumber, int documentType, bool? status)
        {
            throw new NotImplementedException();
        }

        public bool ErrorOcurred()
        {
            throw new NotImplementedException();
        }
    }
}
