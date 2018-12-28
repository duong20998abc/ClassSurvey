namespace ClassSurvey.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNullableToStudentClass : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StudentClasses", "ClassId", "dbo.Classes");
            DropForeignKey("dbo.StudentClasses", "StudentId", "dbo.Students");
            DropForeignKey("dbo.StudentClasses", "TeacherId", "dbo.Teachers");
            DropIndex("dbo.StudentClasses", new[] { "StudentId" });
            DropIndex("dbo.StudentClasses", new[] { "ClassId" });
            DropIndex("dbo.StudentClasses", new[] { "TeacherId" });
            AlterColumn("dbo.StudentClasses", "StudentId", c => c.Int());
            AlterColumn("dbo.StudentClasses", "ClassId", c => c.Int());
            AlterColumn("dbo.StudentClasses", "TeacherId", c => c.Int());
            CreateIndex("dbo.StudentClasses", "StudentId");
            CreateIndex("dbo.StudentClasses", "ClassId");
            CreateIndex("dbo.StudentClasses", "TeacherId");
            AddForeignKey("dbo.StudentClasses", "ClassId", "dbo.Classes", "Id");
            AddForeignKey("dbo.StudentClasses", "StudentId", "dbo.Students", "Id");
            AddForeignKey("dbo.StudentClasses", "TeacherId", "dbo.Teachers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentClasses", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.StudentClasses", "StudentId", "dbo.Students");
            DropForeignKey("dbo.StudentClasses", "ClassId", "dbo.Classes");
            DropIndex("dbo.StudentClasses", new[] { "TeacherId" });
            DropIndex("dbo.StudentClasses", new[] { "ClassId" });
            DropIndex("dbo.StudentClasses", new[] { "StudentId" });
            AlterColumn("dbo.StudentClasses", "TeacherId", c => c.Int(nullable: false));
            AlterColumn("dbo.StudentClasses", "ClassId", c => c.Int(nullable: false));
            AlterColumn("dbo.StudentClasses", "StudentId", c => c.Int(nullable: false));
            CreateIndex("dbo.StudentClasses", "TeacherId");
            CreateIndex("dbo.StudentClasses", "ClassId");
            CreateIndex("dbo.StudentClasses", "StudentId");
            AddForeignKey("dbo.StudentClasses", "TeacherId", "dbo.Teachers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.StudentClasses", "StudentId", "dbo.Students", "Id", cascadeDelete: true);
            AddForeignKey("dbo.StudentClasses", "ClassId", "dbo.Classes", "Id", cascadeDelete: true);
        }
    }
}
