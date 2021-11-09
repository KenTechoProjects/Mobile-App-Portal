using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Services.DocumentReviewService.DTOs;

namespace YellowStone.Web.ViewModels
{
    public class DocumentReviewViewModel : PageLayout
    {
       public List<UnapprovedDocumentResponse> UnapprovedDocumentResponses;
        public string AppBaseUrl { get; set; }
    }

    
}
