using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FBNMobile.DTOs
{
    public class DeviceResponse
    {
        public bool IsSuccessful { get; set; }
        public IEnumerable<Device> Devices{ get; set; }


        public DeviceResponse(bool isSuccessful)
        {
            this.IsSuccessful = isSuccessful;
        }
    }

    public class Device
    {
        public long Id { get; set; }
        public string DeviceId { get; set; }
        public bool IsActive { get; set; }
        public string Model { get; set; }
        public long Customer_Id { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
