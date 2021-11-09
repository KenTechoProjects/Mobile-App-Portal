using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Filters
{
    public class PermissionFilterAttribute : Attribute, IAsyncActionFilter
    {
        public string ResourceName { get; set; }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate action)
        {
            var value = new byte[10000];
            List<Permission> Permissions = new List<Permission>();

            bool ok = context.HttpContext.Session.TryGetValue("Permissions", out value);

            if (!string.IsNullOrEmpty(ResourceName) && ok == true && value != null && value.Count() > 0)
            {
                string sessionValue = System.Text.Encoding.UTF8.GetString(value);
                Permissions = JsonConvert.DeserializeObject<List<Permission>>(sessionValue);

                Permissions = Permissions.Where(x => x.Name.ToLower().Contains(ResourceName.ToLower())).ToList();
                if (Permissions != null && Permissions.Count > 0)
                {
                    context.HttpContext.Items["Allow"] = true;
                }
                else
                {
                    context.HttpContext.Items["Allow"] = false;
                }
            }

            await action();
        }

        public async Task OnActionExecutedAsync(ActionExecutedContext context, ActionExecutionDelegate action)
        {

        }
    }
}
