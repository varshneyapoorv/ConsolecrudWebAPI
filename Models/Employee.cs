using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.Models;

public class Employee
{
    public Guid Id { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Range(18, 65)]
    public int Age { get; set; }

    public DateTime DateOfJoining { get; set; }
}
