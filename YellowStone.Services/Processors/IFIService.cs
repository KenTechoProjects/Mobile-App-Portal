using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models.DTO;
using YellowStone.Services.FIService.DTOs;

namespace YellowStone.Services.Processors
{
   public interface IFIService
    {
        Task<AccountDetailsResponse> GetAccountDetailsAsync(string accountNumber);

     //   Task<TransferResponse> TransferAsync(TransferRequest request);

    }
}
