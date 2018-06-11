namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TextoInstitucionalEmpresa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMPRESA", "TEXTO_INSTITUCIONAL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EMPRESA", "TEXTO_INSTITUCIONAL");
        }
    }
}
