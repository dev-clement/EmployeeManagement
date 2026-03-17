using AutoMapper;
using EmployeeManagement.Model;
using EmployeeManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("/api/[Controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetAllEmployees()
        {
            var employees = await _employeeRepository.GetAllAsync();
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDTO>>(employees);

            foreach (var dto in employeeDtos)
            {
                CreateLinksForEmployee(dto);
            }

            return Ok(employeeDtos);
        }

        [HttpGet("{id}", Name = "GetEmployeeById")]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeById(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<EmployeeDTO>(employee);
            CreateLinksForEmployee(dto);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> CreateEmployee(Employee employee)
        {
            await _employeeRepository.AddEmployeeAsync(employee);

            var dto = _mapper.Map<EmployeeDTO>(employee);
            CreateLinksForEmployee(dto);

            return CreatedAtAction("GetEmployeeById", new { id = dto.Id }, dto);
        }

        [HttpPut("{id}", Name = "UpdateEmployee")]
        public async Task<ActionResult<EmployeeDTO>> UpdateEmployeeAsync(int id, Employee lhs) 
        {
            if (id != lhs.Id)
            {
                return BadRequest($"The id {id} is not present in the database !");
            }
            try
            {
                await _employeeRepository.UpdateEmployeeAsync(id, lhs);

                var dto = _mapper.Map<EmployeeDTO>(lhs);
                CreateLinksForEmployee(dto);

                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}", Name = "DeleteEmployee")]
        public async Task<ActionResult> DeleteEmployeeById(int id)
        {
            try
            {
                await _employeeRepository.DeleteEmployeeAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        private void CreateLinksForEmployee(EmployeeDTO dto)
        {
            // Add self link
            dto.Links.Add(new Link(
                Url.Link("GetEmployeeById", new { id = dto.Id })
                , "self"
                , "GET"));

            // Add update link
            dto.Links.Add(new Link(
                Url.Link("UpdateEmployee", new { id = dto.Id })
                , "update_employee"
                , "PUT"));

            // Add delete link
            dto.Links.Add(new Link(
                Url.Link("DeleteEmployee", new { id = dto.Id })
                , "delete_employee"
                , "DELETE"
                ));
        }
    }
}
