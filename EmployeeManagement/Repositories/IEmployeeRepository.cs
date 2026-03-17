using EmployeeManagement.Model;

namespace EmployeeManagement.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task AddEmployeeAsync(Employee lhs);
        Task UpdateEmployeeAsync(int id, Employee lhs);
        Task DeleteEmployeeAsync(int id);
    }
}
