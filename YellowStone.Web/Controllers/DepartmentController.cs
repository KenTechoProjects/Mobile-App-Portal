using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Flash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YellowStone.Models;
using YellowStone.Models.Enums;
using YellowStone.Repository;
using YellowStone.Repository.Interfaces;
using YellowStone.Filters;
using YellowStone.Services;
using YellowStone.Web.ViewModels;
using System.Text.Encodings.Web;

namespace YellowStone.Controllers
{
    [Authorize]
    public class DepartmentController : BaseController
    {

        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly ILogger logger;
        private readonly IAuthorizationService _authSvc;
        IFlasher _flash;


        DepartmentViewModel departmentViewModel = new DepartmentViewModel();

        public DepartmentController(IAuthorizationService _authSvc, IOptions<SystemSettings> options, UserManager<User> userManager,
            ILogger<Department> logger, IRoleRepository roleRepository,
            IUserRepository userRepository, IDepartmentRepository departmentRepository, IFlasher flash, 
            IAuditTrailLog auditTrailLog, IHttpContextAccessor accessor) :
            base(userManager, options, auditTrailLog, accessor, userRepository )
        {
            this.userRepository = userRepository;
            this.departmentRepository = departmentRepository;
            this.roleRepository = roleRepository;
            this.logger = logger;
            _flash = flash;
            this._authSvc = _authSvc;

        }
        [HttpGet]
        public async Task<IActionResult> Index(long departmentId = 0)
        {
            var user = await CurrentUser();

            ViewBag.user = user;
            var action = $"Accessed department page";
            try
            {
                await InitiateDepartment(departmentId, user);
                GetList();
            }
            catch (Exception er)
            {
                logger.LogError(string.Format("Error occured. DepartmentController : Index : {0}", er));
            }

            //Audit log
            await AddAudit(action, user);
            return View(departmentViewModel);

        }

        private async Task InitiateDepartment(long departmentId, User user)
        {

            departmentViewModel.PageBaseClass = new PageBaseClass
            {
                User = user
            };
            departmentViewModel.PageBaseClass.Departments = true;

            departmentViewModel.PageBaseClass.AdminPageName = "Departments";

            departmentViewModel.Department = new Department();
            if (departmentId > 0)
            {
                departmentViewModel.Department = await departmentRepository.Single(x => x.Id == departmentId);
            }
            departmentViewModel.Departments = departmentRepository.Departments.ToList();
        }

