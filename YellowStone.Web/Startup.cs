using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YellowStone.Authorization;
using YellowStone.Models;
using YellowStone.Models.Enums;
using YellowStone.Repository;
using YellowStone.Repository.Implementation;
using YellowStone.Repository.Interfaces;
using YellowStone.Filters;
using YellowStone.Services;
using YellowStone.Services.FBNService;
using System.Net.Http;
using YellowStone.Services.Entrust;
using YellowStone.Services.Mock;
using YellowStone.Services.Implementations;
using YellowStone.Services.Processors;
using YellowStone.Services.Onboarding;
using YellowStone.Services.FBNMobile;
using YellowStone.Services.WalletService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using YellowStone.Services.DocumentReviewService;


namespace YellowStone.Web
{
    public class Startup
    {
        private const string TotalAccessCORSPolicy = "TotalAccess";

        private readonly ILogger _logger;
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
            .SetBasePath(hostingEnvironment.ContentRootPath)
           //.AddJsonFile("appsettings.development.json", optional: false, reloadOnChange: true);
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddDbContext<FBNMDashboardContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
            });
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<FBNMDashboardContext>()
                .AddDefaultTokenProviders();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // This is the key to control how often validation takes place
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.AddAuthentication()

           .Services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });


            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRolePermissionRepository, RolePermissionRepository>();
            services.AddTransient<IOnboardingRequestRepository, OnboardingRequestRepository>();
            services.AddTransient<IAttachmentRepository, AttachmentRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<ICustomerRequestRepository, CustomerRequestRepository>();
            services.AddTransient<ILimitRepository, LimitsRepository>();
            services.AddTransient<IDocumentReviewRequestRepository, DocumentReviewRequestRepository>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAuditTrailLog, AuditTrailLogRepository>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAppConfig, AppConfig>();

            //Mock Implementation
            services.AddTransient<IFbnService, MockFbnService>();
            services.AddTransient<ITokenService, MockTokenService>();
            // services.AddTransient<IFIService, MockFIService>();
            //services.AddTransient<IFileHandler, MockFileHandler>();
            // services.AddTransient<IOnboardingProcessor, MockOnboardingService>();
            //services.AddTransient<IFbnMobileService, MockFbnMobileService>();
            //services.AddTransient<IWalletService, MockWalletService>();
            //services.AddTransient<IDocumentReview, MockDocumentReviewService>();

            //Concrete Implementation
            //services.AddTransient<IFbnService, FbnService>();
            //services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDocumentReview, DocumentReviewService>();
            services.AddTransient<IFIService, FIService>();
           services.AddTransient<IFileHandler, FileHandler>();
           services.AddTransient<IOnboardingProcessor, OnboardingService>();
           services.AddTransient<IFbnMobileService, FbnMobileService>();
           services.AddTransient<IWalletService, WalletService>();


            services.Configure<FISettings>(opt => Configuration.GetSection("FISettings").Bind(opt));
            services.Configure<EntrustSettings>(opt => Configuration.GetSection("EntrustSettings").Bind(opt));
            services.Configure<FbnServiceSettings>(opt => Configuration.GetSection("FbnServiceSettings").Bind(opt));
            services.Configure<SystemSettings>(opt => Configuration.GetSection("SystemSettings").Bind(opt));
            services.Configure<AzureStorageSettings>(opt => Configuration.GetSection("AzureStorageSettings").Bind(opt));
            services.Configure<OnboardingServiceSettings>(opt => Configuration.GetSection("OnboardingServiceSettings").Bind(opt));
            services.Configure<FbnMobileSettings>(opt => Configuration.GetSection("FbnMobileSettings").Bind(opt));
            services.Configure<WalletServiceSettings>(opt => Configuration.GetSection("WalletServiceSettings").Bind(opt));
            services.Configure<FbnMobileMiddlewareSettings>(opt => Configuration.GetSection("FbnMobileMiddlewareSettings").Bind(opt));


            services.AddHttpContextAccessor();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddHttpClient("HttpMessageHandler").ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                    UseDefaultCredentials = true,
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (message, cert, chain, policy) =>
                    {
                        return true;
                    }

                };
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();
            services.AddFlashes();
            services.AddMvc(options =>
            {
                options.Filters.Add(new PermissionFilterAttribute());
                options.Filters.Add(typeof(PermissionFilterAttribute));
            });
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("ApproveUsers", policy =>
                        policy.Requirements.Add(new ApproveUsersRequirement()));
                    options.AddPolicy("InitiateUser", policy =>
                       policy.Requirements.Add(new UserInitiatorRequirement()));
                    options.AddPolicy("InitiateRoles", policy =>
                        policy.Requirements.Add(new RoleInitiatorRequirement()));
                    options.AddPolicy("ApproveRoles", policy =>
                      policy.Requirements.Add(new RoleApprovalRequirement()));
                    options.AddPolicy("InitiateDepartment", policy =>
                        policy.Requirements.Add(new DepartmentInitiatorRequirement()));
                    options.AddPolicy("ApproveDepartments", policy =>
                      policy.Requirements.Add(new DepartmentApprovalRequirement()));

                    options.AddPolicy("SystemControl", policy =>
                     policy.Requirements.Add(new SystemControlRequirement()));

                    options.AddPolicy("ApproveProfileManagement", policy =>
                    policy.Requirements.Add(new ApproveProfileManagementRequirement()));

                    options.AddPolicy("InitiateProfileManagement", policy =>
                    policy.Requirements.Add(new InitiateProfileManagementRequirement()));

                    options.AddPolicy("InitiateAccountLinking", policy =>
                    policy.Requirements.Add(new AccountLinkingInitiatorRequirement()));

                    options.AddPolicy("ApproveAccountLinking", policy =>
                    policy.Requirements.Add(new AccountLinkingApproverRequirement()));

                }
            );

            services.AddScoped<IAuthorizationHandler,
                        UserApproveAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler,
                         UserInitiatorAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler,
                       RoleInitiatorAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler,
                       RoleApprovalAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler,
                       DepartmentInitiatorAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler,
                       DepartmentApprovalAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler,
                    SystemControlAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler,
                  ProfileManagementInitiatorAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler,
                  ProfileManagmentApprovalAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler,
                 AccountLinkingInitiatorAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler,
                  AccountLinkingApprovalAuthorizationHandler>();


            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(5); });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                await next();
            });

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddLog4Net(Configuration.GetValue<string>("Log4NetConfigFile:Name"));
            loggerFactory.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseHsts();
            //app.UseHttpsRedirection();


            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                Secure = CookieSecurePolicy.SameAsRequest,
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always
            };

            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                
                //  template: "{controller=DocumentReview}/{action=DocumentReviewList}/{id?}");
            });

           //app.UseForwardedHeaders(new ForwardedHeadersOptions
           //{
           // ForwardedHeaders = ForwardedHeaders.XForwardedFor |
           // ForwardedHeaders.XForwardedProto
           //});

            //Initializes the default resources
            InitializeResources(serviceProvider, loggerFactory).Wait();
        }

        private async Task InitializeResources(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {

            ILogger logger = loggerFactory.CreateLogger<Startup>();
            try
            {

                Permission resource = new Permission();
                User user = new User();

                Department department = new Department();

                var permissionRepository = serviceProvider.GetRequiredService<IPermissionRepository>();
                var roleRepository = serviceProvider.GetRequiredService<IRoleRepository>();
                var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
                var departmentRepository = serviceProvider.GetRequiredService<IDepartmentRepository>();
                var rolePermissionRepository = serviceProvider.GetRequiredService<IRolePermissionRepository>();

                await CreateDefaultDepartment(departmentRepository);

                await CreatePermissions(permissionRepository);

                var role = await CreateSuperAdminRole(roleRepository);

                role = await CreateRolePermisions(permissionRepository, roleRepository, rolePermissionRepository, role);

                await CreateSuperAdminUser(user, userRepository, role.Id);
            }
            catch (Exception er)
            {
                logger.LogError($"StartUp Error: InitializeResources : {er}");
            }
        }

        private async Task CreateSuperAdminUser(User user, IUserRepository userRepository, long roleId)
        {
            string sn = Configuration.GetValue<string>("SystemSettings:SuperAdminSN");
            string name = Configuration.GetValue<string>("SystemSettings:SuperAdminName");
            string departmentName = Configuration.GetValue<string>("SystemSettings:SuperAdminDepartmentName");
            string emailAddress = Configuration.GetValue<string>("SystemSettings:SuperAdminEmailAddress");

            if (!string.IsNullOrEmpty(sn) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(departmentName))
            {
                var superAdminAlreadyExists = userRepository.FindWhere(x => x.IsAdmin == true).Any();
                if (!superAdminAlreadyExists)
                {
                    user.CreatedBy = "System";
                    user.CreatedDate = DateTime.Now;
                    user.ApprovedBy = "System";
                    user.ApprovedDate = DateTime.Now;
                    user.Name = name;
                    user.StaffId = sn;
                    user.IsAdmin = true;
                    user.RoleId = roleId;
                    user.Status = UserStatus.Active;
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    user.Email = emailAddress;
                    user.UserName = sn;
                    user.StaffAdDepartment = departmentName;
                    await userRepository.Create(user);
                }
            }
        }


        private async Task<Role> CreateRolePermisions(IPermissionRepository permissionRepository, IRoleRepository roleRepository, IRolePermissionRepository rolePermissionRepository, Role role)
        {
            List<Permission> resourceList = new List<Permission>();
            resourceList = permissionRepository.Permissions.ToList();
            if (role == null || role.Id == 0)
            {
                role = await roleRepository.Single(x => x.Name.ToLower().Contains("superadmin"));
            }

            if (role != null && role.Id > 0 && resourceList != null && resourceList.Count > 0)
            {
                foreach (Permission res in resourceList)
                {
                    RolePermission rolePermission = await rolePermissionRepository.Single(x => x.RoleId == role.Id && x.PermissionId == res.Id);
                    if (rolePermission == null || rolePermission.Id == 0)
                    {
                        rolePermission = new RolePermission
                        {
                            CreatedBy = "System",
                            CreatedDate = DateTime.Now,
                            PermissionId = res.Id,
                            RoleId = role.Id
                        };
                        await rolePermissionRepository.Create(rolePermission);
                    }
                }
            }

            return role;
        }
        private async Task CreateDefaultDepartment(IDepartmentRepository repository)
        {

            var checkDept = await repository.Any(x => x.Name.ToUpper() == "ISOD");
            if (!checkDept)
            {
                var values = Configuration.GetSection("SystemSettings:DefaultDepartments");
                var defaultDepts = values.AsEnumerable();
                foreach (var defaultDept in defaultDepts.ToList())
                {
                    var dept = new Department();
                    if (defaultDept.Value != null)
                    {
                        dept.CreatedBy = "System";
                        dept.CreatedDate = DateTime.Now;
                        dept.ApprovedDate = DateTime.Now;
                        dept.Name = defaultDept.Value;
                        dept.Status = RequestStatus.Active;
                        await repository.Create(dept);
                    }
                }
            }

        }

        private async Task<Role> CreateSuperAdminRole(IRoleRepository roleRepository)
        {
            var role = new Role();
            var checkRole = roleRepository.FindWhere(x => x.Name.ToLower().Contains("superadmin"));
            if (checkRole == null || checkRole.Count() == 0)
            {
                role.CreatedDate = DateTime.Now;
                role.ApprovedDate = DateTime.Now;
                role.Name = "SuperAdmin";
                role.Status = RequestStatus.Active;
                role.CreatedBy = "System";
                await roleRepository.Create(role);
            }
            else
                role = checkRole.FirstOrDefault();
            return role;
        }

        private async Task CreatePermissions(IPermissionRepository permissionRepository)
        {
            Dictionary<string, string> perms = new Dictionary<string, string>()
                {
                    {"InitiateRoles" ,"AdminInitiator"},
                    {"ApproveRoles" ,"AdminApprover"},

                    {"InitiateDepartment" ,"AdminInitiator"},
                    {"ApproveDepartments","AdminApprover"},

                    {"InitiateUser" ,"AdminInitiator"},
                    {"ApproveUsers" ,"AdminApprover"},

                    {"InitiateProfileManagement" ,"NonAdminInitiator"},
                    {"ApproveProfileManagement" ,"NonAdminApprover"},

                    {"InitiateAccountLinking" ,"NonAdminInitiator"},
                    {"ApproveAccountLinking" ,"NonAdminApprover"},

                    {"SystemControl" ," Viewer"},

                };

            foreach (var item in perms)
            {
                var checkResource = permissionRepository.FindWhere(x => x.Name.ToLower().Contains(item.Key.ToLower()));
                if (checkResource == null || checkResource.Count() == 0)
                {
                    Permission newpermission = new Permission
                    {
                        CreatedBy = "System",
                        CreatedDate = DateTime.Now,
                        Name = item.Key,
                        ResourceType = "Action Link",
                        PermissionCategory = item.Value
                    };
                    await permissionRepository.Create(newpermission);
                }
            }
        }
    }
}
