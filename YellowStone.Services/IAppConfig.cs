using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YellowStone.Services
{
    public interface IAppConfig
    {
         string BasePath { get; set; }
    }
}