        public async Task<IActionResult> Create(Department department)
        {
            try
            {
                
                User user = await CurrentUser();
                department.CreatedDate = DateTime.Now;
                string msg = "";

                if (ModelState.IsValid)
                {
                    //  Department departmentExitence = new Department();
                    bool result = false;

                    if (department.Id == 0)
                    {
                        var departmentExitence = await departmentRepository.Single(x => x.Name.ToLower() == department.Name.ToLower());
                        if (departmentExitence != null && !string.IsNullOrEmpty(departmentExitence.Name))
                        {
                            _flash.Flash("danger", "Duplicate Departments are not allowed. Kindly provide another department name.");
                            return RedirectToAction("Index");
                        }
                        department.CreatedBy = user.Name;
                        department.CreatedDate = DateTime.Now;
                        if (user.IsAdmin)
                        {

                            department.Status = RequestStatus.Active;
                            msg = $"Department {department.Name} was created successfully";
                        }
                        else
                        {
                            department.Status = RequestStatus.Pending;
                            msg = $"Department {department.Name} was created successfully. Awaiting Approval";

                        }

                        result = await departmentRepository.SaveDepartment(department);

                        if (result)
                        {
                            //Audit Log
                            var action = $"Created {department.Name} department";
                            var data = $"Data: {JsonConvert.SerializeObject(department)}";
                            await AddAudit(action, user, data);
                            _flash.Flash("success", msg);
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        var oldDepartment = await departmentRepository.Single(x => x.Id == department.Id);
                        var OldData = JsonConvert.SerializeObject(oldDepartment);

                        var departmentExitence = await departmentRepository.Single(x => x.Name.ToLower() == department.Name.ToLower());
                        if (departmentExitence != null && !string.IsNullOrEmpty(departmentExitence.Name))
                        {
                            if (oldDepartment.Name != department.Name)
                            {
                                _flash.Flash("danger", "Duplicate Departments are not allowed. Kindly provide another department name.");
                                return RedirectToAction("Index");
                            }
                        }
                        oldDepartment.Name = department.Name;
                        if (user.IsAdmin)
                        {
                            oldDepartment.Status = RequestStatus.Active;
                            msg = $"Department {department.Name} was updated successfully";
                        }
                        else
                        {
                            oldDepartment.Status = RequestStatus.Pending;
                            msg = $"Department {department.Name} was updated successfully. Awaiting Approval";

                        }
                        if (user != null)
                        {
                            result = await departmentRepository.SaveDepartment(oldDepartment);
                        }

                        if (result)
                        {
                            //Audit Log
                            var action = $"Edited {oldDepartment.Name} department";
                            var data = $"OldData: {OldData} : new Data :{JsonConvert.SerializeObject(oldDepartment)}";

                            await AddAudit(action, user, data);

                            _flash.Flash("success", msg);
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    _flash.Flash("danger", "Invalid input");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception er)
            {
                logger.LogError(string.Format("Error occurred. User is {0}. DepartmentController : Create : {1}", department.CreatedBy, er));
                _flash.Flash("danger", "Something went wrong");
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", new { Error = true });
        }

        public async Task<IActionResult> Edit(long DepartmentID)
        {
            Department department = new Department();
            var user = await CurrentUser();
            try
            {
                if (DepartmentID > 0)
                {
                    await InitiateDepartment(DepartmentID, user);
                    department = await departmentRepository.Single(x => x.Id == DepartmentID);
                    departmentViewModel.Department = department;
                }
            }
            catch (Exception er) { logger.LogError("DepartmentController : Edit : " + er.ToString()); return RedirectToAction("Login", "Account"); }

            return PartialView("_Create", departmentViewModel.Department);
        }

        public IActionResult ViewRoles(int DepartmentID)
        {
            List<Role> roleList = new List<Role>();
            try
            {
                if (DepartmentID > 0)
                {
                    string resourceString = HttpContext.Session.GetString("DepartmentRoles");
                    if (!string.IsNullOrEmpty(resourceString))
                    {
                        roleList = JsonConvert.DeserializeObject<List<Role>>(resourceString);
                        ViewBag.DepartmentRoles = roleList;
                    }
                }
            }
            catch (Exception er) { logger.LogError("DepartmentController : ViewRoles : " + er.ToString()); }

            return PartialView("_ViewRoles", roleList);
        }

        public IActionResult ClearCachedRoles()
        {

            try
            {
                HttpContext.Session.Remove("DepartmentRoles");
            }
            catch (Exception er)
            {
                logger.LogError("ClearCachedRoles", er);
            }

            return PartialView("_Roles", new List<Role>());
        }

        private async Task<List<User>> FetchDepartmentUsers(long DepartmentID)
        {
            List<User> userList = new List<User>();
            User user = new User();

            try
            {
                Department department = await departmentRepository.Single(x => x.Id == DepartmentID);
                userList = userRepository.Users.Where(x => x.Role.DepartmentId == department.Id && x.Status == UserStatus.Active).ToList();
            }
            catch (Exception er)
            {
                logger.LogError("DepartmentController : FetchDepartmentUsers : " + er.ToString());
            }

            return userList;
        }

        public async Task<IActionResult> ViewUsers(long DepartmentID)
        {
            List<User> userList = new List<User>();
            User user = new User();

            try
            {
                if (DepartmentID > 0)
                {
                    userList = await FetchDepartmentUsers(DepartmentID);
                }
            }
            catch (Exception er) { logger.LogError("DepartmentController : ViewUsers : " + er.ToString()); }

            return PartialView("_ViewUsers", userList);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateDepartmentStatus(long departmentId, string actionType)
        {
            var currentUser = await CurrentUser();
            var message = string.Empty;
            try
            {
                if (departmentId > 0)
                {
                    var department = await departmentRepository.GetByID(departmentId);
                    var oldData = JsonConvert.SerializeObject(department);

                    if (actionType == ActionTypes.Approve.ToString())
                    {
                        department.Status = RequestStatus.Active;
                        message = $"Approved {department.Name}";
                    }
                    else
                    {
                        department.Status = RequestStatus.Rejected;
                        message = $"Rejected {department.Name}";
                    }

                    await departmentRepository.Update(department);
                    var data = $"Olddata : {oldData} | Newdata: {JsonConvert.SerializeObject(department)}";
                    await AddAudit(message, currentUser, data);
                    _flash.Flash("success", $"{message}");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception er) { logger.LogError("DepartmentController : RemoveUser : " + er.ToString()); }
            _flash.Flash("error", $"Something went wrong");
            return RedirectToAction("Index");
        }

        private void GetList()
        {
            var departments = departmentRepository.Departments.Where(s => s.Id > 0).ToList();
            departmentViewModel.DepartmentList = departments;
        }
    }

}