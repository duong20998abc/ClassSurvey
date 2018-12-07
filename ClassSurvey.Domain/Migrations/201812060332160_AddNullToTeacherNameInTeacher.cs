namespace ClassSurvey.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNullToTeacherNameInTeacher : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Teachers", "TeacherName", c => c.String(maxLength: 256));
            AlterColumn("dbo.Teachers", "Email", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Teachers", "Email", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Teachers", "TeacherName", c => c.String(nullable: false, maxLength: 256));
        }
    }
}
