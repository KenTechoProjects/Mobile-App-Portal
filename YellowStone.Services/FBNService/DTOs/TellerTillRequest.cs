using System;
namespace YellowStone.Services.FBNService.DTOs
{
    public class TellerTillRequest
    {
        public string RequestId { get; set; }
        public string Currency { get; set; }
        public string Username { get; set; }
    }
}
