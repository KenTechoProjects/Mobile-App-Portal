using System;
using System.Threading.Tasks;
using YellowStone.Services.WalletService.DTOs;

namespace YellowStone.Services.Processors
{
    public interface IWalletService
    {
        Task<WalletEnquiryResponse> GetWalletAsync(string walletId);
    }
}
