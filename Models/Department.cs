using System.ComponentModel.DataAnnotations;

namespace DepartmentManagementSystem.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
