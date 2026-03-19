using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Model
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "First name is required !")]
        public string FirstName { get; set; } = String.Empty;

        [Required(ErrorMessage = "Last Name is required !")]
        public string LastName { get; set; } = String.Empty;

        [Required(ErrorMessage = "Phone number is required !")]
        [Length(5, 30)]
        public string Phone { get; set; } = String.Empty;

        [Required(ErrorMessage = "Member's position is required !")]
        public string Position { get; set; } = String.Empty;
    }
}
