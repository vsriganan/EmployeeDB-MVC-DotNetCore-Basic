using EmployeeManagement.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Name exceeded max length")]
        public string Name { get; set; }

        [Required, Display(Name = "Office Email")]
        [RegularExpression(@"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        public DepartmentEnum? Department { get; set; }

        public string PhotoPath { get; set; }
    }
}
