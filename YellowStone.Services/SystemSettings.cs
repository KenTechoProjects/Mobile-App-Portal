using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services
{
    public class SystemSettings
    {
        public string BasePath { get; set; }
        public string SuperAdminStaffId { get; set; }
        public string CountryId { get; set; }
        public int MaxLoginCount { get; set; }
        public string[] DefaultDepartments { get; set; }
        public string AttachmentsServerPath { get; set; }
        public string ArchiveServerPath { get; set; }
        public int InactiveDays { get; set; }
        public string AppBaseUrl { get; set; }
        public string DocumentBaseUrl { get; set; }
        public string DocumentReturnedBaseUrl { get; set; }
        public string PhotoReturnedBaseUrl { get; set; }
        public bool IsTest { get; set; }
    }

   

}
