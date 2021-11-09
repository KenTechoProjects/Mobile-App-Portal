using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Web.ViewModels
{
    public class NotificationView
    {
        public long ID { get; set; }
        public string Name { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        [DisplayName("Profile Status")]
        public string ProfileStatus { get; set; }
        [DisplayName("Approved By")]
        public string ApprovedBy { get; set; }
        [DisplayName("Approved Date")]
        public string ApprovedDate { get; set; }


        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string EnrolledDate { get; set; }
        public string EnrollmentStatus { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Status2 { get; set; }
        public string PendingStatus { get; set; }
        public string Remark { get; set; }
        public string ManagerRemark { get; set; }

    }
}
