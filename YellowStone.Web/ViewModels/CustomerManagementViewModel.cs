using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models.Enums;

namespace YellowStone.Web.ViewModels
{
    public class CustomerManagementViewModel
    {
        public long? Id { get; set; }
        public string AccountNumber { get; set; }
        public string DeviceId { get; set; }
        public string DeviceModel { get; set; }
        public string AccountName { get; set; }
        public ActionReasons Reason { get; set; }
        public string Comment { get; set; }
        public string CallingPhone { get; set; }
        public RequestTypes RequestType  { get; set; }
}
}
