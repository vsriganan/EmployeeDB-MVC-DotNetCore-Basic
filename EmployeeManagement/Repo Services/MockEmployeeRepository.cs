using EmployeeManagement.Models;
using EmployeeManagement.Models.Enums;
using EmployeeManagement.Repo_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Repo_Services
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee()
                {
                    Id = 1,
                    Name = "One",
                    Email = "first@gmail.com",
                    Department = DepartmentEnum.HR
                },
                new Employee()
                {
                    Id = 2,
                    Name = "Two",
                    Email = "second@gmail.com",
                    Department = DepartmentEnum.IT
                },
                new Employee()
                {
                    Id = 3,
                    Name = "Three",
                    Email = "third@gmail.com",
                    Department = DepartmentEnum.None
                },
                new Employee()
                {
                    Id = 4,
                    Name = "Four",
                    Email = "fourth@gmail.com",
                    Department = DepartmentEnum.Support
                }
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id = _employeeList.Max(employee => employee.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public Employee Delete(int Id)
        {
            var emp = _employeeList.FirstOrDefault(e => e.Id == Id);

            if(emp != null)
            {
                _employeeList.Remove(emp);
            }

            return emp;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == id);
        }

        public Employee Update(Employee employeeChanges)
        {
            var emp = _employeeList.FirstOrDefault(e => e.Id == employeeChanges.Id);

            if (emp != null)
            {
                emp.Name = employeeChanges.Name;
                emp.Department = employeeChanges.Department;
                emp.Email = employeeChanges.Email;
            }

            return emp;
        }
    }
}
