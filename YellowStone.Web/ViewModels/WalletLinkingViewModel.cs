using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Web.ViewModels
{
    public class WalletLinkingViewModel : BaseViewModel
    {
      public  WalletInfoViewModel WalletInfoViewModel { get; set; }
      public AccountDetailsViewModel AccountDetailsViewModel { get; set; }

        public WalletLinkingViewModel(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
