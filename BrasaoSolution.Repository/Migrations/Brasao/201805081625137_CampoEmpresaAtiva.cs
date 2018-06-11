namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CampoEmpresaAtiva : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMPRESA", "EMPRESA_ATIVA", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EMPRESA", "EMPRESA_ATIVA");
        }
    }
}
