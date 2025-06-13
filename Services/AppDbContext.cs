using Microsoft.Office.Interop.Excel;
using MyKpiyapProject.NewModels;
using System.Data.Entity;
using System.Net.NetworkInformation;

namespace MyKpiyapProject.Services
{
    public class AppDbContext : DbContext
    {
        private readonly static string ConnectionString = "Data Source=HOME-PC\\SQLEXPRESS;Initial Catalog=dbRspoManager;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;";

        public DbSet<tbEmployee> Employees { get; set; }
        public DbSet<tbReport> Reports { get; set; }
        public DbSet<tbProject> Projects { get; set; }
        public DbSet<tbTask> Tasks { get; set; }
        public DbSet<tbAdminLog> AdminLogs { get; set; }
        public DbSet<tbStatusTask> Statuses { get; set; }
        public DbSet<tbPriorityTask> Priorities { get; set; }
        public DbSet<tbProjectStatus> ProjectStatuses { get; set; }
        public DbSet<tbProjectDocument> Documents { get; set; }
        public DbSet<tbGender> Genders { get; set; }
        public DbSet<tbPositionAndRole> PositionAndRoles { get; set; }
        public DbSet<tbWorkExperience> workExperiences { get; set; }

        public AppDbContext() : base(ConnectionString)
        {
            Configuration.LazyLoadingEnabled = true;
            //Configuration.ProxyCreationEnabled = false;
         
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbGender>()
               .HasMany(g => g.Employees)
               .WithRequired(e => e.Gender)
               .HasForeignKey(e => e.GenderID)
               .WillCascadeOnDelete(false); // Отключение каскадного удаления

            // Настройка связей для tbProject
            modelBuilder.Entity<tbProject>()
                .HasMany(p => p.Tasks)
                .WithRequired(t => t.Project)
                .HasForeignKey(t => t.ProjectID)
                .WillCascadeOnDelete(true); // Удаление проекта => удаление задач

            // Настройка связей для tbEmployee (исполнителя задач)
            modelBuilder.Entity<tbEmployee>()
                .HasMany(e => e.Tasks)
                .WithRequired(t => t.Executor)
                .HasForeignKey(t => t.ExecutorID)
                .WillCascadeOnDelete(false); // Удаление сотрудника НЕ удаляет задачи

            modelBuilder.Entity<tbReport>()
                   .HasRequired(r => r.Task)
                   .WithMany(t => t.Reports) // Явно указываем обратную связь
                   .HasForeignKey(r => r.TaskID)
                   .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbReport>()
                .HasRequired(r => r.Project)
                .WithMany(p => p.Reports)
                .HasForeignKey(r => r.ProjectID)
                .WillCascadeOnDelete(false);

            // Настройка связей для создателя проекта
            modelBuilder.Entity<tbEmployee>()
                .HasMany(e => e.Projects)
                .WithRequired(p => p.Creator)
                .HasForeignKey(p => p.CreatorID)
                .WillCascadeOnDelete(false);

            // Настройка связей для логов администратора
            modelBuilder.Entity<tbEmployee>()
                .HasMany(e => e.AdminLogs)
                .WithRequired(a => a.Employee)
                .HasForeignKey(a => a.EmployeeID)
                .WillCascadeOnDelete(false);
        }
    }
}
