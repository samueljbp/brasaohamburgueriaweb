namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ControleCasaFechadaEmpresa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMPRESA", "CASA_ABERTA", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EMPRESA", "CASA_ABERTA");
        }
    }
}
