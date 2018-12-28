namespace ClassSurvey.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStringLengthPassword : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Students", "Password", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.Teachers", "Password", c => c.String(nullable: false, maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Teachers", "Password", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Students", "Password", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
