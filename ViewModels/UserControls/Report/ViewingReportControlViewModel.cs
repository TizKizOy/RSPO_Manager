using WpfDoc = System.Windows.Documents; // Псевдоним для WPF Documents
using WordDoc = Microsoft.Office.Interop.Word; // Псевдоним для Word Interop
using Excel = Microsoft.Office.Interop.Excel;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using MyKpiyapProject.Views.UserControls.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MyKpiyapProject.ViewModels.UserControls.Report
{
    public class ProjectInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        
    }

    public class ViewingReportControlViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private System.Action _refreshCallback;
        private ReportService _reportService;
        private tbReport _selectedReport;

        private RichTextBox _richTextBox;
        private FlowDocument _flowDocument;
        private List<ProjectInfo> projectData;

        private ProjectService projectService = new ProjectService();
        private TaskService taskService = new TaskService();

        public string ReportName => SelectedReport?.Title ?? "Без названия";

        private UserControl _currentControl;
        private UserControl _secondControl;

        public ICommand LoadDataCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand SaveDataRepCommand { get; }


        public tbReport SelectedReport
        {
            get => _selectedReport;
            set
            {
                _selectedReport = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ReportName));
                LoadData(null); // Загружаем предпросмотр при изменении отчета
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
        public UserControl CurrentControl
        {
            get { return _currentControl; }
            set
            {
                _currentControl = value;
                OnPropertyChanged();
            }
        }


        public ViewingReportControlViewModel()
        {
        }

        public ViewingReportControlViewModel(tbReport Report)
        {
            SelectedReport = Report;
            LoadDataCommand = new RelayCommand(LoadData);
            PrintCommand = new RelayCommand(PrintData);
            SaveDataRepCommand = new RelayCommand(SaveDataRep);

            LoadDataCommand.Execute(null);
        }

        public void SetRichTextBox(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
        }


        public void LoadData(object parameter)
        {
            if (_richTextBox == null)
            {
                return;
            }

            if (SelectedReport == null)
            {
                MessageBox.Show("Необходимо выбрать запись для создания отчёта");
                return;
            }

            try
            {
                if (SelectedReport.CountEndTask > 0 || SelectedReport.CountEndProject > 0)
                {
                    LoadDataWithTable(parameter); // Используем метод с таблицей для отчета по промежутку времени
                }
                else
                {
                    LoadStandardData(parameter); // Используем стандартный метод загрузки данных
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке документа: {ex.Message}");
            }
        }




        public void LoadDataWithTable(object parameter)
        {
            if (_richTextBox == null || SelectedReport == null)
            {
                MessageBox.Show("Необходимо выбрать запись для создания отчёта");
                return;
            }

            try
            {
                var allProject = projectService.GetAllProjects()
                    .Where(p => p.Status.StatusName == "Закрыт" || p.Status.StatusName == "Отменён");
                var allTask = taskService.GetAllTasks().
                    Where(p => p.Status.StatusName == "Закрыт" || p.Status.StatusName == "Отменён");


                projectData = allProject.Select(p => new ProjectInfo
                {
                    ID = p.ProjectID,
                    Name = p.Title,
                    Status = p.Status.StatusName,
                    Date = p.ClosingDate.ToShortDateString(),

                }).ToList();

                projectData = allTask.Select(p => new ProjectInfo
                {
                    ID = p.ProjectID,
                    Name = p.Title,
                    Status = p.Status.StatusName,
                    Date = p.DeadLineDate.ToShortDateString(),

                }).ToList();

                // Создаем FlowDocument
                _flowDocument = new FlowDocument
                {
                    FontFamily = new System.Windows.Media.FontFamily("Times New Roman"),
                    FontSize = 14,
                    TextAlignment = TextAlignment.Justify,
                    LineHeight = 21
                };

                // Добавляем название отчета из SelectedReport
                Paragraph titleParagraph = new Paragraph(new Run(SelectedReport.Title ?? "Отчет по проектам"))
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                _flowDocument.Blocks.Add(titleParagraph);

                // Добавляем описание отчета из SelectedReport
                Paragraph descriptionParagraph = new Paragraph(new Run(SelectedReport.Description + ". Этот отчет содержит информацию о текущем состоянии всех проектов, включая их идентификаторы, названия и статус выполнения. Данные актуальны на момент формирования отчета."))
                {
                    FontStyle = FontStyles.Italic,
                    TextAlignment = TextAlignment.Justify,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                _flowDocument.Blocks.Add(descriptionParagraph);

                // Создаем таблицу
                Table table = new Table();
                table.CellSpacing = 0;
                table.BorderThickness = new Thickness(1);
                table.BorderBrush = Brushes.Black;

                // Добавляем столбцы
                for (int i = 0; i < 3; i++)
                {
                    table.Columns.Add(new TableColumn());
                }

                // Создаем заголовок таблицы
                TableRowGroup headerRowGroup = new TableRowGroup();
                TableRow headerRow = new TableRow();

                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("ID"))) { TextAlignment = TextAlignment.Center, FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Название"))) { TextAlignment = TextAlignment.Center, FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Статус"))) { TextAlignment = TextAlignment.Center, FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Дата завершения"))) { TextAlignment = TextAlignment.Center, FontWeight = FontWeights.Bold });

                headerRowGroup.Rows.Add(headerRow);
                table.RowGroups.Add(headerRowGroup);

                // Создаем строки таблицы с данными
                TableRowGroup dataRowGroup = new TableRowGroup();

                foreach (var project in projectData)
                {
                    TableRow dataRow = new TableRow();

                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(project.ID.ToString()))) { TextAlignment = TextAlignment.Center });
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(project.Name))) { TextAlignment = TextAlignment.Center });
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(project.Status))) { TextAlignment = TextAlignment.Center });
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(project.Date))) { TextAlignment = TextAlignment.Center });

                    dataRowGroup.Rows.Add(dataRow);
                }

                table.RowGroups.Add(dataRowGroup);

                // Добавляем таблицу в FlowDocument
                _flowDocument.Blocks.Add(table);


                // Подпись
                WpfDoc.Paragraph signaturePara = new WpfDoc.Paragraph(new WpfDoc.Run("Подпись ответственного лица:"))
                {
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                _flowDocument.Blocks.Add(signaturePara);

                WpfDoc.Paragraph namePara = new WpfDoc.Paragraph(new WpfDoc.Run(SelectedReport.EmployeeName ?? "Не указано"))
                {
                    TextAlignment = TextAlignment.Right
                };
                _flowDocument.Blocks.Add(namePara);

                WpfDoc.Paragraph dateSignPara = new WpfDoc.Paragraph(new WpfDoc.Run($"Дата: {SelectedReport.CreationDate:dd.MM.yyyy HH:mm:ss}"))
                {
                    TextAlignment = TextAlignment.Right
                };
                _flowDocument.Blocks.Add(dateSignPara);


                // Устанавливаем FlowDocument в RichTextBox
                _richTextBox.Document = _flowDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке документа: {ex.Message}");
            }
        }




        public void LoadStandardData(object parameter)
        {
            if (_richTextBox == null)
            {
                return;
            }

            if (SelectedReport == null)
            {
                MessageBox.Show("Необходимо выбрать запись для создания отчёта");
                return;
            }

            try
            {
                // Создаем FlowDocument
                _flowDocument = new WpfDoc.FlowDocument
                {
                    FontFamily = new System.Windows.Media.FontFamily("Times New Roman"),
                    FontSize = 14,
                    TextAlignment = TextAlignment.Justify,
                    LineHeight = 21 // 1.5 строки при 14 pt
                };

                // Заголовок компании
                WpfDoc.Paragraph companyPara = new WpfDoc.Paragraph(new WpfDoc.Run("ООО \"Строительная компания TileHaus\""))
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                _flowDocument.Blocks.Add(companyPara);

                // Дата составления
                WpfDoc.Paragraph datePara = new WpfDoc.Paragraph(new WpfDoc.Run($"Дата составления отчёта: {DateTime.Now:dd.MM.yyyy HH:mm:ss}"))
                {
                    TextAlignment = TextAlignment.Justify,
                    Margin = new Thickness(35.4, 0, 0, 0) // Абзацный отступ 1.25 см
                };
                _flowDocument.Blocks.Add(datePara);

                // Заголовок отчёта
                WpfDoc.Paragraph titlePara = new WpfDoc.Paragraph(new WpfDoc.Run("Отчёт по управлению проектами и задачами"))
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                _flowDocument.Blocks.Add(titlePara);

                // Введение
                WpfDoc.Paragraph introPara = new WpfDoc.Paragraph(new WpfDoc.Run("Настоящий отчёт подготовлен в рамках текущей деятельности строительной компании TileHaus, специализирующейся на выполнении широкого спектра строительных и монтажных работ. Документ предназначен для предоставления полной и достоверной информации о ходе выполнения проектов и задач, а также для анализа эффективности использования ресурсов и трудовых затрат. Отчёт составлен на основе данных, собранных в процессе выполнения работ, и отражает текущее состояние дел на момент составления документа."))
                {
                    Margin = new Thickness(35.4, 0, 0, 0)
                };
                _flowDocument.Blocks.Add(introPara);

                // Сведения о сотруднике
                WpfDoc.Paragraph empPara = new WpfDoc.Paragraph(new WpfDoc.Run("Сведения о сотруднике, ответственном за выполнение работ:"))
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                _flowDocument.Blocks.Add(empPara);

                // Список сведений о сотруднике
                WpfDoc.List empList = new WpfDoc.List();
                empList.ListItems.Add(new WpfDoc.ListItem(new WpfDoc.Paragraph(new WpfDoc.Run($"- Идентификатор сотрудника: {SelectedReport.EmployeeID ?? 0}. Данный идентификатор используется для учёта сотрудников в системе управления персоналом компании."))));
                empList.ListItems.Add(new WpfDoc.ListItem(new WpfDoc.Paragraph(new WpfDoc.Run($"- Полное имя сотрудника: {SelectedReport.EmployeeName ?? "Не указано"}. Указанный сотрудник является ответственным лицом, выполняющим контроль и непосредственное участие в реализации задач и проектов."))));
                empList.ListItems.Add(new WpfDoc.ListItem(new WpfDoc.Paragraph(new WpfDoc.Run($"- Затраты, связанные с выполнением работ: {SelectedReport.WorkExpenses ?? "0"} руб. Данные затраты включают в себя расходы на материалы, оборудование, транспорт и прочие операционные издержки, возникшие в процессе выполнения работ."))));
                _flowDocument.Blocks.Add(empList);

                // Раздел отчёта
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

                // Добавляем раздел отчёта
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
                        _flowDocument.Blocks.Add(headerPara);

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
                                _flowDocument.Blocks.Add(para);
                            }
                        }
                        if (sectionList.ListItems.Count > 0)
                        {
                            _flowDocument.Blocks.Add(sectionList);
                        }
                    }
                    else
                    {
                        WpfDoc.Paragraph para = new WpfDoc.Paragraph(new WpfDoc.Run(section))
                        {
                            Margin = new Thickness(35.4, 0, 0, 0)
                        };
                        _flowDocument.Blocks.Add(para);
                    }
                }

                // Заключение
                WpfDoc.Paragraph conclusionPara = new WpfDoc.Paragraph(new WpfDoc.Run("Заключение: Настоящий отчёт отражает текущее состояние выполнения строительных проектов и задач, выполняемых компанией TileHaus. Все данные, представленные в отчёте, основаны на информации, собранной в процессе выполнения работ, и могут быть использованы для дальнейшего планирования, анализа эффективности и принятия управленческих решений. При необходимости более детального анализа рекомендуется обратиться к дополнительным документам и отчётам, хранящимся в архиве компании."))
                {
                    Margin = new Thickness(35.4, 0, 0, 0)
                };
                _flowDocument.Blocks.Add(conclusionPara);

                // Подпись
                WpfDoc.Paragraph signaturePara = new WpfDoc.Paragraph(new WpfDoc.Run("Подпись ответственного лица:"))
                {
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                _flowDocument.Blocks.Add(signaturePara);

                WpfDoc.Paragraph namePara = new WpfDoc.Paragraph(new WpfDoc.Run(SelectedReport.EmployeeName ?? "Не указано"))
                {
                    TextAlignment = TextAlignment.Right
                };
                _flowDocument.Blocks.Add(namePara);

                WpfDoc.Paragraph dateSignPara = new WpfDoc.Paragraph(new WpfDoc.Run($"Дата: {SelectedReport.CreationDate:dd.MM.yyyy HH:mm:ss}"))
                {
                    TextAlignment = TextAlignment.Right
                };
                _flowDocument.Blocks.Add(dateSignPara);

                // Устанавливаем FlowDocument в RichTextBox
                _richTextBox.Document = _flowDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке документа: {ex.Message}");
            }
        }

        private void PrintData(object parameter)
        {
            if (_flowDocument == null)
            {
                MessageBox.Show("Сначала загрузите отчет.");
                return;
            }

            try
            {
                // Преобразуем FlowDocument в FixedDocument
                FixedDocument fixedDocument = new FixedDocument();
                PageContent pageContent = new PageContent();
                FixedPage fixedPage = new FixedPage();

                // Копируем содержимое FlowDocument
                TextRange textRange = new TextRange(_flowDocument.ContentStart, _flowDocument.ContentEnd);
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

                // Печать
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(fixedDocument.DocumentPaginator, $"Печать отчета: {SelectedReport.Title}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}");
            }
        }

        private void SaveDataRep(object parameter)
        {
            if (SelectedReport == null || projectData == null)
            {
                MessageBox.Show("Ошибка: Отчет не загружен или не выбран.");
                return;
            }

            if (SelectedReport.CountEndTask > 0 || SelectedReport.CountEndProject > 0)
            {
                Excel.Application excelApp = null;
                Excel.Workbook workbook = null;

                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string fileName = $"Report_{SelectedReport.ReportID}_{DateTime.Now:dd.MM.yyyy}.xlsx";
                    string filePath = System.IO.Path.Combine(desktopPath, fileName);

                    excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    workbook = excelApp.Workbooks.Add(Type.Missing);
                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

                    // Заголовок отчета
                    worksheet.Cells[1, 1] = "Промежуток: 03.05.25 - 02.06.25";
                    worksheet.get_Range("A1", "D1").Merge();
                    worksheet.get_Range("A1", "D1").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet.get_Range("A1", "D1").Font.Bold = true;

                    // Описание отчета
                    worksheet.Cells[2, 1] = "Отчёт заа промежуток времени, а именно май месяц, итоги будут представлены ниже. Этот отчёт содержит информацию о текущем состоянии всех проектов, включая их идентификаторы, названия и статус выполнения. Данные актуальны на момент формирования отчета.";
                    worksheet.get_Range("A2", "D2").Merge();
                    worksheet.get_Range("A2", "D2").HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    worksheet.get_Range("A2", "D2").WrapText = true;

                    // Заголовки таблицы
                    worksheet.Cells[4, 1] = "ID";
                    worksheet.Cells[4, 2] = "Название";
                    worksheet.Cells[4, 3] = "Статус";
                    worksheet.Cells[4, 4] = "Дата завершения";

                    // Заполняем данными
                    for (int i = 0; i < projectData.Count; i++)
                    {
                        worksheet.Cells[i + 5, 1] = projectData[i].ID;
                        worksheet.Cells[i + 5, 2] = projectData[i].Name;
                        worksheet.Cells[i + 5, 3] = projectData[i].Status;
                        worksheet.Cells[i + 5, 4] = projectData[i].Date;
                    }

                    // Подпись
                    worksheet.Cells[7 + projectData.Count, 1] = "Подпись ответственного лица:";
                    worksheet.get_Range($"A{7 + projectData.Count}", $"D{7 + projectData.Count}").Merge();
                    worksheet.get_Range($"A{7 + projectData.Count}", $"D{7 + projectData.Count}").HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    worksheet.get_Range($"A{7 + projectData.Count}", $"D{7 + projectData.Count}").Font.Bold = true;

                    worksheet.Cells[8 + projectData.Count, 1] = SelectedReport.EmployeeName ?? "Не указано";
                    worksheet.get_Range($"A{8 + projectData.Count}", $"D{8 + projectData.Count}").Merge();
                    worksheet.get_Range($"A{8 + projectData.Count}", $"D{8 + projectData.Count}").HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    worksheet.Cells[9 + projectData.Count, 1] = $"Дата: {SelectedReport.CreationDate:dd.MM.yyyy HH:mm:ss}";
                    worksheet.get_Range($"A{9 + projectData.Count}", $"D{9 + projectData.Count}").Merge();
                    worksheet.get_Range($"A{9 + projectData.Count}", $"D{9 + projectData.Count}").HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    // Сохраняем файл
                    workbook.SaveAs(filePath, Excel.XlFileFormat.xlOpenXMLWorkbook);
                    workbook.Close();
                    excelApp.Quit();

                    MessageBox.Show($"Отчёт сохранён на рабочем столе как {fileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении в Excel: {ex.Message}");
                }
                finally
                {
                    if (workbook != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    }
                    if (excelApp != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                }
            }
            else
            {
                // Сохраняем в Word для всех остальных типов отчетов
                WordDoc.Application wordApp = null;
                WordDoc.Document wordDoc = null;

                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string fileName = $"Report_{SelectedReport.ReportID}_{DateTime.Now:dd.MM.yyyy}.docx";
                    string filePath = System.IO.Path.Combine(desktopPath, fileName);

                    wordApp = new WordDoc.Application();
                    wordApp.Visible = false;
                    wordDoc = wordApp.Documents.Add();

                    // Настройка страницы по ГОСТ
                    wordDoc.PageSetup.LeftMargin = 28.35f;
                    wordDoc.PageSetup.RightMargin = 28.35f;
                    wordDoc.PageSetup.TopMargin = 56.7f;
                    wordDoc.PageSetup.BottomMargin = 56.7f;
                    wordDoc.PageSetup.Orientation = WordDoc.WdOrientation.wdOrientPortrait;

                    // Применение стилей
                    WordDoc.Style normalStyle = wordDoc.Styles[WordDoc.WdBuiltinStyle.wdStyleNormal];
                    normalStyle.Font.Name = "Times New Roman";
                    normalStyle.Font.Size = 14;
                    normalStyle.ParagraphFormat.Alignment = WordDoc.WdParagraphAlignment.wdAlignParagraphJustify;
                    normalStyle.ParagraphFormat.LineSpacingRule = WordDoc.WdLineSpacing.wdLineSpace1pt5;
                    normalStyle.ParagraphFormat.FirstLineIndent = 35.4f;
                    normalStyle.ParagraphFormat.SpaceAfter = 0;

                    // Извлекаем текст из RichTextBox и добавляем в Word
                    WpfDoc.TextRange textRange = new WpfDoc.TextRange(_flowDocument.ContentStart, _flowDocument.ContentEnd);
                    string[] paragraphs = textRange.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);

                    foreach (string paraText in paragraphs)
                    {
                        if (!string.IsNullOrWhiteSpace(paraText))
                        {
                            wordDoc.Content.Paragraphs.Add();
                            WordDoc.Paragraph para = wordDoc.Content.Paragraphs.Last;
                            para.Range.Text = paraText;
                            para.Range.set_Style(normalStyle);
                        }
                    }

                    wordDoc.SaveAs2(filePath);
                    wordDoc.Close();
                    wordApp.Quit();

                    MessageBox.Show($"Отчёт сохранён на рабочем столе как {fileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении в Word: {ex.Message}");
                }
                finally
                {
                    if (wordDoc != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDoc);
                    }
                    if (wordApp != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                    }
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
