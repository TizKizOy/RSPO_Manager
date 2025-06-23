using MyKpiyapProject.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Compression;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace MyKpiyapProject.NewModels
{
    public class tbProject : INotifyPropertyChanged
    {
        private int projectID;
        private DateTime creationDate;
        private DateTime closingDate;
        private int statusID;
        private string description;
        private string title;
        private int creatorID;
        private int docxDataID;

        private string tasksCount;
        private int rowNumber;

        [NotMapped]
        public string DocumentType
        {
            get
            {
                if (DocxData.DocxData == null || DocxData.DocxData.Length == 0)
                    return "Нет документа";

                string extension = GetFileExtension(DocxData.DocxData);
                return extension switch
                {
                    ".doc" => "Word (DOC)",
                    ".docx" => "Word (DOCX)",
                    ".xls" => "Excel (XLS)",
                    ".xlsx" => "Excel (XLSX)",
                    _ => "Неподдерживаемый формат"
                };
            }
        }


        [NotMapped]
        public string TasksCount
        {
            get
            {
                try
                {
                    int total = Tasks.Count;
                    int completed = Tasks.Count(t => t?.Status.StatusName == "Закрыт" || t?.Status.StatusName == "Отменён");
                    return $"{completed}/{total}";
                }
                catch
                {
                    return "0/0";
                }
            }
            set
            {
                tasksCount = value;
                OnPropertyChanged();
            }
        }

        [NotMapped]
        public int RowNumber
        {
            get { return rowNumber; }
            set { rowNumber = value; OnPropertyChanged(); }
        }
        [NotMapped]
        public string CreatorName
        {
            get
            {
                return Creator.FullName;
            }
            set
            {
                Creator.FullName = value;
                OnPropertyChanged();
            }
        }

        [Key]
        public int ProjectID
        {
            get { return projectID; }
            set { projectID = value; OnPropertyChanged(); }
        }

        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; OnPropertyChanged(); }
        }

        public DateTime ClosingDate
        {
            get { return closingDate; }
            set { closingDate = value; OnPropertyChanged(); }
        }

        //public string Status
        //{
        //    get { return status; }
        //    set { status = value; OnPropertyChanged(); }
        //}

        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        //public byte[] DocxData
        //{
        //    get { return docxData; }
        //    set { docxData = value; OnPropertyChanged(); }
        //}

        [ForeignKey("Creator")]
        public int CreatorID
        {
            get { return creatorID; }
            set { creatorID = value; OnPropertyChanged(); }
        }

        [ForeignKey("Status")]
        public int StatusID { get; set; }

        [ForeignKey("DocxData")]
        public int? DocumentID { get; set; }

        public virtual tbEmployee Creator { get; set; }
        public virtual tbProjectStatus Status { get; set; }
        public virtual tbProjectDocument DocxData { get; set; }
        public virtual ICollection<tbTask> Tasks { get; set; }
        public virtual ICollection<tbReport> Reports { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string GetFileExtension(byte[] fileData)
        {
            if (fileData == null || fileData.Length < 8)
                return null;

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

            return null;
        }
    }
}
