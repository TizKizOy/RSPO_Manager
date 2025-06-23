using Moq;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.UserControls.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTest.Employee
{
    [TestClass]
    public class EmployeeViewModelTests
    {
        private EmployeeViewModel _viewModel;
        private Mock<EmployeeService> _employeeServiceMock;
        private Mock<LoggingService> _loggingServiceMock;
        private tbEmployee _currentUser;
        private tbEmployee _selectedEmployee;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize mock objects
            _employeeServiceMock = new Mock<EmployeeService>();
            _loggingServiceMock = new Mock<LoggingService>();

            // Create a PositionAndRole object for the current user
            var positionAndRole = new tbPositionAndRole
            {
                PositionAndRoleID = 1,
                PositionAndRoleName = "Админ" // Set the appropriate role name
            };

            // Initialize current user with PositionAndRole
            _currentUser = new tbEmployee
            {
                EmployeeID = 1,
                PositionAndRoleID = 1,
                PositionAndRole = positionAndRole // Assign the PositionAndRole object
            };

            // Initialize selected employee
            _selectedEmployee = new tbEmployee
            {
                EmployeeID = 2,
                FullName = "John Doe"
            };

            // Initialize ViewModel with the current user
            _viewModel = new EmployeeViewModel(_currentUser)
            {
                SelectedEmployee = _selectedEmployee
            };
        }

        [TestMethod]
        public void CanRemoveEmployee_ValidData_ReturnsTrue()
        {
            // Arrange
            _viewModel.SelectedEmployee = _selectedEmployee;

            // Act
            bool result = _viewModel.RemoveEmployeeCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanRemoveEmployee_InvalidData_ReturnsFalse()
        {
            // Arrange
            _viewModel.SelectedEmployee = null;

            // Act
            bool result = _viewModel.RemoveEmployeeCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task RemoveEmployee_ValidData_DeletesEmployee()
        {
            // Arrange
            var mockEmployeeService = new Mock<EmployeeService>();
            var mockLoggingService = new Mock<LoggingService>();
            mockLoggingService.Setup(x => x.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Создаем тестового пользователя для удаления с фиктивным EmployeeID
            var employeeToDelete = new tbEmployee { EmployeeID = 2, FullName = "John Doe" };

            // Настраиваем мок EmployeeService для удаления сотрудника
            mockEmployeeService.Setup(es => es.DeleteEmployee(employeeToDelete.EmployeeID));

            // Создаем экземпляр EmployeeViewModel и устанавливаем SelectedEmployee
            var viewModel = new EmployeeViewModel(_currentUser, mockEmployeeService.Object, mockLoggingService.Object)
            {
                SelectedEmployee = employeeToDelete
            };

            // Act
            await System.Threading.Tasks.Task.Run(() => viewModel.RemoveEmployee());

            // Assert
            mockEmployeeService.Verify(es => es.DeleteEmployee(employeeToDelete.EmployeeID), Times.Once);
            mockLoggingService.Verify(ls => ls.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

    }
}
