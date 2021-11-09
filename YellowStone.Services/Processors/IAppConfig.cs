using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YellowStone.Services.Interfaces
{
    public interface IAppConfig
    {
        Task<string> GetBaseUrl();
    }
}
