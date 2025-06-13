using MyKpiyapProject.NewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.Services
{
    public class ProjectDocumentService
    {
        private readonly AppDbContext _context;

        public ProjectDocumentService()
        {
            _context = new AppDbContext();
        }

        public void AddOrUpdateDocument(int documentId, byte[] docxData)
        {
            var document = _context.Documents.Find(documentId);
            if (document == null)
            {
                document = new tbProjectDocument { DocxData = docxData };
                _context.Documents.Add(document);
            }
            else
            {
                document.DocxData = docxData;
            }
            _context.SaveChanges();
        }

        public byte[] GetDocumentData(int documentId)
        {
            var document = _context.Documents.Find(documentId);
            return document?.DocxData;
        }

        public int GetDocumentIdByData(byte[] docxData)
        {
            var document = _context.Documents.FirstOrDefault(d => d.DocxData == docxData);
            return document?.DocumentID ?? 0;
        }

        public void AssignDocumentToProject(int projectId, int documentId)
        {
            var project = _context.Projects.Find(projectId);
            if (project != null)
            {
                project.DocumentID = documentId;
                _context.SaveChanges();
            }
        }

    }
}
