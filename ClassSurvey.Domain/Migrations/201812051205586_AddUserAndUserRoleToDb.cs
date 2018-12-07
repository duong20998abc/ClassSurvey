namespace ClassSurvey.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserAndUserRoleToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        Username = c.String(nullable: false, maxLength: 256),
                        Password = c.String(nullable: false, maxLength: 256),
                        Logo = c.String(maxLength: 500),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Classes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassName = c.String(nullable: false, maxLength: 256),
                        ClassCode = c.String(nullable: false, maxLength: 256),
                        Semester = c.Int(nullable: false),
                        NumberOfDegrees = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudentClasses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: false),
                        ClassId = c.Int(nullable: false),
                        TeacherId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Classes", t => t.ClassId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.StudentId)
                .Index(t => t.ClassId)
                .Index(t => t.TeacherId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentCode = c.String(nullable: false, maxLength: 256),
                        StudentName = c.String(nullable: false, maxLength: 256),
                        Email = c.String(maxLength: 256),
                        ClassByGrade = c.String(maxLength: 100),
                        Username = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 50),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Surveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyQuestionId = c.Int(nullable: false),
                        StudentClassId = c.Int(nullable: false),
                        Result = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudentClasses", t => t.StudentClassId, cascadeDelete: true)
                .ForeignKey("dbo.SurveyQuestions", t => t.SurveyQuestionId, cascadeDelete: true)
                .Index(t => t.SurveyQuestionId)
                .Index(t => t.StudentClassId);
            
            CreateTable(
                "dbo.SurveyQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false, maxLength: 2048),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeacherName = c.String(nullable: false, maxLength: 256),
                        Email = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(maxLength: 50),
                        Office = c.String(maxLength: 100),
                        Username = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 100),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.String(),
                        Area = c.String(),
                        Controller = c.String(),
                        Action = c.String(),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 256),
                        Password = c.String(nullable: false),
                        Position = c.String(nullable: false),
                        StudentId = c.Int(),
                        TeacherId = c.Int(),
                        AdminId = c.Int(),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentClasses", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.Surveys", "SurveyQuestionId", "dbo.SurveyQuestions");
            DropForeignKey("dbo.Surveys", "StudentClassId", "dbo.StudentClasses");
            DropForeignKey("dbo.StudentClasses", "StudentId", "dbo.Students");
            DropForeignKey("dbo.StudentClasses", "ClassId", "dbo.Classes");
            DropIndex("dbo.Surveys", new[] { "StudentClassId" });
            DropIndex("dbo.Surveys", new[] { "SurveyQuestionId" });
            DropIndex("dbo.StudentClasses", new[] { "TeacherId" });
            DropIndex("dbo.StudentClasses", new[] { "ClassId" });
            DropIndex("dbo.StudentClasses", new[] { "StudentId" });
            DropTable("dbo.Users");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Teachers");
            DropTable("dbo.SurveyQuestions");
            DropTable("dbo.Surveys");
            DropTable("dbo.Students");
            DropTable("dbo.StudentClasses");
            DropTable("dbo.Classes");
            DropTable("dbo.Admins");
        }
    }
}
