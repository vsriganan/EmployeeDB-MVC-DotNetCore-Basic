using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee()
                {
                    Id = 1,
                    Name = "Mary",
                    Department = Models.Enums.DepartmentEnum.IT,
                    Email = "mary@email.com"
                },
                new Employee()
                {
                    Id = 2,
                    Name = "John",
                    Department = Models.Enums.DepartmentEnum.HR,
                    Email = "john@email.com"
                }
            );
        }
    }
}
