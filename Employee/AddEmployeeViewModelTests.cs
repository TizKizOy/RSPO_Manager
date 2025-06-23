using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using MyKpiyapProject.ViewModels.UserControls.Employee;
using Moq;
using System;
using System.Threading.Tasks;

namespace ProjectTest.Employee
{
    [TestClass]
    public class AddEmployeeViewModelTests
    {
        private AddEmployeeViewModel _viewModel;
        private Mock<EmployeeService> _employeeServiceMock;
        private Mock<AdminLogService> _adminLogServiceMock;
        private Mock<LoggingService> _loggingServiceMock;
        private tbEmployee _currentUser;
        private tbEmployee _testEmployee;

        [TestInitialize]
        public void TestInitialize()
        {
            _employeeServiceMock = new Mock<EmployeeService>();
            _adminLogServiceMock = new Mock<AdminLogService>();
            _loggingServiceMock = new Mock<LoggingService>();

            _currentUser = new tbEmployee { EmployeeID = 1 };

            _testEmployee = new tbEmployee
            {
                FullName = "John Doe",
                Email = "john.doe@example.com",
                Phone = "+123456789",
                Password = "password",
                GenderID = 1,
                PositionAndRoleID = 4,
                ExperienceID = 1
            };

            _viewModel = new AddEmployeeViewModel(() => { }, _currentUser)
            {
                FullName = _testEmployee.FullName,
                Email = _testEmployee.Email,
                Phone = _testEmployee.Phone,
                Password = _testEmployee.Password,
               
            };
        }

        [TestMethod]
        public void CanSaveEmployee_ValidData_ReturnsTrue()
        {
            // Arrange
            _viewModel.FullName = _testEmployee.FullName;
            _viewModel.Email = _testEmployee.Email;
            _viewModel.Phone = _testEmployee.Phone;
            _viewModel.Password = _testEmployee.Password;

            // Act
            bool result = _viewModel.CanSaveEmployee(null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanSaveEmployee_InvalidData_ReturnsFalse()
        {
            // Arrange
            _viewModel.FullName = "";
            _viewModel.Email = "";
            _viewModel.Phone = "";
            _viewModel.Password = "";

            // Act
            bool result = _viewModel.CanSaveEmployee(null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateData_ValidData_ReturnsTrue()
        {
            // Arrange
            _viewModel.FullName = _testEmployee.FullName;
            _viewModel.Email = _testEmployee.Email;
            _viewModel.Phone = _testEmployee.Phone;
            _viewModel.Password = _testEmployee.Password;

            // Act
            bool result = _viewModel.ValidateData(out string errorMessage);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void ValidateData_InvalidData_ReturnsFalse()
        {
            // Arrange
            _viewModel.FullName = "";
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
        public async System.Threading.Tasks.Task SaveEmployee_ValidData_SavesEmployee()
        {
            // Arrange
            var mockEmployeeService = new Mock<EmployeeService>();
            var mockLoggingService = new Mock<LoggingService>();
            mockLoggingService.Setup(x => x.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(System.Threading.Tasks.Task.CompletedTask);

            var viewModel = new AddEmployeeViewModel(() => { }, new tbEmployee { EmployeeID = 1 }, mockEmployeeService.Object, mockLoggingService.Object)
            {
                FullName = _testEmployee.FullName,
                Email = _testEmployee.Email,
                Phone = _testEmployee.Phone,
                Password = _testEmployee.Password
            };

            // Act
            await System.Threading.Tasks.Task.Run(() => viewModel.SaveEmployee());

            // Assert
            mockEmployeeService.Verify(es => es.AddEmployee(It.IsAny<tbEmployee>()), Times.Once);
            mockLoggingService.Verify(ls => ls.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}