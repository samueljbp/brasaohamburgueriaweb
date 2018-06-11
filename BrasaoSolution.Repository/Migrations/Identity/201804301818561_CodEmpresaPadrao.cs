namespace BrasaoSolution.Web.Migrations.Identity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodEmpresaPadrao : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuarios", "CodEmpresaPreferencial", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Usuarios", "CodEmpresaPreferencial");
        }
    }
}
