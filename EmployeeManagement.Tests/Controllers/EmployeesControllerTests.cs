using AutoMapper;
using EmployeeManagement.Repositories;
using EmployeeManagement.Model;
using Moq;
using EmployeeManagement.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly EmployeeController _controller;

        public EmployeesControllerTests()
        {
            _mockRepo = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new EmployeeController(_mockRepo.Object, _mockMapper.Object);

            // Somulation of the IUrlHelper for avoid NullReferenceException in the CreateLinksForEmployee method
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost/test");
            _controller.Url = mockUrlHelper.Object;
        }

        [Fact]
        public async Task GetAllEmployees_ReturnOk_WithListOfEmployeesAndCorrectLinks()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "John", LastName = "Doe" },
                new Employee { Id = 2, FirstName = "Jane", LastName = "Smith"  }
            };
            var employeeDtos = new List<EmployeeDTO>
            {
                new EmployeeDTO { Id = 1, FirstName = "John", LastName = "Doe", Links = new List<Link>() },
                new EmployeeDTO { Id = 2, FirstName = "Jane", LastName = "Smith", Links = new List<Link>() }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(employees);

            // We tell the mock mapper to return our DTO list when any IEnumerable of employee is passed
            _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeDTO>>(It.IsAny<IEnumerable<Employee>>()))
                .Returns(employeeDtos);

            // Act
            var result = await _controller.GetAllEmployees();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDtos = Assert.IsAssignableFrom<IEnumerable<EmployeeDTO>>(okResult.Value);

            Assert.Equal(2, returnedDtos.Count());

            // Check for the HATEOAS links in the first employee DTO
            foreach(var dto in returnedDtos)
            {
                // We wait 3 links: self, update_employee, delete_employee
                Assert.Equal(3, dto.Links.Count);

                // Check for the self link
                var selfLink = dto.Links.FirstOrDefault(l => l.Rel == "self");
                Assert.NotNull(selfLink);
                Assert.Equal("GET", selfLink.Method);
                Assert.Equal("http://localhost/test", selfLink.Href);

                // Check for the update_employee link
                var updateLink = dto.Links.FirstOrDefault(l => l.Rel == "update_employee");
                Assert.NotNull(updateLink);
                Assert.Equal("PUT", updateLink.Method);

                // Check for the delete_employee link
                var deleteLink = dto.Links.FirstOrDefault(l => l.Rel == "delete_employee");
                Assert.NotNull(deleteLink);
                Assert.Equal("DELETE", deleteLink.Method);
            }

            _mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEmployeeById_ReturnsOk_WhenEmployeeExists()
        {
            // Arrange
            int empId = 1;
            var employee = new Employee { Id = empId, FirstName = "John" };
            var employeeDto = new EmployeeDTO { Id = empId, FirstName = "John" };

            _mockRepo.Setup(repo => repo.GetByIdAsync(empId)).ReturnsAsync(employee);
            _mockMapper.Setup(m => m.Map<EmployeeDTO>(It.IsAny<Employee>())).Returns(employeeDto);

            // Act
            var result = await _controller.GetEmployeeById(empId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<EmployeeDTO>(okResult.Value);

            // 1. Check basic data
            Assert.Equal(empId, returnedDto.Id);

            // 2. Check HATEOAS Links (Indirectly testing the private CreateLinksForEmployee method)
            Assert.Equal(3, returnedDto.Links.Count);

            // Verify self
            var selfLink = returnedDto.Links.FirstOrDefault(l => l.Rel == "self");
            Assert.NotNull(selfLink);
            Assert.Equal("GET", selfLink.Method);
            Assert.Equal("http://localhost/test", selfLink.Href);

            // Verify "update_employee" link
            var updateLink = returnedDto.Links.FirstOrDefault(l => l.Rel == "update_employer");
            Assert.NotNull(updateLink);
            Assert.Equal("PUT", updateLink.Method);

            // Verify "delete_employee" link
            var deleteLink = returnedDto.Links.FirstOrDefault(l => l.Rel == "delete_employee");
            Assert.NotNull(deleteLink);
            Assert.Equal("DELETE", deleteLink.Method);

            _mockRepo.Verify(repo => repo.GetByIdAsync(empId), Times.Once);
        }

        [Fact]
        public async Task GetEmployeeById_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            int empId = 999;
            // We setup the repository to return null when the GetByIdAsync method is called with the non-existing id
            _mockRepo.Setup(repo => repo.GetByIdAsync(empId)).ReturnsAsync((Employee?)null);

            // Act
            var result = await _controller.GetEmployeeById(empId);

            // Assert
            // When the controller returns NotFound, the Result property contains the NotFoundResult
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateEmployee_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            int empId = 1;
            var employeeToUpdate = new Employee { Id = empId, FirstName = "UpdatedName" };
            var employeeDto = new EmployeeDTO { Id = empId, FirstName = "UpdatedName", Links = new List<Link>() };

            // We mock a successful repository update
            _mockRepo.Setup(repo => repo.UpdateEmployeeAsync(empId, employeeToUpdate)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<EmployeeDTO>(It.IsAny<Employee>())).Returns(employeeDto);

            // Act
            var result = await _controller.UpdateEmployeeAsync(empId, employeeToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<EmployeeDTO>(okResult.Value);

            // Verify the HATEOAS links are present on successful update
            Assert.Equal(3, returnedDto.Links.Count);
            Assert.Contains(returnedDto.Links, l => l.Rel == "self");
            Assert.Contains(returnedDto.Links, l => l.Rel == "update_employee");
            Assert.Contains(returnedDto.Links, l => l.Rel == "delete_employee");

            _mockRepo.Verify(repo => repo.UpdateEmployeeAsync(empId, employeeToUpdate), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            int urlId = 1;
            var emp = new Employee { Id = 2 };

            // Act
            var result = await _controller.UpdateEmployeeAsync(urlId, emp);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
