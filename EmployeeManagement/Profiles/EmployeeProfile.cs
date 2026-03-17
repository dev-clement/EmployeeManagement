using AutoMapper;
using EmployeeManagement.Model;

namespace EmployeeManagement.Profiles
{
    public class EmployeeProfile : Profile
    {
        /**
         * @brief Map Employee to Employee, ignoring the Id property to prevent overwriting it during updates.
         * This allows us to update an employee's details without changing their primary key
         */
        public EmployeeProfile() 
        {
            // 1. When doing Model to Model update
            CreateMap<Employee, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // 2. Used for the HATEOAS response (model to DTO)
            CreateMap<Employee, EmployeeDTO>();
        }
    }
}
