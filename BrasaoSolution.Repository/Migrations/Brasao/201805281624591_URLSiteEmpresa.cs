namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class URLSiteEmpresa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMPRESA", "URL_SITE", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EMPRESA", "URL_SITE");
        }
    }
}
