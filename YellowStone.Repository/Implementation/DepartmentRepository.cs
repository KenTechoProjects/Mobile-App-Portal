using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Repository.Implementation
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        private readonly FBNMDashboardContext context;

        public DepartmentRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<Department> Departments => context.Departments.OrderByDescending(x => x.Id);

        public async Task<bool> SaveDepartment(Department department)
        {
            bool success = false;
            if (department != null)
            {
                if (department.Id == 0)
                {
                  await Create(department);
                }
                else
                {
                  await Update(department);
                }
                await context.SaveChangesAsync();
                success = true;
            }
            return success;
        }
    }
}
