using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyKpiyapProject.ViewModels.UserControls
{
    public class PageNumberModel
    {
        public int PageNumber { get; set; }
        public bool IsCurrentPage { get; set; }
        public bool IsVisible { get; set; }
    }

    public class LogAdminControlViewModel : INotifyPropertyChanged
    {
        private readonly tbEmployee _myUser;
        private ObservableCollection<tbAdminLog> _adminLog;
        private ObservableCollection<tbAdminLog> _allAdminLog = new ObservableCollection<tbAdminLog>();
        private readonly AdminLogService _adminLogService;
        private readonly LoggingService _loggingService;
        private string _searchText;
        private int _currentPage = 1;
        private int _pageSize = 50;
        private int _totalPages;
        private bool _isLoading;

        public ObservableCollection<tbAdminLog> AdminLog
        {
            get => _adminLog;
            set
            {
                _adminLog = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayPageNumbers));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                CurrentPage = 1; // Reset to first page when filter changes
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayPageNumbers));
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayPageNumbers));
            }
        }

        public IEnumerable<PageNumberModel> DisplayPageNumbers
        {
            get
            {
                if (TotalPages == 0)
                    return Enumerable.Empty<PageNumberModel>();

                var maxButtons = 7; // Max number of page buttons to display
                var pageNumbers = new List<PageNumberModel>();
                int startPage, endPage;

                if (TotalPages <= maxButtons)
                {
                    // Show all pages if total pages are less than or equal to maxButtons
                    startPage = 1;
                    endPage = TotalPages;
                }
                else
                {
                    // Show first page, last page, current page, and a few pages around current
                    int sideButtons = maxButtons / 2; // Buttons on each side of current page
                    startPage = Math.Max(1, CurrentPage - sideButtons);
                    endPage = Math.Min(TotalPages, CurrentPage + sideButtons);

                    // Adjust if we're near the start or end
                    if (startPage <= 2)
                    {
                        startPage = 1;
                        endPage = maxButtons;
                    }
                    else if (endPage >= TotalPages - 1)
                    {
                        startPage = TotalPages - maxButtons + 1;
                        endPage = TotalPages;
                    }
                }

                // Add first page
                pageNumbers.Add(new PageNumberModel
                {
                    PageNumber = 1,
                    IsCurrentPage = CurrentPage == 1,
                    IsVisible = true
                });

                // Add ellipsis after first page if needed
                if (startPage > 2)
                {
                    pageNumbers.Add(new PageNumberModel
                    {
                        PageNumber = 0, // 0 represents ellipsis
                        IsCurrentPage = false,
                        IsVisible = false
                    });
                }

                // Add middle pages
                for (int i = Math.Max(2, startPage); i <= Math.Min(TotalPages - 1, endPage); i++)
                {
                    pageNumbers.Add(new PageNumberModel
                    {
                        PageNumber = i,
                        IsCurrentPage = CurrentPage == i,
                        IsVisible = true
                    });
                }

                // Add ellipsis before last page if needed
                if (endPage < TotalPages - 1)
                {
                    pageNumbers.Add(new PageNumberModel
                    {
                        PageNumber = 0, // 0 represents ellipsis
                        IsCurrentPage = false,
                        IsVisible = false
                    });
                }

                // Add last page
                if (TotalPages > 1)
                {
                    pageNumbers.Add(new PageNumberModel
                    {
                        PageNumber = TotalPages,
                        IsCurrentPage = CurrentPage == TotalPages,
                        IsVisible = true
                    });
                }

                return pageNumbers;
            }
        }

        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }
        public ICommand LoadDataCommand { get; }

        public LogAdminControlViewModel() { }

        public LogAdminControlViewModel(tbEmployee tbEmployee)
        {
            _myUser = tbEmployee;
            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();

            LoadDataCommand = new RelayCommand(async _ => await LoadAdminLog());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage(), _ => CurrentPage > 1);
            NextPageCommand = new RelayCommand(_ => NextPage(), _ => CurrentPage < TotalPages);
            GoToPageCommand = new RelayCommand(page => GoToPage(int.Parse(page.ToString())));

            LoadDataCommand.Execute(null);
        }

        private async System.Threading.Tasks.Task LoadAdminLog()
        {
            try
            {
                IsLoading = true;

                var logs = await _adminLogService.GetAllAdminLogsIQueryable()
                    .Include(e => e.Employee)
                    .OrderByDescending(log => log.DateTime)
                    .ToListAsync();

                _allAdminLog = new ObservableCollection<tbAdminLog>(logs);
                TotalPages = (int)Math.Ceiling((double)_allAdminLog.Count / PageSize);
                ApplyFilters();
            }
            catch (Exception ex)
            {
                AdminLog = new ObservableCollection<tbAdminLog>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            if (_allAdminLog == null || !_allAdminLog.Any())
            {
                AdminLog = new ObservableCollection<tbAdminLog>();
                return;
            }

            IEnumerable<tbAdminLog> filtered = _allAdminLog;

            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchTextLower = SearchText.ToLower();
                filtered = filtered.Where(e =>
                    (e.Employee?.FullName?.ToLower().Contains(searchTextLower) == true ||
                    e.Status?.ToLower().Contains(searchTextLower) == true ||
                    e.Action?.ToLower().Contains(searchTextLower) == true ||
                    e.EventType?.ToLower().Contains(searchTextLower) == true));
            }

            TotalPages = (int)Math.Ceiling((double)filtered.Count() / PageSize);

            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }

            var pagedItems = filtered
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            AdminLog = new ObservableCollection<tbAdminLog>(pagedItems);
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                ApplyFilters();
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                ApplyFilters();
            }
        }

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
                ApplyFilters();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}