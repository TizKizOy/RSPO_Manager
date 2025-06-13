namespace MyKpiyapProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addExpModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbWorkExperiences",
                c => new
                    {
                        ExperienceID = c.Int(nullable: false, identity: true),
                        ExperienceName = c.String(),
                    })
                .PrimaryKey(t => t.ExperienceID);
            
            AddColumn("dbo.tbEmployees", "ExperienceID", c => c.Int(nullable: false));
            CreateIndex("dbo.tbEmployees", "ExperienceID");
            AddForeignKey("dbo.tbEmployees", "ExperienceID", "dbo.tbWorkExperiences", "ExperienceID", cascadeDelete: true);
            DropColumn("dbo.tbEmployees", "Experience");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tbEmployees", "Experience", c => c.String());
            DropForeignKey("dbo.tbEmployees", "ExperienceID", "dbo.tbWorkExperiences");
            DropIndex("dbo.tbEmployees", new[] { "ExperienceID" });
            DropColumn("dbo.tbEmployees", "ExperienceID");
            DropTable("dbo.tbWorkExperiences");
        }
    }
}
