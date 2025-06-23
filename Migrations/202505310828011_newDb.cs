namespace MyKpiyapProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbAdminLogs",
                c => new
                    {
                        LogID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        Action = c.String(),
                        EventType = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.LogID)
                .ForeignKey("dbo.tbEmployees", t => t.EmployeeID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.tbEmployees",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Login = c.String(),
                        Password = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Photo = c.Binary(),
                        Experience = c.String(),
                        GenderID = c.Int(nullable: false),
                        PositionAndRoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.tbGenders", t => t.GenderID)
                .ForeignKey("dbo.tbPositionAndRoles", t => t.PositionAndRoleID, cascadeDelete: true)
                .Index(t => t.GenderID)
                .Index(t => t.PositionAndRoleID);
            
            CreateTable(
                "dbo.tbGenders",
                c => new
                    {
                        GenderID = c.Int(nullable: false, identity: true),
                        GenderName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GenderID);
            
            CreateTable(
                "dbo.tbPositionAndRoles",
                c => new
                    {
                        PositionAndRoleID = c.Int(nullable: false, identity: true),
                        PositionAndRoleName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PositionAndRoleID);
            
            CreateTable(
                "dbo.tbProjects",
                c => new
                    {
                        ProjectID = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        ClosingDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        Title = c.String(),
                        CreatorID = c.Int(nullable: false),
                        StatusID = c.Int(nullable: false),
                        DocumentID = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectID)
                .ForeignKey("dbo.tbProjectDocuments", t => t.DocumentID)
                .ForeignKey("dbo.tbProjectStatus", t => t.StatusID, cascadeDelete: true)
                .ForeignKey("dbo.tbEmployees", t => t.CreatorID)
                .Index(t => t.CreatorID)
                .Index(t => t.StatusID)
                .Index(t => t.DocumentID);
            
            CreateTable(
                "dbo.tbProjectDocuments",
                c => new
                    {
                        DocumentID = c.Int(nullable: false, identity: true),
                        DocxData = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.DocumentID);
            
            CreateTable(
                "dbo.tbReports",
                c => new
                    {
                        ReportID = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        EmployeeID = c.Int(),
                        CountEndTask = c.Int(),
                        CountNotEndProject = c.Int(),
                        CountEndProject = c.Int(),
                        WorkExpenses = c.String(),
                        Status = c.String(),
                        ProjectID = c.Int(nullable: false),
                        TaskID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReportID)
                .ForeignKey("dbo.tbEmployees", t => t.EmployeeID)
                .ForeignKey("dbo.tbProjects", t => t.ProjectID)
                .ForeignKey("dbo.tbTasks", t => t.TaskID)
                .Index(t => t.EmployeeID)
                .Index(t => t.ProjectID)
                .Index(t => t.TaskID);
            
            CreateTable(
                "dbo.tbTasks",
                c => new
                    {
                        TaskID = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        DeadLineDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        Title = c.String(),
                        ProjectID = c.Int(nullable: false),
                        ExecutorID = c.Int(nullable: false),
                        CreatorID = c.Int(nullable: false),
                        StatusID = c.Int(nullable: false),
                        PriorityID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskID)
                .ForeignKey("dbo.tbEmployees", t => t.CreatorID, cascadeDelete: true)
                .ForeignKey("dbo.tbPriorityTasks", t => t.PriorityID, cascadeDelete: true)
                .ForeignKey("dbo.tbStatusTasks", t => t.StatusID, cascadeDelete: true)
                .ForeignKey("dbo.tbProjects", t => t.ProjectID, cascadeDelete: true)
                .ForeignKey("dbo.tbEmployees", t => t.ExecutorID)
                .Index(t => t.ProjectID)
                .Index(t => t.ExecutorID)
                .Index(t => t.CreatorID)
                .Index(t => t.StatusID)
                .Index(t => t.PriorityID);
            
            CreateTable(
                "dbo.tbPriorityTasks",
                c => new
                    {
                        PriorityID = c.Int(nullable: false, identity: true),
                        PriorityName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PriorityID);
            
            CreateTable(
                "dbo.tbStatusTasks",
                c => new
                    {
                        StatusID = c.Int(nullable: false, identity: true),
                        StatusName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.StatusID);
            
            CreateTable(
                "dbo.tbProjectStatus",
                c => new
                    {
                        ProjectStatusID = c.Int(nullable: false, identity: true),
                        StatusName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectStatusID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbTasks", "ExecutorID", "dbo.tbEmployees");
            DropForeignKey("dbo.tbProjects", "CreatorID", "dbo.tbEmployees");
            DropForeignKey("dbo.tbTasks", "ProjectID", "dbo.tbProjects");
            DropForeignKey("dbo.tbProjects", "StatusID", "dbo.tbProjectStatus");
            DropForeignKey("dbo.tbReports", "TaskID", "dbo.tbTasks");
            DropForeignKey("dbo.tbTasks", "StatusID", "dbo.tbStatusTasks");
            DropForeignKey("dbo.tbTasks", "PriorityID", "dbo.tbPriorityTasks");
            DropForeignKey("dbo.tbTasks", "CreatorID", "dbo.tbEmployees");
            DropForeignKey("dbo.tbReports", "ProjectID", "dbo.tbProjects");
            DropForeignKey("dbo.tbReports", "EmployeeID", "dbo.tbEmployees");
            DropForeignKey("dbo.tbProjects", "DocumentID", "dbo.tbProjectDocuments");
            DropForeignKey("dbo.tbEmployees", "PositionAndRoleID", "dbo.tbPositionAndRoles");
            DropForeignKey("dbo.tbEmployees", "GenderID", "dbo.tbGenders");
            DropForeignKey("dbo.tbAdminLogs", "EmployeeID", "dbo.tbEmployees");
            DropIndex("dbo.tbTasks", new[] { "PriorityID" });
            DropIndex("dbo.tbTasks", new[] { "StatusID" });
            DropIndex("dbo.tbTasks", new[] { "CreatorID" });
            DropIndex("dbo.tbTasks", new[] { "ExecutorID" });
            DropIndex("dbo.tbTasks", new[] { "ProjectID" });
            DropIndex("dbo.tbReports", new[] { "TaskID" });
            DropIndex("dbo.tbReports", new[] { "ProjectID" });
            DropIndex("dbo.tbReports", new[] { "EmployeeID" });
            DropIndex("dbo.tbProjects", new[] { "DocumentID" });
            DropIndex("dbo.tbProjects", new[] { "StatusID" });
            DropIndex("dbo.tbProjects", new[] { "CreatorID" });
            DropIndex("dbo.tbEmployees", new[] { "PositionAndRoleID" });
            DropIndex("dbo.tbEmployees", new[] { "GenderID" });
            DropIndex("dbo.tbAdminLogs", new[] { "EmployeeID" });
            DropTable("dbo.tbProjectStatus");
            DropTable("dbo.tbStatusTasks");
            DropTable("dbo.tbPriorityTasks");
            DropTable("dbo.tbTasks");
            DropTable("dbo.tbReports");
            DropTable("dbo.tbProjectDocuments");
            DropTable("dbo.tbProjects");
            DropTable("dbo.tbPositionAndRoles");
            DropTable("dbo.tbGenders");
            DropTable("dbo.tbEmployees");
            DropTable("dbo.tbAdminLogs");
        }
    }
}
