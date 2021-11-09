using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Web.ViewModels
{
    public class CustomerDevicesViewModel : BaseViewModel
    {
        public IEnumerable<CustomerDevices> CustomerDevices { get; set; }
        public CustomerDevicesViewModel(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
            this.CustomerDevices = new List<CustomerDevices>();
        }

    }

    public class CustomerDevices
    {
        public long Id { get; set; }
        public string DeviceId { get; set; }
        public bool IsActive { get; set; }
        public string Model { get; set; }
        public long CustomerId { get; set; }
        public DateTime? DateCreated { get; set; }

    }

}
