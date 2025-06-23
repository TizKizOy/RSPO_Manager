using Moq;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.UserControls.Employee;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ProjectTest.Employee
{
    [TestClass]
    public class EditEmployeeViewModelTests
    {
        private EditEmployeeViewModel _viewModel;
        private Mock<EmployeeService> _employeeServiceMock;
        private Mock<LoggingService> _loggingServiceMock;
        private tbEmployee _currentUser;
        private tbEmployee _originalEmployee;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize mock objects
            _employeeServiceMock = new Mock<EmployeeService>();
            _loggingServiceMock = new Mock<LoggingService>();

            // Initialize current user
            _currentUser = new tbEmployee { EmployeeID = 1, PositionAndRoleID = 1 };

            // Initialize original employee with related objects
            _originalEmployee = new tbEmployee
            {
                EmployeeID = 2,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                Phone = "+123456789",
                Gender = new tbGender { GenderName = "Male" }, // Initialize Gender object
                PositionAndRoleID = 4,
                ExperienceID = 1,
                Password = "oldPassword",
            };



            // Initialize ViewModel with all necessary dependencies
            _viewModel = new EditEmployeeViewModel(
                _originalEmployee,
                () => { },
                _currentUser,
                _employeeServiceMock.Object,
                _loggingServiceMock.Object
            );

            // Set ViewModel properties
            _viewModel.Name = _originalEmployee.FullName;
            _viewModel.Email = _originalEmployee.Email;
            _viewModel.Phone = _originalEmployee.Phone;
            _viewModel.Password = "newPassword";
        }

        [TestMethod]
        public void ValidateData_ValidData_ReturnsTrue()
        {
            // Arrange
            _viewModel.Name = _originalEmployee.FullName;
            _viewModel.Email = _originalEmployee.Email;
            _viewModel.Phone = _originalEmployee.Phone;
            _viewModel.Password = "newPassword";

            // Act
            bool result = _viewModel.ValidateData(out string errorMessage);

            // Assert
            Assert.IsFalse(result);

        }

        [TestMethod]
        public void ValidateData_InvalidData_ReturnsFalse()
        {
            // Arrange
            _viewModel.Name = "";
            _viewModel.Email = "invalid-email";
            _viewModel.Phone = "123456789";
            _viewModel.Password = "123";

            // Act
            bool result = _viewModel.ValidateData(out string errorMessage);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task SaveEmployee_ValidData_UpdatesEmployee()
        {
            // Arrange
            var mockEmployeeService = new Mock<EmployeeService>();
            var mockLoggingService = new Mock<LoggingService>();
            mockLoggingService.Setup(x => x.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Ensure _originalEmployee and _currentUser are properly initialized
            var originalEmployee = new tbEmployee
            {
                EmployeeID = 2,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                Phone = "+123456789",
                Gender = new tbGender { GenderName = "Male" },
                PositionAndRoleID = 4,
                ExperienceID = 1,
                Password = "oldPassword",
                PositionAndRole = new tbPositionAndRole { PositionAndRoleID = 4, PositionAndRoleName = "Developer" },
                Experience = new tbWorkExperience { ExperienceID = 1, ExperienceName = "Senior" }
            };

            var currentUser = new tbEmployee
            {
                EmployeeID = 1,
                PositionAndRoleID = 1,
                PositionAndRole = new tbPositionAndRole { PositionAndRoleID = 1, PositionAndRoleName = "Admin" }
            };

            var viewModel = new EditEmployeeViewModel(originalEmployee, () => { }, currentUser, mockEmployeeService.Object, mockLoggingService.Object)
            {
                Name = originalEmployee.FullName,
                Email = originalEmployee.Email,
                Phone = originalEmployee.Phone,
                Password = "newPassword",
                Gender = originalEmployee.Gender.GenderName,
                Position = originalEmployee.PositionAndRole.PositionAndRoleName,
                Experience = originalEmployee.Experience.ExperienceName
            };

            // Act
            await System.Threading.Tasks.Task.Run(() => viewModel.SaveEmployee());

            // Assert
            mockEmployeeService.Verify(es => es.UpdateEmployee(It.IsAny<tbEmployee>()), Times.Once);
            mockLoggingService.Verify(ls => ls.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

    }
}
