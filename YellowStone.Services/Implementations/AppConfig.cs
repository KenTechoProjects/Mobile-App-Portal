using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YellowStone.Services.Implementations
{
    public class AppConfig : IAppConfig
    {
        private readonly SystemSettings systemSettings;

        public AppConfig(IOptions<SystemSettings> systemSettings)
        {
            this.systemSettings = systemSettings.Value;
        }

        public string BasePath { get => this.systemSettings.BasePath; set => value = this.systemSettings.BasePath; }

        //public string GetBasePath()
        //{
        //    return systemSettings.BasePath.ToString();
        //}

    }
}
