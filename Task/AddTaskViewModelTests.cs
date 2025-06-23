using Moq;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.UserControls.Task;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTest.Task
{
    [TestClass]
    public class AddTaskControlViewModelTests
    {
        private AddTaskControlViewModel _viewModel;
        private Mock<TaskService> _taskServiceMock;
        private Mock<EmployeeService> _employeeServiceMock;
        private Mock<ProjectService> _projectServiceMock;
        private Mock<LoggingService> _loggingServiceMock;
        private Mock<AdminLogService> _adminLogServiceMock;
        private tbEmployee _currentUser;
        private tbTask _testTask;
        private tbEmployee _testExecutor;
        private tbProject _testProject;

        [TestInitialize]
        public void TestInitialize()
        {
            // Инициализация моков
            _taskServiceMock = new Mock<TaskService>();
            _employeeServiceMock = new Mock<EmployeeService>();
            _projectServiceMock = new Mock<ProjectService>();
            _loggingServiceMock = new Mock<LoggingService>();
            _adminLogServiceMock = new Mock<AdminLogService>();

            // Создание тестовых данных
            _currentUser = new tbEmployee { EmployeeID = 3, FullName = "Test User" };
            _testExecutor = new tbEmployee { EmployeeID = 3, FullName = "Executor User" };
            _testProject = new tbProject { ProjectID = 1, Title = "Test Project" };

            //// Добавление тестовых данных в базу данных
            //using (var context = new AppDbContext())
            //{
            //    context.tbEmployees.Add(_currentUser);
            //    context.tbEmployees.Add(_testExecutor);
            //    context.tbProjects.Add(_testProject);
            //    context.SaveChanges();
            //}

            _testTask = new tbTask
            {
                Title = "Test Task",
                Description = "Test Description",
                PriorityID = 2,
                StatusID = 1,
                DeadLineDate = DateTime.Today.AddDays(1)
            };

            _viewModel = new AddTaskControlViewModel(() => { }, _currentUser)
            {
                NameTasks = _testTask.Title,
                DescriptionTasks = _testTask.Description,
                DeadLineDate = _testTask.DeadLineDate,
                SelectedExecutor = _testExecutor,
                SelectedProject = _testProject
            };
        }

        [TestMethod]
        public void CanSaveTask_ValidData_ReturnsTrue()
        {
            // Arrange
            _viewModel.SelectedExecutor = new tbEmployee { EmployeeID = 2, FullName = "Executor User" };
            _viewModel.SelectedProject = new tbProject { ProjectID = 1, Title = "Test Project" };

            // Act
            bool result = _viewModel.CanSaveTask(null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanSaveTask_InvalidData_ReturnsFalse()
        {
            _viewModel.NameTasks = "";
            _viewModel.SelectedExecutor = null;
            _viewModel.SelectedProject = new tbProject { ProjectID = 1, Title = "Test Project" };

            // Act
            bool result = _viewModel.CanSaveTask(null);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
