using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using YellowStone.Models;

namespace YellowStone.Web.ViewModels
{
    public class PageBaseClass
    {
        public bool Roles { get; set; } = false;
        public bool Resources { get; set; } = false;
        public bool Departments { get; set; } = false;
        public bool Users { get; set; } = false;
        public bool AuditLogs { get; set; } = false;

        public bool ApproveRoles { get; set; } = false;
        public bool ApproveDepartments { get; set; } = false;
        public bool ApproveUsers { get; set; } = false;
        public bool ViewAuditTrail { get; set; } = false;
        public bool SuperAdmin { get; set; } = false;

        public bool Initiator { get; set; } = false;
        public bool InitiatorNotification { get; set; } = false;
        public bool InitiatorActivity { get; set; } = false;
        public bool ApprovalNotification { get; set; } = false;

        public bool FAQ { get; set; } = false;
        public bool ShowFAQ { get; set; } = false;

        public int NotificationCount { get; set; } = 0;

        public string LoginID { get; set; }
        public string ErrorMessage { get; set; }
        public bool ShowSaveBtn { get; set; } = true;
        public string AdminFooterText { get; set; } = string.Format("Powered By FBN Digital Lab. Copyright © {0} FBN Digital Lab. All Rights Reserved", DateTime.Now.Year.ToString());
        public string AdminPageName { get; set; }
        public PermissionModel Permission { get; set; }
        public List<Tuple<string, string, string, object>> Breadcrumbs { get; set; }
        public User User { get; set; }

        public void ValidateUser(string Allow)
        {
            //if (Allow.ToLower() != "true")
            //    throw new Exception();
        }

    }
}
