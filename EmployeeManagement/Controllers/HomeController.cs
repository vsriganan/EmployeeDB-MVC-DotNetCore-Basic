using EmployeeManagement.Models;
using EmployeeManagement.Repo_Services.Interfaces;
using EmployeeManagement.Security;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        public ILogger<HomeController> Logger { get; }
        private readonly IDataProtector protector;

        public HomeController(
            IEmployeeRepository employeeRepository,
            IWebHostEnvironment hostingEnvironment,
            ILogger<HomeController> logger,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            Logger = logger;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            var employees = _employeeRepository.GetAllEmployees().Select(e =>
                                                                            {
                                                                                e.EncryptedId = protector.Protect(e.Id.ToString());
                                                                                return e;
                                                                            });

            return View(employees);
        }

        [AllowAnonymous]
        public ViewResult Details(string id)
        {
            Logger.LogTrace("Trace Log");
            Logger.LogDebug("Debug Log");
            Logger.LogInformation("Information Log");
            Logger.LogWarning("Warning Log");
            Logger.LogError("Error Log");
            Logger.LogCritical("Critical Log");

            int employeeId = Convert.ToInt32(protector.Unprotect(id));

            Employee employee = _employeeRepository.GetEmployee(employeeId);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }

            HomeDetailsViewModel viewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };
            return View(viewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int Id)
        {
            Employee employee = _employeeRepository.GetEmployee(Id);

            EmployeeEditViewModel editEmployee = new EmployeeEditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(editEmployee);
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel employee)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(employee);

                Employee newEmployee = new Employee()
                {
                    Name = employee.Name,
                    Department = employee.Department,
                    Email = employee.Email,
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("Details", new { Id = newEmployee.Id });
            }

            return View();
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel employee)
        {
            if (ModelState.IsValid)
            {
                Employee editedEmployee = _employeeRepository.GetEmployee(employee.Id);
                editedEmployee.Name = employee.Name;
                editedEmployee.Email = employee.Email;
                editedEmployee.Department = employee.Department;

                if (employee.Photo != null)
                {
                    if (employee.ExistingPhotoPath != null)
                    {
                        var filePath = Path.Combine(hostingEnvironment.WebRootPath, "img", employee.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }

                    editedEmployee.PhotoPath = ProcessUploadedFile(employee);
                }

                Employee updatedEmployee = _employeeRepository.Update(editedEmployee);
                return RedirectToAction("index");
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel employee)
        {
            string uniqueFileName = null;
            if (employee.Photo != null)
            {
                var fileUploadPath = Path.Combine(hostingEnvironment.WebRootPath, "img");
                uniqueFileName = Guid.NewGuid() + employee.Photo.FileName;

                string filepath = Path.Combine(fileUploadPath, uniqueFileName);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    employee.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
