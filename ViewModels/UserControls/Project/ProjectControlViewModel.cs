using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using System.Windows.Controls;
using MyKpiyapProject.ViewModels.Commands;
using MyKpiyapProject.UserControls;
using MyKpiyapProject.UserControls.EditControls;
using System.Windows;
using System.IO;
using System.IO.Compression;
using MyKpiyapProject.ViewModels.UserControls.Task;
using MyKpiyapProject.Views.UserControls.Task;
using MyKpiyapProject.Views.UserControls.Project;

namespace MyKpiyapProject.ViewModels.UserControls.Project
{
    public class ProjectControlViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private ObservableCollection<tbProject> _projects;
        private ProjectService _projectService;
        private UserControl _currentControl;
        private UserControl _secondControl;
        private bool _isLoading;
        private tbProject _selectedProject;
        private ObservableCollection<tbProject> _allProject;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;

        private string _currentFilterStatus = "Все"; // Текущий выбранный фильтр
        private string _selectedStatus = "Все";
        private string _searchText;
        private string _projectCountText;

        public string ProjectCountText
        {
            get => _projectCountText;
            private set
            {
                _projectCountText = value;
                OnPropertyChanged();
            }
        }
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters(); // Применяем фильтры при изменении текста
            }
        }
        public string SelectedRole
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
            }
        }
        public tbProject SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // Обновляем состояние команд
            }
        }
        public ObservableCollection<tbProject> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                OnPropertyChanged();
            }
        }
        public ProjectService ProjectServis
        {
            get { return _projectService; }
            set
            {
                _projectService = value;
                OnPropertyChanged();
            }
        }
        public UserControl CurrentControl
        {
            get { return _currentControl; }
            set
            {
                _currentControl = value;
                OnPropertyChanged();
            }
        }
        public UserControl SecondControl
        {
            get { return _secondControl; }
            set
            {
                _secondControl = value;
                OnPropertyChanged(nameof(SecondControl));
            }
        }



        public ICommand LoadAddProjectControlCommand { get; }
        public ICommand LoadEditProjectControlCommand { get; }
        public ICommand LoadProjectCommand { get; }
        public ICommand RemoveProjectCommand { get; }
        public ICommand FilterByStatusCommand { get; }
        public ICommand OpenDocxCommand { get; }
        public ICommand LoadDataProjCommand { get; }

        public ProjectControlViewModel() { }

        public ProjectControlViewModel(tbEmployee tbEmployee)
        {
            if (tbEmployee == null)
            {
                throw new ArgumentNullException(nameof(tbEmployee), "tbEmployee cannot be null");
            }

            ProjectServis = new ProjectService();
            Projects = new ObservableCollection<tbProject>();

            myUser = tbEmployee;
            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();

            LoadAddProjectControlCommand = new RelayCommand(_ => LoadAddProjectControl());
            LoadEditProjectControlCommand = new RelayCommand(_ => LoadEditProjectControl(), _ => SelectedProject != null);
            LoadProjectCommand = new RelayCommand(async _ => await LoadProject(), _ => !_isLoading);
            RemoveProjectCommand = new RelayCommand(_ => RemoveProject(), _ => SelectedProject != null);
            FilterByStatusCommand = new RelayCommand(FilterByStatus);
            OpenDocxCommand = new RelayCommand(OpenProjectDocument);
            LoadDataProjCommand = new RelayCommand(_ => LoadDataProj());

            RefreshData();
        }

        private void LoadDataProj()
        {
            if (SelectedProject != null)
            {
                var currentProj = SelectedProject; // Сохраняем выбранную задачу
                SecondControl = null; // Сначала очищаем
                SecondControl = new ViewinProjectControl(currentProj, RefreshData, myUser)
                {
                    DataContext = new ViewingProjectViewModel(currentProj, RefreshData, myUser)
                };

                CurrentControl = null;
            }
        }


        private void OpenProjectDocument(object parameter)
        {
            if (parameter is tbProject project)
            {
                if (project.DocxData == null || project.DocxData.DocxData.Length == 0)
                {
                    MessageBox.Show("Документ не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string extension = GetFileExtension(project.DocxData.DocxData);

                if (extension == null)
                {
                    MessageBox.Show("Неподдерживаемый формат файла. Разрешены только DOC, DOCX, XLS, XLSX.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    string tempFile = Path.GetTempFileName();
                    string finalPath = Path.ChangeExtension(tempFile, extension);
                    File.Move(tempFile, finalPath);
                    File.WriteAllBytes(finalPath, project.DocxData.DocxData);

                    OpenOfficeFile(finalPath, extension);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenOfficeFile(string filePath, string extension)
        {
            try
            {
                switch (extension.ToLower())
                {
                    case ".doc":
                    case ".docx":
                        var wordApp = new Microsoft.Office.Interop.Word.Application();
                        wordApp.Documents.Open(filePath);
                        wordApp.Visible = true;
                        break;

                    case ".xls":
                    case ".xlsx":
                        var excelApp = new Microsoft.Office.Interop.Excel.Application();
                        excelApp.Workbooks.Open(filePath);
                        excelApp.Visible = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл в Office: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Удаление временного файла через 30 секунд
                System.Threading.Tasks.Task.Delay(30000).ContinueWith(_ =>
                {
                    try { File.Delete(filePath); }
                    catch { }
                });
            }
        }

        private string GetFileExtension(byte[] fileData)
        {
            if (fileData == null || fileData.Length < 8)
                return null; // Недостаточно данных для определения

            // Проверка на DOCX/XLSX (ZIP-архив)
            if (fileData[0] == 0x50 && fileData[1] == 0x4B) // PK-сигнатура
            {
                try
                {
                    using (var stream = new MemoryStream(fileData))
                    using (var archive = new ZipArchive(stream))
                    {
                        // Проверка на Word
                        if (archive.Entries.Any(e => e.FullName.StartsWith("word/")))
                            return ".docx";

                        // Проверка на Excel
                        if (archive.Entries.Any(e => e.FullName.StartsWith("xl/")))
                            return ".xlsx";
                    }
                }
                catch { }
                return null; // ZIP, но не Office документ
            }

            // Проверка на старые форматы (OLE)
            if (fileData[0] == 0xD0 && fileData[1] == 0xCF) // OLE-сигнатура
            {
                // Ищем маркеры Word или Excel
                for (int i = 512; i < fileData.Length - 4; i++)
                {
                    // Word
                    if (fileData[i] == 0x57 && fileData[i + 1] == 0x6F && // "Wo"
                        fileData[i + 2] == 0x72 && fileData[i + 3] == 0x64) // "rd"
                        return ".doc";

                    // Excel
                    if (fileData[i] == 0x57 && fileData[i + 1] == 0x6F && // "Wo"
                        fileData[i + 2] == 0x72 && fileData[i + 3] == 0x6B) // "rk"
                        return ".xls";
                }
            }

            return null; // Неизвестный формат
        }

        private void OpenFileWithCorrectApp(string filePath, string extension)
        {
            try
            {
                switch (extension.ToLower())
                {
                    case ".doc":
                    case ".docx":
                        var wordApp = new Microsoft.Office.Interop.Word.Application();
                        wordApp.Documents.Open(filePath);
                        wordApp.Visible = true;
                        break;

                    case ".xls":
                    case ".xlsx":
                        var excelApp = new Microsoft.Office.Interop.Excel.Application();
                        excelApp.Workbooks.Open(filePath);
                        excelApp.Visible = true;
                        break;

                    default:
                        // Пробуем открыть через ассоциированное приложение
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OpenFile(string filePath)
        {
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();

                switch (extension)
                {
                    case ".xls":
                    case ".xlsx":
                        OpenExcelFile(filePath);
                        break;

                    case ".doc":
                    case ".docx":
                        OpenWordFile(filePath);
                        break;

                    default:
                        // Попробуем открыть через ассоциацию приложений
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Удаляем временный файл через 30 секунд
                System.Threading.Tasks.Task.Delay(30000).ContinueWith(t =>
                {
                    try { File.Delete(filePath); }
                    catch { /* Игнорируем ошибки удаления */ }
                });
            }
        }

        private void OpenExcelFile(string filePath)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Workbooks.Open(filePath);
            excelApp.Visible = true;
        }

        private void OpenWordFile(string filePath)
        {
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Documents.Open(filePath);
            wordApp.Visible = true;
        }



        private void FilterByStatus(object role)
        {
            _currentFilterStatus = role?.ToString() ?? "Все";
            SelectedRole = _currentFilterStatus; 
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allProject == null) return;

            IEnumerable<tbProject> filtered = _allProject;

            // Фильтрация по роли
            if (!string.IsNullOrEmpty(_selectedStatus) && _selectedStatus != "Все")
            {
                filtered = filtered.Where(e => e.Status.StatusName == _selectedStatus);
            }

            // Фильтрация по тексту
            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchTextLower = SearchText.ToLower();
                filtered = filtered.Where(e =>
                    e.Title.ToLower().Contains(searchTextLower) ||
                    e.Status.StatusName.ToLower().Contains(searchTextLower) ||
                    e.DocumentType.ToLower().Contains(searchTextLower) ||
                    e.CreatorName.ToLower().Contains(searchTextLower));
            }

            

            Projects = new ObservableCollection<tbProject>(filtered);
            UpdateRecordCount();
        }

        private void UpdateRecordCount()
        {
            int count = Projects.Count;
            string ending = count % 100 is >= 11 and <= 14 ? "Проектов" :
                (count % 10) switch
                {
                    1 => "Проект",
                    2 or 3 or 4 => "Проекта",
                    _ => "Проектов"
                };

            ProjectCountText = $"{count} {ending}";
        }

        private async void RefreshData()
        {
            try
            {
                _isLoading = true;
                CommandManager.InvalidateRequerySuggested();

                var projects = await System.Threading.Tasks.Task.Run(() => _projectService.GetAllProjects());
                _allProject = new ObservableCollection<tbProject>(projects);
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке проектов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void LoadAddProjectControl()
        {
            if (myUser.PositionAndRole.PositionAndRoleName != "Админ" && myUser.PositionAndRole.PositionAndRoleName != "Менеджер")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CurrentControl = new AddProjectControl(RefreshData, myUser)
            {
                DataContext = new AddProjectControlViewModel(RefreshData, myUser)
            };
        }
        private async void RemoveProject()
        {
            if (myUser.PositionAndRole.PositionAndRoleName != "Админ" && myUser.PositionAndRole.PositionAndRoleName != "Менеджер")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (MessageBox.Show($"Удалить проект {SelectedProject.Title}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _projectService.DeleteProject(SelectedProject.ProjectID);
                    RefreshData();
                    await _loggingService.LogAction(myUser.EmployeeID, "Удвление проекта", "Операции с данными", "Успех");
                }
            }
            catch
            {
                await _loggingService.LogAction(myUser.EmployeeID, "Удвление проекта", "Операции с данными", "Ошибка");
            }
        }

        private void LoadEditProjectControl()
        {
            if (myUser.PositionAndRole.PositionAndRoleName != "Админ" && myUser.PositionAndRole.PositionAndRoleName != "Менеджер")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CurrentControl = new EditProjectControl(SelectedProject, RefreshData, myUser)
            {
                DataContext = new EditProjectControlViewModel(SelectedProject, RefreshData, myUser)
            };
        }

        private async System.Threading.Tasks.Task LoadProject()
        {
            try
            {
                _isLoading = true;
                CommandManager.InvalidateRequerySuggested();

                var projects = await System.Threading.Tasks.Task.Run(() =>
                    _projectService.GetAllProjects()); 

                _allProject = new ObservableCollection<tbProject>(projects);
                ApplyFilters();
                await _loggingService.LogAction(myUser.EmployeeID, "Загрузка проектов", "Операции с данными", "Успех");
            }
            finally
            {
                _isLoading = false;
                CommandManager.InvalidateRequerySuggested();
                await _loggingService.LogAction(myUser.EmployeeID, "Загрузка проектов", "Операции с данными", "Ошибка");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
