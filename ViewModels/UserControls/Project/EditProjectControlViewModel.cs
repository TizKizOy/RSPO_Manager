using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MyKpiyapProject.ViewModels.UserControls.Project
{
    public class EditProjectControlViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private readonly ProjectService _projectService;
        private readonly Action _refreshCallback;
        private tbProject _currentProject;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;
        private ProjectStatusService _projectStatusService;
        private ProjectDocumentService _projectDocumentService;

        private string _projectName;
        private string _projectDescription;
        private tbEmployee _selectedCreator;
        private string _status;
        private DateTime _startDate;
        private DateTime _endDate;
        private byte[] _documentData;
        private string _selectedFilePath;

        public ObservableCollection<tbEmployee> Creators { get; } = new ObservableCollection<tbEmployee>();

        public string ProjectName
        {
            get => _projectName;
            set { _projectName = value; OnPropertyChanged(); }
        }

        public string ProjectDescription
        {
            get => _projectDescription;
            set { _projectDescription = value; OnPropertyChanged(); }
        }

        public string CreatorName => SelectedCreator?.FullName;
        public tbEmployee SelectedCreator
        {
            get => _selectedCreator;
            set
            {
                _selectedCreator = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CreatorName)); 
            }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set { _selectedFilePath = value; OnPropertyChanged(); }
        }

        public ICommand SaveProjectCommand { get; }
        public ICommand SelectDocxCommand { get; }

        public EditProjectControlViewModel(tbProject project, Action refreshCallback, tbEmployee tbEmployee)
        {
            _projectService = new ProjectService();
            _currentProject = project;
            _refreshCallback = refreshCallback;

            myUser = tbEmployee;
            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();
            _projectStatusService = new ProjectStatusService();
            _projectDocumentService = new ProjectDocumentService();

            // Инициализация свойств из текущего проекта
            ProjectName = project.Title;
            ProjectDescription = project.Description;
            Status = project.Status.StatusName;
            StartDate = project.CreationDate;
            EndDate = project.ClosingDate;
            _documentData = project.DocxData.DocxData;

            // Загрузка списка создателей
            LoadCreators();

            SaveProjectCommand = new RelayCommand(_ => SaveProject(), CanSaveProject);
            SelectDocxCommand = new RelayCommand(_ => SelectOfficeFile());
        }

        private void LoadCreators()
        {
            var userService = new EmployeeService();
            var creators = userService.GetAllEmployees();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Creators.Clear();
                foreach (var creator in creators)
                {
                    Creators.Add(creator);
                }

                SelectedCreator = Creators.FirstOrDefault(c => c.EmployeeID == _currentProject.CreatorID);
                OnPropertyChanged(nameof(SelectedCreator));
                OnPropertyChanged(nameof(CreatorName));
            });
        }

        private bool CanSaveProject(object parameter)
        {
            return !string.IsNullOrWhiteSpace(ProjectName) &&
                   !string.IsNullOrWhiteSpace(ProjectDescription) &&
                   !string.IsNullOrWhiteSpace(Status) &&
                   SelectedCreator != null;
        }

        private void SelectOfficeFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Office Files|*.doc;*.docx;*.xls;*.xlsx",
                Title = "Выберите документ Office"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string extension = Path.GetExtension(openFileDialog.FileName).ToLower();
                    if (extension != ".doc" && extension != ".docx" &&
                        extension != ".xls" && extension != ".xlsx")
                    {
                        MessageBox.Show("Выберите файл формата DOC, DOCX, XLS или XLSX",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    SelectedFilePath = openFileDialog.FileName;
                    _documentData = File.ReadAllBytes(SelectedFilePath);
                    MessageBox.Show("Файл успешно загружен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SaveProject()
        {
            try
            {
                if (SelectedCreator == null)
                {
                    MessageBox.Show("Создатель проекта не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int statusId = _projectStatusService.GetProjectStatusIdByName(Status);
                int docId = _projectDocumentService.GetDocumentIdByData(_documentData);

                if (!ValidateProjectData(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedProject = new tbProject
                {
                    ProjectID = _currentProject.ProjectID,
                    Title = ProjectName,
                    Description = ProjectDescription,
                    StatusID = statusId,
                    CreationDate = StartDate,
                    ClosingDate = EndDate,
                    CreatorID = SelectedCreator.EmployeeID,
                    DocumentID = docId,
                };

                _projectService.UpdateProject(updatedProject);

                
                //OnPropertyChanged(nameof(ProjectName));
                //OnPropertyChanged(nameof(ProjectDescription));
                //OnPropertyChanged(nameof(CreatorName));

                _refreshCallback?.Invoke();

                await _loggingService.LogAction(myUser.EmployeeID, "Редактирование проекта", "Операции с данными", "Успех");
                MessageBox.Show("Проект успешно обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAction(myUser.EmployeeID, "Редактирование проекта", "Операции с данными", "Ошибка");
                MessageBox.Show($"Ошибка при сохранении проекта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Метод для валидации данных проекта
        private bool ValidateProjectData(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(ProjectName))
            {
                errorMessage = "Название проекта не может быть пустым.";
                return false;
            }

            if (StartDate == default)
            {
                errorMessage = "Дата создания проекта должна быть указана.";
                return false;
            }

            if (SelectedCreator == null)
            {
                errorMessage = "Необходимо указать создателя проекта.";
                return false;
            }

            return true;
        }


        //private async void SaveProject()
        //{
        //    try
        //    {
        //        int statusId = _projectStatusService.GetProjectStatusIdByName(Status);

        //        _currentProject.Title = ProjectName;
        //        _currentProject.Description = ProjectDescription;
        //        _currentProject.Status.StatusName = Status;
        //        _currentProject.Status.ProjectStatusID = statusId;
        //        _currentProject.CreationDate = StartDate;
        //        _currentProject.ClosingDate = EndDate;
        //        _currentProject.Creator = SelectedCreator;
        //        _currentProject.CreatorID = SelectedCreator.EmployeeID;
        //        _currentProject.CreatorName = SelectedCreator.FullName;


        //        if (_documentData != null)
        //        {
        //            _currentProject.DocxData.DocxData = _documentData;
        //        }

        //        _projectService.UpdateProject(_currentProject);

        //        // Принудительно обновляем привязки
        //        OnPropertyChanged(nameof(SelectedCreator));
        //        OnPropertyChanged(nameof(CreatorName));

        //        _refreshCallback?.Invoke();

        //        await _loggingService.LogAction(myUser.EmployeeID, "Редактирование проекта", "Операции с данными", "Успех");

        //        MessageBox.Show("Проект успешно обновлен", "Успех",
        //            MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _loggingService.LogAction(myUser.EmployeeID, "Редактирование проекта", "Операции с данными", "Ошибка");
        //        MessageBox.Show($"Ошибка при сохранении проекта: {ex.Message}",
        //            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}