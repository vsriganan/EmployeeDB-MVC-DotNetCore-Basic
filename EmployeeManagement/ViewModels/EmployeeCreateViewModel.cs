using EmployeeManagement.Models.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class EmployeeCreateViewModel
    {
        [Required, MaxLength(50, ErrorMessage = "Name exceeded max length")]
        public string Name { get; set; }

        [Required, Display(Name = "Office Email")]
        [RegularExpression(@"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        public DepartmentEnum? Department { get; set; }

        public IFormFile Photo { get; set; }
    }
}
