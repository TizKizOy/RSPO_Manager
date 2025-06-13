using WpfDoc = System.Windows.Documents; // Псевдоним для WPF Documents
using WordDoc = Microsoft.Office.Interop.Word; // Псевдоним для Word Interop
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.UserControls;
using MyKpiyapProject.ViewModels.Commands;
using MyKpiyapProject.Views.UserControls.Report;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Office.Interop.Word;
using System;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MyKpiyapProject.ViewModels.UserControls.Report
{
    public class ReportControlViewModel : INotifyPropertyChanged
    {
        private ReportService reportService;
        private ObservableCollection<tbReport> allReport;
        private ObservableCollection<tbReport> currentReport;
        private tbReport selectedReport;
        private UserControl _currentControl;
        private UserControl _secondControl;
        private UserControl _centerControl;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;

        private tbEmployee myUser;
        private DateTime creationDate;
        private string title;
        private string description;
        private string employeeName;
        private string workExpenses;
        private string projectName;
        private string taskName;
        private string status;

        private bool _isLoading;
        private string currentFilterStatus = "Все"; // Текущий выбранный фильтр
        private string selectedStatus = "Все";
        private string searchText;
        private string countText;

        public string CountText
        {
            get { return countText; }
            set { countText = value; OnPropertyChanged(); }
        }

        public UserControl CenterControl
        {
            get { return _centerControl; }
            set { _centerControl = value; OnPropertyChanged(); }
        }
        public UserControl CurrentControl
        {
            get { return _currentControl; }
            set
            {
                _currentControl = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentControl));
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
        public string SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                OnPropertyChanged();
            }
        }
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
                ApplyFilters(); // Применяем фильтры при изменении текста
            }
        }
        public tbReport SelectedReport
        {
            get => selectedReport;
            set
            {
                selectedReport = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // Обновляем состояние команд
            }
        }
        public ObservableCollection<tbReport> CurrentReport
        {
            get { return currentReport; }
            set
            {
                currentReport = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentReport));
            }
        }
        public ReportService ReportService
        {
            get { return reportService; }
            set
            {
                reportService = value;
                OnPropertyChanged();
            }
        }

        public DateTime CreationDateString
        {
            get { return creationDate; }
            set { creationDate = value; OnPropertyChanged(); }
        }
        public string Title
        {
            get => title;
            set { title = value; OnPropertyChanged(); }
        }
        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(); }
        }
        public string EmployeeName
        {
            get => employeeName;
            set { employeeName = value; OnPropertyChanged(); }
        }
        public string WorkExpenses
        {
            get => workExpenses;
            set { workExpenses = value; OnPropertyChanged(); }
        }
        public string ProjectName
        {
            get => projectName;
            set { projectName = value; OnPropertyChanged(); }
        }
        public string TaskName
        {
            get => taskName;
            set { taskName = value; OnPropertyChanged(); }
        }
        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(); }
        }


        public ICommand FilterByStatusCommand { get; }
        public ICommand LoadReportCommand { get; }
        public ICommand RemoveReportCommand { get; }


        public ICommand PrintReportCommand { get; }
        public ICommand LoadAddReportControlCommand { get; }
        public ICommand LoadProjectReportFormCommand { get; }
        public ICommand LoadTaskReportFormCommand { get; }
        public ICommand LoadTimeReportFormCommand { get; }
        public ICommand LoadDataReportCommand { get; }


        public ReportControlViewModel() { }

        public ReportControlViewModel(tbEmployee tbEmployee)
        {
            myUser = tbEmployee;
            ReportService = new ReportService();
            CurrentReport = new ObservableCollection<tbReport>();

            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();

            LoadReportCommand = new RelayCommand(async _ => await LoadReport(), _ => !_isLoading);
            LoadAddReportControlCommand = CreateCommand(() => new ChoosingReportControl(this));
            //LoadProjectReportFormCommand = CreateCommand(() => new AddReportControl() { DataContext = new AddReportViewModel(myUser, RefreshData) });
            LoadProjectReportFormCommand = CreateCommandWithViewModel<AddReportControl, AddReportViewModel>(
             () => new AddReportViewModel(myUser, RefreshData));

            LoadTaskReportFormCommand = CreateCommandWithViewModel<AddReportTaskControl, AddReportTaskViewModel>(
             () => new AddReportTaskViewModel(myUser, RefreshData));

            LoadTimeReportFormCommand = CreateCommandWithViewModel<AddReportTimeIntervalControl, AddReportTimeIntervalViewModel>(
             () => new AddReportTimeIntervalViewModel(myUser, RefreshData));
            FilterByStatusCommand = new RelayCommand(FilterByStatus);
            RemoveReportCommand = new RelayCommand(_ => RemoveReport());
            PrintReportCommand = new RelayCommand(_ => PrintReport());
            LoadDataReportCommand = CreateCommand2(() => new ViewingReportControl(SelectedReport));

            LoadReportCommand.Execute(null);
        }

        private async void PrintReport()
        {
            if (SelectedReport == null)
            {
                MessageBox.Show("Необходимо выбрать запись для создания отчёта");
                return;
            }

            try
            {
                // Create FlowDocument
                WpfDoc.FlowDocument flowDocument = new WpfDoc.FlowDocument
                {
                    FontFamily = new System.Windows.Media.FontFamily("Times New Roman"),
                    FontSize = 14,
                    TextAlignment = TextAlignment.Justify,
                    LineHeight = 21 // 1.5 строки при 14 pt
                };

                // Company header
                WpfDoc.Paragraph companyPara = new WpfDoc.Paragraph(new WpfDoc.Run("ООО \"Строительная компания TileHaus\""))
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                flowDocument.Blocks.Add(companyPara);

                // Report date
                WpfDoc.Paragraph datePara = new WpfDoc.Paragraph(new WpfDoc.Run($"Дата составления отчёта: {DateTime.Now:dd.MM.yyyy HH:mm:ss}"))
                {
                    TextAlignment = TextAlignment.Justify,
                    Margin = new Thickness(35.4, 0, 0, 0) // Абзацный отступ 1.25 см
                };
                flowDocument.Blocks.Add(datePara);

                // Report title
                WpfDoc.Paragraph titlePara = new WpfDoc.Paragraph(new WpfDoc.Run("Отчёт по управлению проектами и задачами"))
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                flowDocument.Blocks.Add(titlePara);

                // Introduction
                WpfDoc.Paragraph introPara = new WpfDoc.Paragraph(new WpfDoc.Run("Настоящий отчёт подготовлен в рамках текущей деятельности строительной компании TileHaus, специализирующейся на выполнении широкого спектра строительных и монтажных работ. Документ предназначен для предоставления полной и достоверной информации о ходе выполнения проектов и задач, а также для анализа эффективности использования ресурсов и трудовых затрат. Отчёт составлен на основе данных, собранных в процессе выполнения работ, и отражает текущее состояние дел на момент составления документа."))
                {
                    Margin = new Thickness(35.4, 0, 0, 0)
                };
                flowDocument.Blocks.Add(introPara);

                // Employee information
                WpfDoc.Paragraph empPara = new WpfDoc.Paragraph(new WpfDoc.Run("Сведения о сотруднике, ответственном за выполнение работ:"))
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                flowDocument.Blocks.Add(empPara);

                // Employee details list
                WpfDoc.List empList = new WpfDoc.List();
                empList.ListItems.Add(new WpfDoc.ListItem(new WpfDoc.Paragraph(new WpfDoc.Run($"- Идентификатор сотрудника: {SelectedReport.EmployeeID ?? 0}. Данный идентификатор используется для учёта сотрудников в системе управления персоналом компании."))));
                empList.ListItems.Add(new WpfDoc.ListItem(new WpfDoc.Paragraph(new WpfDoc.Run($"- Полное имя сотрудника: {SelectedReport.EmployeeName ?? "Не указано"}. Указанный сотрудник является ответственным лицом, выполняющим контроль и непосредственное участие в реализации задач и проектов."))));
                empList.ListItems.Add(new WpfDoc.ListItem(new WpfDoc.Paragraph(new WpfDoc.Run($"- Затраты, связанные с выполнением работ: {SelectedReport.WorkExpenses ?? "0"} руб. Данные затраты включают в себя расходы на материалы, оборудование, транспорт и прочие операционные издержки, возникшие в процессе выполнения работ."))));
                flowDocument.Blocks.Add(empList);

                // Report section
                string reportTypeSection = "";
                if (SelectedReport.ProjectID != null && SelectedReport.TaskID == null)
                {
                    reportTypeSection = $@"Отчёт по проекту

Данный раздел отчёта посвящён описанию проекта, выполняемого строительной компанией TileHaus в рамках её основной деятельности:

- Идентификатор проекта: {SelectedReport.ProjectID}. Уникальный идентификатор позволяет отслеживать проект в системе управления и координировать действия всех участников процесса.
- Название проекта: {SelectedReport.ProjectName}. Название отражает основное направление проекта, например, строительство жилого комплекса, промышленного объекта или реконструкция существующего здания.
- Текущий статус выполнения: {SelectedReport.Status}. Статус отражает текущую фазу выполнения проекта, включая информацию о завершённых этапах, текущих работах и возможных задержках, которые могут повлиять на общий график реализации проекта.
- Описание:{SelectedReport.Project.Description}. Настоящий проект представляет собой комплекс строительных работ, направленных на реализацию ключевых этапов строительства. Работы выполняются в строгом соответствии с утверждённым планом, строительными нормами и правилами, а также с учётом требований заказчика.";
                }
                else if (SelectedReport.TaskID != null)
                {
                    reportTypeSection = $@"Отчёт по проекту

Данный раздел отчёта посвящён описанию проекта, в рамках которого выполняются конкретные задачи, направленные на достижение поставленных целей строительной компании TileHaus:

- Идентификатор проекта: {SelectedReport.ProjectID}. Уникальный идентификатор позволяет отслеживать проект в системе управления и координировать действия всех участников процесса.
- Название проекта: {SelectedReport.ProjectName}. Название отражает основное направление проекта, например, строительство жилого комплекса, промышленного объекта или реконструкция существующего здания.
- Текущий статус выполнения: {SelectedReport.Status}. Статус отражает текущую фазу выполнения проекта, включая информацию о завершённых этапах, текущих работах и возможных задержках, которые могут повлиять на общий график реализации проекта.
- Описание: {SelectedReport.Project.Description} Настоящий проект включает в себя выполнение связанных задач, направленных на достижение конечного результата. Работы выполняются в строгом соответствии с утверждённым планом, строительными нормами и правилами, а также с учётом требований заказчика.

Отчёт по задаче

В данном разделе представлена информация о конкретной задаче, выполняемой в рамках указанного проекта, с целью детального анализа её текущего состояния:

- Идентификатор задачи: {SelectedReport.TaskID}. Уникальный идентификатор задачи используется для её учёта и контроля в системе управления задачами.
- Название задачи: {SelectedReport.TaskName}. Название задачи отражает её содержание, например, закупка строительных материалов, установка оборудования или контроль качества выполненных работ.
- Текущий статус выполнения: {SelectedReport.Status}. Статус задачи позволяет оценить её прогресс, выявить возможные проблемы и определить, какие действия необходимы для её завершения в установленные сроки.
- Описание: {SelectedReport.Task.Description} Данная задача является частью более крупного проекта и включает в себя выполнение конкретных действий, направленных на достижение промежуточных целей. Выполнение задачи осуществляется в соответствии с утверждённым планом и под контролем ответственного сотрудника.";
                }
                else if (SelectedReport.CountEndTask > 0 || SelectedReport.CountEndProject > 0)
                {
                    reportTypeSection = $@"Отчёт за период времени

Настоящий раздел отчёта содержит обобщённую информацию о выполнении строительных проектов и задач за определённый временной период, что позволяет оценить общую эффективность работы компании TileHaus:

- Начало периода: {SelectedReport.CreationDate:dd.MM.yyyy}. Указанная дата соответствует началу периода, за который проводится анализ выполненных работ.
- Окончание периода: {SelectedReport.FinishDate:dd.MM.yyyy}. Дата окончания периода совпадает с моментом составления настоящего отчёта, что позволяет учитывать самые актуальные данные.
- Общий статус за период: {SelectedReport.Status}. Статус отражает общее состояние выполнения работ за указанный период, включая достигнутый прогресс, выявленные проблемы и возможные риски, которые могут повлиять на дальнейшую деятельность.
- Описание: {SelectedReport.Description} Настоящий отчёт охватывает указанный временной период и предоставляет обзор выполнения работ. В рамках данного периода анализируются все проекты и задачи, выполняемые компанией, с целью выявления ключевых достижений, а также проблемных моментов, требующих дополнительного внимания со стороны руководства.";
                }

                if (string.IsNullOrEmpty(reportTypeSection))
                {
                    reportTypeSection = "Тип отчёта не определён. Проверьте данные.";
                }

                // Add report section
                string[] sections = reportTypeSection.Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string section in sections)
                {
                    if (section.StartsWith("Отчёт по") || section.StartsWith("Отчёт за"))
                    {
                        string[] lines = section.Split(new[] { "\r\n" }, StringSplitOptions.None);
                        WpfDoc.Paragraph headerPara = new WpfDoc.Paragraph(new WpfDoc.Run(lines[0]))
                        {
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 10, 0, 0)
                        };
                        flowDocument.Blocks.Add(headerPara);

                        WpfDoc.List sectionList = new WpfDoc.List();
                        for (int i = 1; i < lines.Length; i++)
                        {
                            if (lines[i].StartsWith("- "))
                            {
                                sectionList.ListItems.Add(new WpfDoc.ListItem(new WpfDoc.Paragraph(new WpfDoc.Run(lines[i]))));
                            }
                            else
                            {
                                WpfDoc.Paragraph para = new WpfDoc.Paragraph(new WpfDoc.Run(lines[i]))
                                {
                                    Margin = new Thickness(35.4, 0, 0, 0)
                                };
                                flowDocument.Blocks.Add(para);
                            }
                        }
                        if (sectionList.ListItems.Count > 0)
                        {
                            flowDocument.Blocks.Add(sectionList);
                        }
                    }
                    else
                    {
                        WpfDoc.Paragraph para = new WpfDoc.Paragraph(new WpfDoc.Run(section))
                        {
                            Margin = new Thickness(35.4, 0, 0, 0)
                        };
                        flowDocument.Blocks.Add(para);
                    }
                }

                // Conclusion
                WpfDoc.Paragraph conclusionPara = new WpfDoc.Paragraph(new WpfDoc.Run("Заключение: Настоящий отчёт отражает текущее состояние выполнения строительных проектов и задач, выполняемых компанией TileHaus. Все данные, представленные в отчёте, основаны на информации, собранной в процессе выполнения работ, и могут быть использованы для дальнейшего планирования, анализа эффективности и принятия управленческих решений. При необходимости более детального анализа рекомендуется обратиться к дополнительным документам и отчётам, хранящимся в архиве компании."))
                {
                    Margin = new Thickness(35.4, 0, 0, 0)
                };
                flowDocument.Blocks.Add(conclusionPara);

                // Signature
                WpfDoc.Paragraph signaturePara = new WpfDoc.Paragraph(new WpfDoc.Run("Подпись ответственного лица:"))
                {
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                flowDocument.Blocks.Add(signaturePara);

                WpfDoc.Paragraph namePara = new WpfDoc.Paragraph(new WpfDoc.Run(SelectedReport.EmployeeName ?? "Не указано"))
                {
                    TextAlignment = TextAlignment.Right
                };
                flowDocument.Blocks.Add(namePara);

                WpfDoc.Paragraph dateSignPara = new WpfDoc.Paragraph(new WpfDoc.Run($"Дата: {SelectedReport.CreationDate:dd.MM.yyyy HH:mm:ss}"))
                {
                    TextAlignment = TextAlignment.Right
                };
                flowDocument.Blocks.Add(dateSignPara);

                // Convert FlowDocument to FixedDocument
                WpfDoc.FixedDocument fixedDocument = new WpfDoc.FixedDocument();
                WpfDoc.PageContent pageContent = new WpfDoc.PageContent();
                WpfDoc.FixedPage fixedPage = new WpfDoc.FixedPage();

                // Copy the content of FlowDocument
                WpfDoc.TextRange textRange = new WpfDoc.TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                TextBlock textBlock = new TextBlock
                {
                    Text = textRange.Text,
                    Margin = new Thickness(28.35, 56.7, 28.35, 56.7), // Отступы по ГОСТ
                    FontFamily = new System.Windows.Media.FontFamily("Times New Roman"),
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap
                };
                fixedPage.Children.Add(textBlock);

                fixedPage.Width = 8.5 * 96; // A4
                fixedPage.Height = 11 * 96;

                ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
                fixedDocument.Pages.Add(pageContent);

                // Print
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(fixedDocument.DocumentPaginator, $"Печать отчета: {SelectedReport.Title}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации и печати отчета: {ex.Message}");
            }



            //await System.Threading.Tasks.Task.Run(async () =>
            //{
            //    if (SelectedReport == null)
            //    {
            //        MessageBox.Show("Необходимо выбрать запись для создания отчёта");
            //        return;
            //    }

            //    Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            //    Document wordDoc = wordApp.Documents.Add();

            //    try
            //    {
            //        // Настройка страницы по ГОСТ
            //        wordDoc.PageSetup.LeftMargin = 28.35f; // 10 мм
            //        wordDoc.PageSetup.RightMargin = 28.35f; // 10 мм
            //        wordDoc.PageSetup.TopMargin = 56.7f;   // 20 мм
            //        wordDoc.PageSetup.BottomMargin = 56.7f; // 20 мм
            //        wordDoc.PageSetup.Orientation = WdOrientation.wdOrientPortrait;

            //        // Применение стандартных стилей
            //        Microsoft.Office.Interop.Word.Style normalStyle = wordDoc.Styles[WdBuiltinStyle.wdStyleNormal];
            //        normalStyle.Font.Name = "Times New Roman";
            //        normalStyle.Font.Size = 14;
            //        wordDoc.Content.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            //        normalStyle.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpace1pt5;
            //        normalStyle.ParagraphFormat.FirstLineIndent = 35.4f; // Абзацный отступ 1.25 см
            //        normalStyle.ParagraphFormat.SpaceAfter = 0;

            //        // Стиль для заголовков
            //        Microsoft.Office.Interop.Word.Style headingStyle = wordDoc.Styles.Add("HeadingStyle", WdStyleType.wdStyleTypeParagraph);
            //        headingStyle.Font.Name = "Times New Roman";
            //        headingStyle.Font.Size = 14;
            //        headingStyle.Font.Bold = 1;
            //        headingStyle.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            //        headingStyle.ParagraphFormat.SpaceAfter = 0;

            //        // Стиль для подзаголовков
            //        Microsoft.Office.Interop.Word.Style subheadingStyle = wordDoc.Styles.Add("SubheadingStyle", WdStyleType.wdStyleTypeParagraph);
            //        subheadingStyle.Font.Name = "Times New Roman";
            //        subheadingStyle.Font.Size = 14;
            //        subheadingStyle.Font.Bold = 1;
            //        subheadingStyle.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            //        subheadingStyle.ParagraphFormat.SpaceAfter = 0;

            //        // Заголовок компании
            //        Paragraph companyPara = wordDoc.Content.Paragraphs.Add();
            //        companyPara.Range.Text = "ООО \"Строительная компания TileHaus\"";
            //        companyPara.set_Style(headingStyle);
            //        companyPara.Range.InsertParagraphAfter();

            //        // Дата составления
            //        Paragraph datePara = wordDoc.Content.Paragraphs.Add();
            //        datePara.Range.Text = $"Дата составления отчёта: {SelectedReport.CreationDate:dd.MM.yyyy HH:mm}";
            //        datePara.set_Style(normalStyle);
            //        datePara.Range.InsertParagraphAfter();

            //        // Пустая строка
            //        wordDoc.Content.Paragraphs.Add().Range.InsertParagraphAfter();

            //        // Заголовок отчёта
            //        Paragraph titlePara = wordDoc.Content.Paragraphs.Add();
            //        titlePara.Range.Text = "Отчёт по управлению проектами и задачами";
            //        titlePara.set_Style(headingStyle);
            //        titlePara.Range.InsertParagraphAfter();

            //        // Введение
            //        Paragraph introPara = wordDoc.Content.Paragraphs.Add();
            //        introPara.Range.Text = "Настоящий отчёт подготовлен в рамках текущей деятельности строительной компании TileHaus, специализирующейся на выполнении широкого спектра строительных и монтажных работ. Документ предназначен для предоставления полной и достоверной информации о ходе выполнения проектов и задач, а также для анализа эффективности использования ресурсов и трудовых затрат. Отчёт составлен на основе данных, собранных в процессе выполнения работ, и отражает текущее состояние дел на момент составления документа.";
            //        introPara.set_Style(normalStyle);
            //        introPara.Range.InsertParagraphAfter();

            //        // Сведения о сотруднике
            //        Paragraph empPara = wordDoc.Content.Paragraphs.Add();
            //        empPara.Range.Text = "Сведения о сотруднике, ответственном за выполнение работ:";
            //        empPara.set_Style(subheadingStyle);
            //        empPara.Range.InsertParagraphAfter();

            //        // Добавление пунктов списка
            //        AddListItem(wordDoc, $"- Идентификатор сотрудника: {SelectedReport.EmployeeID ?? 0}. Данный идентификатор используется для учёта сотрудников в системе управления персоналом компании.");
            //        AddListItem(wordDoc, $"- Полное имя сотрудника: {SelectedReport.EmployeeName ?? "Не указано"}. Указанный сотрудник является ответственным лицом, выполняющим контроль и непосредственное участие в реализации задач и проектов.");
            //        AddListItem(wordDoc, $"- Затраты, связанные с выполнением работ: {SelectedReport.WorkExpenses ?? "0"} руб. Данные затраты включают в себя расходы на материалы, оборудование, транспорт и прочие операционные издержки, возникшие в процессе выполнения работ.");
            //        wordDoc.Content.Paragraphs.Add().Range.InsertParagraphAfter();

            //        // Раздел отчёта
            //        string reportTypeSection = "";

            //        if (SelectedReport.ProjectID != null && SelectedReport.TaskID == null)
            //        {
            //            reportTypeSection = $@"Отчёт по проекту

            //            Данный раздел отчёта посвящён описанию проекта, выполняемого строительной компанией TileHaus в рамках её основной деятельности:

            //            - Идентификатор проекта: {SelectedReport.ProjectID}. Уникальный идентификатор позволяет отслеживать проект в системе управления и координировать действия всех участников процесса.
            //            - Название проекта: {SelectedReport.ProjectName}. Название отражает основное направление проекта, например, строительство жилого комплекса, промышленного объекта или реконструкция существующего здания.
            //            - Текущий статус выполнения: {SelectedReport.Status}. Статус отражает текущую фазу выполнения проекта, включая информацию о завершённых этапах, текущих работах и возможных задержках, которые могут повлиять на общий график реализации проекта.
            //            - Примечание: Настоящий проект представляет собой комплекс строительных работ, направленных на реализацию ключевых этапов строительства. Работы выполняются в строгом соответствии с утверждённым планом, строительными нормами и правилами, а также с учётом требований заказчика.";
            //        }
            //        else if (SelectedReport.TaskID != null)
            //        {
            //            reportTypeSection = $@"Отчёт по проекту

            //            Данный раздел отчёта посвящён описанию проекта, в рамках которого выполняются конкретные задачи, направленные на достижение поставленных целей строительной компании TileHaus:

            //            - Идентификатор проекта: {SelectedReport.ProjectID}. Уникальный идентификатор позволяет отслеживать проект в системе управления и координировать действия всех участников процесса.
            //            - Название проекта: {SelectedReport.ProjectName}. Название отражает основное направление проекта, например, строительство жилого комплекса, промышленного объекта или реконструкция существующего здания.
            //            - Текущий статус выполнения: {SelectedReport.Status}. Статус отражает текущую фазу выполнения проекта, включая информацию о завершённых этапах, текущих работах и возможных задержках, которые могут повлиять на общий график реализации проекта.
            //            - Примечание: Настоящий проект включает в себя выполнение связанных задач, направленных на достижение конечного результата. Работы выполняются в строгом соответствии с утверждённым планом, строительными нормами и правилами, а также с учётом требований заказчика.

            //            Отчёт по задаче

            //            В данном разделе представлена информация о конкретной задаче, выполняемой в рамках указанного проекта, с целью детального анализа её текущего состояния:

            //            - Идентификатор задачи: {SelectedReport.TaskID}. Уникальный идентификатор задачи используется для её учёта и контроля в системе управления задачами.
            //            - Название задачи: {SelectedReport.TaskName}. Название задачи отражает её содержание, например, закупка строительных материалов, установка оборудования или контроль качества выполненных работ.
            //            - Текущий статус выполнения: {SelectedReport.Status}. Статус задачи позволяет оценить её прогресс, выявить возможные проблемы и определить, какие действия необходимы для её завершения в установленные сроки.
            //            - Примечание: Данная задача является частью более крупного проекта и включает в себя выполнение конкретных действий, направленных на достижение промежуточных целей. Выполнение задачи осуществляется в соответствии с утверждённым планом и под контролем ответственного сотрудника.";
            //        }
            //        else if (SelectedReport.CountEndTask > 0 || SelectedReport.CountEndProject > 0)
            //        {
            //            reportTypeSection = $@"Отчёт за период времени

            //            Настоящий раздел отчёта содержит обобщённую информацию о выполнении строительных проектов и задач за определённый временной период, что позволяет оценить общую эффективность работы компании TileHaus:

            //            - Начало периода: {DateTime.Now.AddDays(-30):dd.MM.yyyy}. Указанная дата соответствует началу периода, за который проводится анализ выполненных работ.
            //            - Окончание периода: {DateTime.Now:dd.MM.yyyy}. Дата окончания периода совпадает с моментом составления настоящего отчёта, что позволяет учитывать самые актуальные данные.
            //            - Общий статус за период: {SelectedReport.Status}. Статус отражает общее состояние выполнения работ за указанный период, включая достигнутый прогресс, выявленные проблемы и возможные риски, которые могут повлиять на дальнейшую деятельность.
            //            - Примечание: Настоящий отчёт охватывает указанный временной период и предоставляет обзор выполнения работ. В рамках данного периода анализируются все проекты и задачи, выполняемые компанией, с целью выявления ключевых достижений, а также проблемных моментов, требующих дополнительного внимания со стороны руководства.";
            //        }

            //        if (string.IsNullOrEmpty(reportTypeSection))
            //        {
            //            reportTypeSection = "Тип отчёта не определён. Проверьте данные.";
            //        }

            //        // Добавление раздела отчёта с сохранением форматирования
            //        string[] sections = reportTypeSection.Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            //        foreach (string section in sections)
            //        {
            //            if (section.StartsWith("Отчёт по") || section.StartsWith("Отчёт за"))
            //            {
            //                // Это заголовок раздела
            //                string[] lines = section.Split(new[] { "\r\n" }, StringSplitOptions.None);
            //                Paragraph headerPara = wordDoc.Content.Paragraphs.Add();
            //                headerPara.Range.Text = lines[0];
            //                headerPara.set_Style(headingStyle);
            //                headerPara.Range.InsertParagraphAfter();

            //                // Добавляем остальной текст раздела
            //                for (int i = 1; i < lines.Length; i++)
            //                {
            //                    if (lines[i].StartsWith("- "))
            //                    {
            //                        AddListItem(wordDoc, lines[i]);
            //                    }
            //                    else
            //                    {
            //                        Paragraph para = wordDoc.Content.Paragraphs.Add();
            //                        para.Range.Text = lines[i];
            //                        para.set_Style(normalStyle);
            //                        para.Range.InsertParagraphAfter();
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                Paragraph para = wordDoc.Content.Paragraphs.Add();
            //                para.Range.Text = section;
            //                para.set_Style(normalStyle);
            //                para.Range.InsertParagraphAfter();
            //            }
            //        }

            //        // Итоговые показатели
            //        Paragraph summaryPara = wordDoc.Content.Paragraphs.Add();
            //        summaryPara.Range.Text = "Итоговые показатели деятельности за отчётный период:";
            //        summaryPara.set_Style(subheadingStyle);
            //        summaryPara.Range.InsertParagraphAfter();

            //        AddListItem(wordDoc, $"- Количество завершённых задач: {SelectedReport.CountEndTask ?? 0}. Указанное количество отражает общее число задач, которые были полностью выполнены в рамках проектов компании за отчётный период.");
            //        AddListItem(wordDoc, $"- Количество завершённых проектов: {SelectedReport.CountEndProject ?? 0}. Данный показатель демонстрирует общее количество проектов, успешно завершённых и сданных в эксплуатацию.");
            //        AddListItem(wordDoc, $"- Количество незавершённых задач и проектов: {SelectedReport.CountNotEndProject ?? 0}. Этот показатель позволяет оценить объём текущих работ, которые требуют дальнейшего выполнения и контроля.");
            //        AddListItem(wordDoc, $"- Номер строки в общем реестре отчётов: {SelectedReport.RowNumber}. Данный номер используется для учёта отчёта в общей системе документации компании.");
            //        wordDoc.Content.Paragraphs.Add().Range.InsertParagraphAfter();

            //        // Заключение
            //        Paragraph conclusionPara = wordDoc.Content.Paragraphs.Add();
            //        conclusionPara.Range.Text = "Заключение: Настоящий отчёт отражает текущее состояние выполнения строительных проектов и задач, выполняемых компанией TileHaus. Все данные, представленные в отчёте, основаны на информации, собранной в процессе выполнения работ, и могут быть использованы для дальнейшего планирования, анализа эффективности и принятия управленческих решений. При необходимости более детального анализа рекомендуется обратиться к дополнительным документам и отчётам, хранящимся в архиве компании.";
            //        conclusionPara.set_Style(normalStyle);
            //        conclusionPara.Range.InsertParagraphAfter();

            //        // Подпись
            //        Paragraph signaturePara = wordDoc.Content.Paragraphs.Add();
            //        signaturePara.Range.Text = "Подпись ответственного лица:";
            //        signaturePara.set_Style(subheadingStyle);
            //        signaturePara.Range.InsertParagraphAfter();

            //        Paragraph namePara = wordDoc.Content.Paragraphs.Add();
            //        namePara.Range.Text = SelectedReport.EmployeeName ?? "Не указано";
            //        namePara.set_Style(normalStyle);
            //        namePara.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            //        namePara.Range.InsertParagraphAfter();

            //        Paragraph dateSignPara = wordDoc.Content.Paragraphs.Add();
            //        dateSignPara.Range.Text = $"Дата: {SelectedReport.CreationDate:dd.MM.yyyy HH:mm}";
            //        dateSignPara.set_Style(normalStyle);
            //        dateSignPara.Alignment = WdParagraphAlignment.wdAlignParagraphRight;

            //        // Добавление нумерации страниц по ГОСТ (внизу по центру, начиная со 2-й страницы)
            //        wordDoc.Sections[1].Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(
            //            WdPageNumberAlignment.wdAlignPageNumberCenter,
            //            WdPageNumberStyle.wdPageNumberStyleArabic
            //        );

            //        // Сохранение документа
            //        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //        string fileName = $"Report_{SelectedReport.ReportID}_{DateTime.Now:dd.MM.yyyy}.docx";
            //        wordDoc.SaveAs2(System.IO.Path.Combine(desktopPath, fileName));
            //        MessageBox.Show($"Отчёт сохранён на рабочем столе как {fileName}");

            //        await _loggingService.LogAction(myUser.EmployeeID, "Создание отчёта", "Операции с данными", "Успех");
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Ошибка при создании отчёта: {ex.Message}");
            //         await _loggingService.LogAction(myUser.EmployeeID, "Создание отчёта", "Операции с данными", "Ошибка");
            //    }
            //    finally
            //    {
            //        wordDoc.Close();
            //        wordApp.Quit();
            //    }
            //});

        }

        private void AddListItem(Document doc, string text)
        {
            WordDoc.Paragraph para = doc.Content.Paragraphs.Add();
            para.Range.Text = text;
            para.set_Style(doc.Styles[WdBuiltinStyle.wdStyleNormal]);
            para.Range.InsertParagraphAfter();
        }


        private RelayCommand CreateCommandWithViewModel<TControl, TViewModel>(Func<TViewModel> viewModelFactory)
        where TControl : UserControl, new()
        {
            return new RelayCommand(_ =>
            {
                var control = new TControl();
                control.DataContext = viewModelFactory();
                CurrentControl = control;
            });
        }

        private RelayCommand CreateCommand(Func<UserControl> controlFactory)
        {
            return new RelayCommand(_ =>
            {
                
                CurrentControl = controlFactory();
            });
        }
        private RelayCommand CreateCommand2(Func<UserControl> controlFactory)
        {
            return new RelayCommand(_ =>
            {
                SecondControl = controlFactory();
            });
        }

        private async System.Threading.Tasks.Task LoadReport()
        {
            try
            {
                _isLoading = true;
                CommandManager.InvalidateRequerySuggested();

                var report = await System.Threading.Tasks.Task.Run(() =>
                    reportService.GetAllReports());

                allReport = new ObservableCollection<tbReport>(report);
                ApplyFilters();
                await _loggingService.LogAction(myUser.EmployeeID, "Загрузка проектов", "Операции с данными", "Успех");
            }
            catch
            {
                _isLoading = false;
                CommandManager.InvalidateRequerySuggested();
                await _loggingService.LogAction(myUser.EmployeeID, "Загрузка проектов", "Операции с данными", "Ошибка");
            }
        }
        private void FilterByStatus(object role)
        {
            currentFilterStatus = role?.ToString() ?? "Все";
            SelectedStatus = currentFilterStatus;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (allReport == null) return;

            IEnumerable<tbReport> filtered = allReport;

            // Фильтрация по роли
            if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Все")
            {
                filtered = filtered.Where(e => e.Status == selectedStatus);
            }

            // Фильтрация по тексту
            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchTextLower = SearchText.ToLower();
                filtered = filtered.Where(e =>
                    e.Title.ToLower().Contains(searchTextLower) ||
                    e.Status.ToLower().Contains(searchTextLower) ||
                    e.EmployeeName.ToLower().Contains(searchTextLower) ||
                    e.ProjectName.ToLower().Contains(searchTextLower) ||
                    e.TaskName.ToLower().Contains(searchTextLower));
            }

            CurrentReport = new ObservableCollection<tbReport>(filtered);
            UpdateRecordCount();
        }

        private void UpdateRecordCount()
        {
            int count = CurrentReport.Count;
            string ending = count % 100 is >= 11 and <= 14 ? "Отчётов" :
                (count % 10) switch
                {
                    1 => "Отчёт",
                    2 or 3 or 4 => "Отчёта",
                    _ => "Отчётов"
                };

            CountText = $"{count} {ending}";
            OnPropertyChanged(nameof(CountText));
        }

        private async void RefreshData()
        {
            try
            {
                _isLoading = true;
                CommandManager.InvalidateRequerySuggested();

                var projects = await System.Threading.Tasks.Task.Run(() => reportService.GetAllReports());
                allReport = new ObservableCollection<tbReport>(projects);
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


        private async void RemoveReport()
        {
            try
            {
                if (MessageBox.Show($"Удалить отчёт {SelectedReport.Title}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    reportService.DeleteReport(SelectedReport.ReportID);
                    RefreshData();

                    await _loggingService.LogAction(myUser.EmployeeID, "Удаление отчёта", "Операции с данными", "Успех");
                }
            }
            catch
            {
                await _loggingService.LogAction(myUser.EmployeeID, "Удаление отчёта", "Операции с данными", "Ошибка");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
