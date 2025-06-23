using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.UserControls.Task;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ProjectTest.Task
{
    [TestClass]
    public class EditTaskControlViewModelTests
    {
        private EditTaskControlViewModel _viewModel;
        private Mock<TaskService> _taskServiceMock;
        private Mock<EmployeeService> _employeeServiceMock;
        private Mock<ProjectService> _projectServiceMock;
        private Mock<LoggingService> _loggingServiceMock;
        private tbEmployee _currentUser;
        private tbTask _testTask;


        [TestInitialize]
        public void TestInitialize()
        {
            // Инициализация моков сервисов
            _taskServiceMock = new Mock<TaskService>();
            _employeeServiceMock = new Mock<EmployeeService>();
            _projectServiceMock = new Mock<ProjectService>();
            _loggingServiceMock = new Mock<LoggingService>();

            // Инициализация текущего пользователя
            _currentUser = new tbEmployee { EmployeeID = 1, FullName = "Test User" };

            // Инициализация тестовой задачи
            _testTask = new tbTask
            {
                TaskID = 1,
                Title = "Test Task",
                Description = "Test Description",
                PriorityID = 2,
                StatusID = 1,
                DeadLineDate = DateTime.Today.AddDays(1),
                ProjectID = 1,
                ExecutorID = 2,
                Priority = new tbPriorityTask { PriorityName = "Высокий" },
                Status = new tbStatusTask { StatusName = "В процессе" }
            };

            // Инициализация коллекций в ViewModel
            var projects = new ObservableCollection<tbProject>
            {
                new tbProject { ProjectID = 1, Title = "Test Project" }
            };

                    var executors = new ObservableCollection<tbEmployee>
            {
                new tbEmployee { EmployeeID = 2, FullName = "Executor User" }
            };

            _viewModel = new EditTaskControlViewModel(_testTask, () => { }, _currentUser, _taskServiceMock.Object, _loggingServiceMock.Object)
            {
                Projects = projects,
                Executors = executors
            };

            // Убедитесь, что SelectedProject и SelectedExecutor установлены
            _viewModel.SelectedProject = projects.FirstOrDefault();
            _viewModel.SelectedExecutor = executors.FirstOrDefault();
        }





        [TestMethod]
        public void LoadCurrentSelections_ValidData_PropertiesSet()
        {
            // Act
            _viewModel.LoadCurrentSelections();

            // Assert
            //Assert.IsNotNull(_viewModel.SelectedProject);
            //Assert.IsNotNull(_viewModel.SelectedExecutor);
            Assert.AreEqual(1, _viewModel.SelectedProject.ProjectID);
            Assert.AreEqual(2, _viewModel.SelectedExecutor.EmployeeID);
        }

        [TestMethod]
        public void SaveTask_ValidData_UpdatesTask()
        {
            // Arrange
            _taskServiceMock.Setup(ts => ts.UpdateTask(It.IsAny<tbTask>())).Verifiable();
            _loggingServiceMock.Setup(ls => ls.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            _viewModel.SaveTask();

            // Assert
            _taskServiceMock.Verify(ts => ts.UpdateTask(It.IsAny<tbTask>()), Times.Once);
            _loggingServiceMock.Verify(ls => ls.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
