using EmployeeManagement.Model;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace EmployeeManagement.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        public EmployeeRepository(AppDbContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }
        public async Task AddEmployeeAsync(Employee lhs)
        {
            await _ctx.Employees.AddAsync(lhs);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _ctx.Employees.FindAsync(id);

            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with Id {id} wasn't found !");
            }

            _ctx.Employees.Remove(employee);
            await _ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _ctx.Employees.ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _ctx.Employees.FindAsync(id);
        }

        public async Task UpdateEmployeeAsync(int id, Employee lhs)
        {
            var existingEmployee = await _ctx.Employees.FindAsync(id);
            
            if (existingEmployee == null)
            {
                throw new KeyNotFoundException($"Employee with Id {id} wasn't found !");
            }

            _mapper.Map(lhs, existingEmployee);

            await _ctx.SaveChangesAsync();
        }
    }
}
