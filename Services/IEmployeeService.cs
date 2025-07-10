using EmployeeApi.Models;

namespace EmployeeApi.Services;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee> CreateAsync(Employee employee);
    Task<bool> UpdateAsync(Guid id, Employee employee);
    Task<bool> DeleteAsync(Guid id);
}
