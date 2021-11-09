using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FIService.DTOs
{
    public class TransferResponse : BaseResponse
    {
        public string Reference { get; set; }
        public DateTime Date { get; set; }
    }

}
