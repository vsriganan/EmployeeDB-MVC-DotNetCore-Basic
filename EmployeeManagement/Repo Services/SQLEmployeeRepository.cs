using EmployeeManagement.Models;
using EmployeeManagement.Models.DBContext;
using EmployeeManagement.Repo_Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Repo_Services
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDBContext _context;
        public ILogger<SQLEmployeeRepository> Logger { get; }

        public SQLEmployeeRepository(AppDBContext context, ILogger<SQLEmployeeRepository> logger)
        {
            this._context = context;
            Logger = logger;
        }

        public Employee Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();

            return employee;
        }

        public Employee Delete(int Id)
        {
            var employee = _context.Employees.Find(Id);

            if(employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }

            return employee;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _context.Employees;
        }

        public Employee GetEmployee(int id)
        {
            Logger.LogTrace("Trace Log");
            Logger.LogDebug("Debug Log");
            Logger.LogInformation("Information Log");
            Logger.LogWarning("Warning Log");
            Logger.LogError("Error Log");
            Logger.LogCritical("Critical Log");

            return _context.Employees.Find(id);
        }

        public Employee Update(Employee employeeChanges)
        {
            var employee = _context.Employees.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return employeeChanges;
        }
    }
}
