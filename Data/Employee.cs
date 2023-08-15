using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TheTask.Data
{
    public class Employee
    {
        [Key] // Primary key annotation
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public User User { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public int? SupervisorId { get; set; }
        
        [ForeignKey("SupervisorId")]
        public Employee Supervisor { get; set; }
    }
}