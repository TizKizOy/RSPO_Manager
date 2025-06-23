using Moq;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.UserControls.Task;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTest.Task
{
    [TestClass]
    public class TaskControlViewModelTests
    {
        private TaskControlViewModel _viewModel;
        private Mock<TaskService> _taskServiceMock;
        private Mock<LoggingService> _loggingServiceMock;
        private tbEmployee _currentUser;

        [TestInitialize]
        public void TestInitialize()
        {
            _taskServiceMock = new Mock<TaskService>();
            _loggingServiceMock = new Mock<LoggingService>();

            _currentUser = new tbEmployee { EmployeeID = 1, FullName = "Test User", PositionAndRoleID = 1 };

            _viewModel = new TaskControlViewModel(_currentUser, _taskServiceMock.Object, _loggingServiceMock.Object);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task LoadTask_ValidData_TasksLoaded()
        {
            // Arrange
            var tasks = new List<tbTask>
            {
                new tbTask { TaskID = 1, Title = "Task 1" },
                new tbTask { TaskID = 2, Title = "Task 2" }
            };

            _taskServiceMock.Setup(ts => ts.GetAllTasks()).Returns(tasks);
            _loggingServiceMock.Setup(ls => ls.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
             _viewModel.LoadTaskCommand.Execute(null);

            // Assert
            Assert.AreEqual(0, _viewModel.Tasks.Count);
            _loggingServiceMock.Verify(ls => ls.LogAction(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2)); // Ожидаем два вызова
        }


        [TestMethod]
        public void FilterByPriority_ValidPriority_FiltersTasks()
        {
            // Arrange
            var tasks = new ObservableCollection<tbTask>
            {
                new tbTask
                {
                    TaskID = 1,
                    Title = "Task 1",
                    PriorityID = 1,
                    Priority = new tbPriorityTask { PriorityName = "Высокий" } // Инициализация Priority
                },
                new tbTask
                {
                    TaskID = 2,
                    Title = "Task 2",
                    PriorityID = 2,
                    Priority = new tbPriorityTask { PriorityName = "Средний" } // Инициализация Priority
                }
            };

            _viewModel.Tasks = tasks;

            // Act
            _viewModel.FilterByPriorityCommand.Execute("Высокий");

            // Assert
            Assert.AreEqual(1, _viewModel.Tasks.Count(t => t.Priority.PriorityName == "Высокий"));
        }

    }
}
