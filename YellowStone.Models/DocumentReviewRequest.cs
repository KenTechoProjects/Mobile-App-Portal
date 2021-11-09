using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YellowStone.Models
{
    public  class DocumentReviewRequest
    {
        
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public int DocumentType { get; set; }
        public string ReviewedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime DateFirstActedOn { get; set; }
        public DateTime DateLastActedOn { get; set; }
        public string InitiatorComment { get; set; }
        public string BranchApproverComment { get; set; }
        public int CurrentApprovalLevel { get; set; }
        public int Status { get; set; }
        //public WhomAmI  WhomAmI { get; set; }
    }
    public enum WhomAmI
    {
        Initiator=1,Approver=2
    }
}
